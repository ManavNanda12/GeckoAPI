using DemoWebAPI.model.Models;
using GeckoAPI.Common;
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
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly EmailService _emailService;
        private readonly IWebHostEnvironment _env;
        #endregion

        #region Constructor
        public ContactUsController(IContactusService contactusService , IHttpContextAccessor httpContextAccessor, EmailService emailService , IWebHostEnvironment env)
        {
            _contactusService = contactusService;
            _env = env;
            _emailService = emailService;
            _httpContextAccessor = httpContextAccessor;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Get contact us list
        /// </summary>
        [HttpPost("get-contactus-list")]
        public async Task<BaseAPIResponse<List<ContactUs>>> GetContactUsList(CommonListRequestModel model)
        {
            var response = new BaseAPIResponse<List<ContactUs>>();
            try
            {
                var contactusList = await _contactusService.GetContactUsList(model);

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
                    string filePath = Path.Combine(_env.WebRootPath, "EmailTemplates", "CustomerReplyTemplate.html");
                    string htmlTemplate = await System.IO.File.ReadAllTextAsync(filePath);

                    string htmlBody = htmlTemplate
                        .Replace("{{CustomerName}}", model.CustomerName)
                        .Replace("{{OriginalSubject}}", model.ContactSubject)
                        .Replace("{{OriginalDate}}", DateTime.Now.ToString("MMMM dd, yyyy"))
                        .Replace("{{OriginalMessage}}", model.CustomerMessage)
                        .Replace("{{ReplyMessage}}", model.AdminMessage)
                        .Replace("{{AdminName}}", "Manav")
                        .Replace("{{AdminTitle}}", "CEO")
                        .Replace("{{AdminEmail}}", "manav@evincedev.com")
                        .Replace("{{AdminPhone}}", "8780160945")
                        .Replace("{{Year}}", DateTime.Now.Year.ToString());

                    await _emailService.SendEmailAsync(model.CustomerEmail, model.CustomerName, "Response to Your Inquiry - FitLife Gym", htmlBody);
                    await _emailService.SendEmailAsync("manav@evincedev.com", "Manav", "Provided response to request", htmlBody);
                }
                else if (isAdded == 0)
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
