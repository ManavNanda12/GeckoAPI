using DemoWebAPI.model.Models;
using GeckoAPI.Common;
using GeckoAPI.Model.models;
using GeckoAPI.Service.contactus;
using GeckoAPI.Service.sitepolicy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GeckoAPI.Controllers
{
    [Route("api/site-policy")]
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

        /// <summary>
        /// Update Site Policies
        /// </summary>
        [HttpPost("update-policy")]
        public async Task<BaseAPIResponse<long>> SaveSitePolicy(SitePolicy model)
        {
            var response = new BaseAPIResponse<long>();
            try
            {
                var isUpdated = await _sitePolicyService.SaveSitePolicy(model);

                if(isUpdated == 1)
                {
                    response.Success = true;
                    response.Message = "Site policy data updated successfully.";
                }
                else
                {
                    response.Success = false;
                    response.Message = "Some error occured updating site policy data.";
                }
                   
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
