using DemoWebAPI.model.Models;
using GeckoAPI.Model.models;
using GeckoAPI.Service.payment;
using GeckoAPI.Service.plan;
using Microsoft.AspNetCore.Mvc;

namespace GeckoAPI.CustomerControllers
{
    [Route("api/customer/plan")]
    [ApiController]
    public class PlanController : ControllerBase
    {
        #region Fields
        public readonly IPlanService _planService;
        public readonly IPaymentService _paymentService;
        #endregion

        #region Constructor
        public PlanController(IPlanService planService, IPaymentService paymentService)
        {
            _planService = planService;
            _paymentService = paymentService;
        }
        #endregion

        #region Methods

        [HttpGet("get-plan-list/{customerId}")]
        public async Task<BaseAPIResponse<List<PlanListResponseModel>>> GetPlanList(long customerId)
        {
            var response = new BaseAPIResponse<List<PlanListResponseModel>>();
            try
            {
                var data = await _planService.GetPlanList(customerId);
                response.Data = data;
                response.Success = true;
                response.Message = "Plans fetched successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"An error occurred: {ex.Message}";
            }
            return response;
        }

        [HttpPost("check-plan")]
        public async Task<BaseAPIResponse<PlanCheckResponseModel>> CheckPlan(PlanCheckRequestModel model)
        {
            var response = new BaseAPIResponse<PlanCheckResponseModel>();
            try
            {
                // Use the authenticated customer id rather than trusting the body.
                if (HttpContext.Items["UserId"] is int customerId)
                {
                    model.CustomerId = customerId;
                }

                var data = await _paymentService.CheckPlan(model);
                response.Data = data;
                response.Success = true;
                response.Message = "Plan validation successful.";
            }
            catch (Exception)
            {
                response.Success = false;
                response.Message = "We couldn't validate your plan right now. Please try again.";
            }
            return response;
        }

        // NOTE: the change-plan endpoint lives on PaymentController ("api/customer/payment/change-plan"),
        // which is what the portal calls. The duplicate that used to be here was removed to avoid drift.

        #endregion
    }
}