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
                var redirectUrl = await _paymentService.ChangePlanAsync(model);
                response.Data = redirectUrl; // may be null if free plan
                response.Success = true;
                response.Message = "Plan updated successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"An error occurred: {ex.Message}";
            }
            return response;
        }
        #endregion
    }
}
