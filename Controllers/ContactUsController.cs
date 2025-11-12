using DemoWebAPI.model.Models;
using GeckoAPI.Model.models;
using GeckoAPI.Service.contactus;
using GeckoAPI.Service.dashboard;
using GeckoAPI.Service.jwt;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GeckoAPI.Controllers
{
    [Route("api/contactus")]
    [ApiController]
    public class ContactUsController : ControllerBase
    {
        #region Fields
        private readonly IContactusService _contactusService;
        #endregion

        #region Constructor
        public ContactUsController(IContactusService contactusService)
        {
            _contactusService = contactusService;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Get contact us list
        /// </summary>
        [HttpGet("get-contactus-list")]
        public async Task<BaseAPIResponse<List<ContactUs>>> GetContactUsList()
        {
            var response = new BaseAPIResponse<List<ContactUs>>();
            try
            {
                var contactusList = await _contactusService.GetContactUsList();

                // Set the response data
                response.Data = contactusList;
                response.Success = true;
                response.Message = "Contact us data fetched successfully.";
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
