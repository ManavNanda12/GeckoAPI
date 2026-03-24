using DemoWebAPI.model.Models;
using GeckoAPI.Model.models;
using GeckoAPI.Service.payment;
using GeckoAPI.Service.plan;
using Microsoft.AspNetCore.Mvc;

namespace GeckoAPI.Controllers
{
    [Route("api/plans")]
    [ApiController]
    public class PlanController : ControllerBase
    {
        #region Fields
        public readonly IPlanService _planService;
        #endregion

        #region Constructor
        public PlanController(IPlanService planService)
        {
            _planService = planService;
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

        [HttpGet("get-plan-details/{planId}")]
        public async Task<BaseAPIResponse<List<PlanSubscriptionDetailModel>>> GetPlanSubscriptionDetails(long planId)
        {
            var response = new BaseAPIResponse<List<PlanSubscriptionDetailModel>>();
            try
            {
                var data = await _planService.GetPlanSubscriptionDetails(planId);
                response.Data = data;
                response.Success = true;
                response.Message = "Plan subscription details fetched successfully.";
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
