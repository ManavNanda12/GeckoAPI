using GeckoAPI.Model.models;
using GeckoAPI.Service.payment;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stripe;

namespace GeckoAPI.Controllers
{
    [Route("api/webhook")]  
    [ApiController]
    public class WebhookController : ControllerBase
    {

        #region Fields
        public readonly IConfiguration _configuration;
        public readonly IPaymentService _paymentService;
        public readonly IWebHostEnvironment _env;
        #endregion

        #region Constructor
        public WebhookController(IConfiguration configuration, IPaymentService paymentService, IWebHostEnvironment env)
        {
            _configuration = configuration;
            _paymentService = paymentService;
            _env = env;
        }
        #endregion


        #region Methods
        [HttpPost]
        public async Task<IActionResult> StripeWebhook()
        {
            string json;

            try
            {
                using var reader = new StreamReader(HttpContext.Request.Body);
                json = await reader.ReadToEndAsync();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error reading request body: {ex}");
            }

            Event stripeEvent;

            if (_env.IsDevelopment())
            {
                stripeEvent = EventUtility.ParseEvent(json, throwOnApiVersionMismatch: false);

                if (stripeEvent == null)
                    return BadRequest("Failed to parse Stripe event.");
            }
            else
            {
                try
                {
                    stripeEvent = EventUtility.ConstructEvent(
                        json,
                        Request.Headers["Stripe-Signature"],
                        _configuration["Stripe:WebhookSecret"]
                    );
                }
                catch (StripeException ex)
                {
                    return BadRequest($"Stripe signature validation failed: {ex.Message}");
                }
            }

            try
            {
                // =========================
                // 🔥 IDEMPOTENCY: log the event first; bail out if we've already processed it.
                // Stripe delivers events at-least-once and retries, so the same event id can
                // arrive multiple times. The SP inserts ON CONFLICT (StripeEventId) DO NOTHING
                // and RETURNs the new row id; a duplicate returns 0.
                // =========================
                string? paymentIntentId = null;
                string? chargeId = null;

                if (stripeEvent.Data.Object is PaymentIntent pi)
                {
                    paymentIntentId = pi.Id;
                    chargeId = pi.LatestChargeId;
                }

                var logRequest = new SaveStripeWebhookEventRequest
                {
                    StripeEventId = stripeEvent.Id,
                    EventType = stripeEvent.Type,
                    ApiVersion = stripeEvent.ApiVersion,
                    PaymentIntentId = paymentIntentId,
                    ChargeId = chargeId,
                    CheckoutSessionId = null,
                    PayloadJson = json
                };

                var insertedId = await _paymentService.SaveWebhookEvent(logRequest);
                if (insertedId == 0)
                {
                    // Already processed — acknowledge so Stripe stops retrying.
                    return Ok();
                }

                // =========================
                // 🔥 HANDLE SUBSCRIPTION EVENTS
                // =========================
                if (stripeEvent.Type == "customer.subscription.created" ||
                    stripeEvent.Type == "customer.subscription.updated" ||
                    stripeEvent.Type == "customer.subscription.deleted")
                {
                    var subscription = stripeEvent.Data.Object as Subscription;

                    if (subscription != null)
                    {
                        var item = subscription.Items?.Data?.FirstOrDefault();

                        DateTime? periodStart = item?.CurrentPeriodStart;
                        DateTime? periodEnd = item?.CurrentPeriodEnd;

                        long? customerIdValue = null;
                        long? planIdValue = null;

                        if (subscription.Metadata != null
                            && subscription.Metadata.TryGetValue("CustomerId", out var customerIdStr)
                            && long.TryParse(customerIdStr, out var parsedCustomerId))
                        {
                            customerIdValue = parsedCustomerId;
                        }

                        if (subscription.Metadata != null
                            && subscription.Metadata.TryGetValue("PlanId", out var planIdStr)
                            && long.TryParse(planIdStr, out var parsedPlanId))
                        {
                            planIdValue = parsedPlanId;
                        }

                        // Without a CustomerId we cannot attribute the subscription to anyone.
                        // Skip the save (the event is still logged above) rather than crashing
                        // on a null unbox, which previously 500'd and caused endless retries.
                        if (customerIdValue.HasValue && planIdValue.HasValue)
                        {
                            var subscriptionRequest = new SaveCustomerSubscriptionRequestModel
                            {
                                CustomerId = customerIdValue.Value,
                                StripeSubscriptionId = subscription.Id,
                                SubscriptionStatus = subscription.Status,
                                PlanId = planIdValue.Value,
                                StripeCustomerId = subscription.CustomerId,
                                CancelAtPeriodEnd = subscription.CancelAtPeriodEnd,
                                CurrentPeriodStart = periodStart,
                                CurrentPeriodEnd = periodEnd
                            };

                            await _paymentService.SaveSubscriptionEvent(subscriptionRequest);
                        }
                    }
                }

                return Ok();
            }
            catch (Exception ex)
            {
                // Returning 500 makes Stripe retry, which is what we want for transient DB failures.
                return StatusCode(500, $"Webhook processing error: {ex}");
            }
        }
        #endregion
    }
}
