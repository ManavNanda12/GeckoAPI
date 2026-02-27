using DemoWebAPI.model.Models;
using GeckoAPI.Model.models;
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
        #endregion

        #region Constructor
        public PlanController (IPlanService planService)
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
        #endregion
    }
}
