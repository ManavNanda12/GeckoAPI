using DemoWebAPI.model.Models;
using GeckoAPI.Model.models;
using GeckoAPI.Service.sitepolicy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GeckoAPI.CustomerControllers
{
    [Route("api/customer/site-policy")]
    [ApiController]
    public class SitePolicyController : ControllerBase
    {
        #region Fields
        private readonly ISitePolicyService _sitePolicyService;
        #endregion

        #region Constructor
        public SitePolicyController(ISitePolicyService sitePolicyService)
        {
            _sitePolicyService = sitePolicyService;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Get Site Policies
        /// </summary>
        [HttpGet("get-site-policies")]
        public async Task<BaseAPIResponse<List<SitePolicy>>> GetSitePolicies()
        {
            var response = new BaseAPIResponse<List<SitePolicy>>();
            try
            {
                var sitePolicyList = await _sitePolicyService.GetSitePolicies();

                // Set the response data
                response.Data = sitePolicyList;
                response.Success = true;
                response.Message = "Site policy data fetched successfully.";
            }
            catch (Exception ex)
            {
                // Handle exceptions and set error response
                response.Success = false;
                response.Message = $"An error occurred: {ex.Message}";
            }
            return response;
        }

       
        #endregion
    }
}
