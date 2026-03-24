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
        #endregion

        #region Constructor
        public WebhookController(IConfiguration configuration, IPaymentService paymentService)
        {
            _configuration = configuration;
            _paymentService = paymentService;
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

            try
            {
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
                        long? customerIdValue = null;
                        long? planIdValue = null;
                        var item = subscription.Items?.Data?.FirstOrDefault();

                        DateTime? periodStart = item?.CurrentPeriodStart;
                        DateTime? periodEnd = item?.CurrentPeriodEnd;

                        if (subscription.Metadata.TryGetValue("CustomerId", out var customerIdStr)
                            && long.TryParse(customerIdStr, out var parsedCustomerId))
                        {
                            customerIdValue = parsedCustomerId;
                        }

                        if (subscription.Metadata.TryGetValue("PlanId", out var planIdStr)
                            && long.TryParse(planIdStr, out var parsedPlanId))
                        {
                            planIdValue = parsedPlanId;
                        }

                        var subscriptionRequest = new SaveCustomerSubscriptionRequestModel
                        {
                            CustomerId = (long)customerIdValue,
                            StripeSubscriptionId = subscription.Id,
                            SubscriptionStatus = subscription.Status,
                            PlanId = (long)planIdValue,
                            CancelAtPeriodEnd = subscription.CancelAtPeriodEnd,
                            CurrentPeriodStart = periodStart,
                            CurrentPeriodEnd = periodEnd
                        };

                        await _paymentService.SaveSubscriptionEvent(subscriptionRequest);
                    }
                }

                // =========================
                // 🔥 SAVE GENERIC WEBHOOK LOG
                // =========================
                string? paymentIntentId = null;
                string? chargeId = null;

                if (stripeEvent.Data.Object is PaymentIntent pi)
                {
                    paymentIntentId = pi.Id;
                    chargeId = pi.LatestChargeId;
                }

                var request = new SaveStripeWebhookEventRequest
                {
                    StripeEventId = stripeEvent.Id,
                    EventType = stripeEvent.Type,
                    ApiVersion = stripeEvent.ApiVersion,
                    PaymentIntentId = paymentIntentId,
                    ChargeId = chargeId,
                    CheckoutSessionId = null,
                    PayloadJson = json
                };

                await _paymentService.SaveWebhookEvent(request);

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Webhook processing error: {ex}");
            }
        }
        #endregion
    }
}
