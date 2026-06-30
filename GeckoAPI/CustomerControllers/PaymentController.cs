using DemoWebAPI.model.Models;
using GeckoAPI.Model.models;
using GeckoAPI.Service.payment;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;

namespace GeckoAPI.CustomerControllers
{
    [Route("api/customer/payment")]
    [ApiController]
    public class PaymentController : ControllerBase
    {

        #region Fields
        public readonly IPaymentService _paymentService;
        #endregion

        #region Constructor
        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }
        #endregion

        #region Methods
        [HttpPost("create-payment-intent")]
        public async Task<BaseAPIResponse<string>> CreatePaymentIntent([FromBody] PaymentIntentRequestModel model)
        {
            var response = new BaseAPIResponse<string>();
            try
            {
                var paymentIntent = await _paymentService.CreatePaymentIntent(model);
                response.Data = paymentIntent;
                response.Success = true;
                response.Message = "Payment intent created successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"An error occurred: {ex.Message}";
            }
            return response;
        }

        [HttpPost("change-plan")]
        public async Task<BaseAPIResponse<string>> ChangePlan([FromBody] ChangePlanRequest model)
        {
            var response = new BaseAPIResponse<string>();
            try
            {
                // Authoritative customer id comes from the JWT (set by CustomMiddleware),
                // never from the request body — otherwise a caller could change someone else's plan.
                if (HttpContext.Items["UserId"] is int customerId)
                {
                    model.CustomerId = customerId;
                }
                else
                {
                    response.Success = false;
                    response.Message = "You must be logged in to change your plan.";
                    return response;
                }

                var redirectUrl = await _paymentService.ChangePlanAsync(model);
                response.Data = redirectUrl; // null when handled without a checkout redirect
                response.Success = true;
                response.Message = string.IsNullOrEmpty(redirectUrl)
                    ? "Plan updated successfully."
                    : "Redirecting to checkout...";
            }
            catch (BusinessRuleException ex)
            {
                // Safe, user-facing message (e.g. "You already have this plan active.").
                response.Success = false;
                response.Message = ex.Message;
            }
            catch (Exception)
            {
                // Don't leak Stripe/DB internals to the client.
                response.Success = false;
                response.Message = "We couldn't update your plan right now. Please try again.";
            }
            return response;
        }
        #endregion
    }
}
