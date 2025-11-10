using DemoWebAPI.model.Models;
using DemoWebAPI.Service.User;
using GeckoAPI.Model.models;
using GeckoAPI.Service.dashboard;
using GeckoAPI.Service.jwt;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GeckoAPI.Controllers
{
    [Route("api/dashboard")]
    [ApiController]
    public class DashboardController : ControllerBase
    {

        #region Fields
        private readonly IDashboardService _dashboardService;
        private readonly IJWTService _jwtService;
        private readonly IConfiguration _configuration;
        #endregion

        #region Constructor
        public DashboardController(IDashboardService dashboardService, IJWTService jwtService, IConfiguration configuration)
        {
            _dashboardService = dashboardService;
            _jwtService = jwtService;
            _configuration = configuration;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Get dashboard count
        /// </summary>
        [HttpGet("get-dashboard-count")]
        public async Task<BaseAPIResponse<Dashboard>> GetDashboardCount()
        {
            var response = new BaseAPIResponse<Dashboard>();
            try
            {
                // Fetch dashboard count using the service
                var dashboard = await _dashboardService.GetDashboardCount();

                // Set the response data
                response.Data = dashboard;
                response.Success = true;
                response.Message = "Dashboard count fetched successfully.";
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
