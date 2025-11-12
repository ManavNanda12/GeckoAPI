using DemoWebAPI.model.Models;
using GeckoAPI.Model.models;
using GeckoAPI.Service.contactus;
using GeckoAPI.Service.wishlist;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GeckoAPI.CustomerControllers
{
    [Route("api/customer/contactus")]
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
        /// Save contact us request
        /// </summary>        
        [HttpPost("save-contactus-request")]
        public async Task<BaseAPIResponse<long>> SaveWishlistItem(ContactUs model)
        {
            var response = new BaseAPIResponse<long>();
            try
            {

                var isAdded = await _contactusService.SaveContactRequest(model);
                if (isAdded == 1)
                {
                    response.Success = true;
                    response.Message = "Contact us replied successfully.";
                }
                else if(isAdded == 0)
                {
                    response.Success = true;
                    response.Message = "Thanks for contacting";
                }
                else
                {
                    response.Success = false;
                    response.Message = "Failed to submit contact us request.";
                }
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
