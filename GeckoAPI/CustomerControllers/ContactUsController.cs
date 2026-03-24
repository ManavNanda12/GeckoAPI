using DemoWebAPI.model.Models;
using GeckoAPI.Common;
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
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly EmailService _emailService;
        private readonly IWebHostEnvironment _env;
        #endregion

        #region Constructor
        public ContactUsController(IContactusService contactusService, IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor, EmailService emailService)
        {
            _contactusService = contactusService;
            _env = env;
            _emailService = emailService;
            _httpContextAccessor = httpContextAccessor;
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
                    string thanksFilePath = Path.Combine(_env.WebRootPath, "EmailTemplates", "CustomerThankYouTemplate.html");
                    string thanksHtmlTemplate = await System.IO.File.ReadAllTextAsync(thanksFilePath);

                    string thanksHtmlBody = thanksHtmlTemplate
                        .Replace("{{CustomerName}}", model.CustomerName)
                        .Replace("{{Year}}", DateTime.Now.Year.ToString());
                    await _emailService.SendEmailAsync(model.CustomerEmail, model.CustomerName, "Thank You for Contacting Us", thanksHtmlBody);


                    string notifyFilePath = Path.Combine(_env.WebRootPath, "EmailTemplates", "AdminNotificationTemplate.html");
                    string notifyHtmlTemplate = await System.IO.File.ReadAllTextAsync(notifyFilePath);

                    string notifyHtmlBody = notifyHtmlTemplate
                        .Replace("{{CustomerName}}", model.CustomerName)
                        .Replace("{{CustomerEmail}}", model.CustomerEmail)
                        .Replace("{{Subject}}", model.ContactSubject)
                        .Replace("{{Message}}", model.CustomerMessage)
                        .Replace("{{DateTime}}", DateTime.Now.ToString("MMMM dd, yyyy hh:mm tt"))
                        .Replace("{{Year}}", DateTime.Now.Year.ToString());

                    await _emailService.SendEmailAsync("manav@evincedev.com", "Admin", "New Customer Inquiry", notifyHtmlBody);

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
