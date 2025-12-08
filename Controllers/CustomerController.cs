using DemoWebAPI.model.Models;
using GeckoAPI.Common;
using GeckoAPI.Model.models;
using GeckoAPI.Service.customer;
using GeckoAPI.Service.product;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GeckoAPI.Controllers
{
    [Route("api/customers")]
    [ApiController]
    public class CustomerController : ControllerBase
    {

        #region Fields
        private readonly ICustomerService _customerService;
        private readonly IWebHostEnvironment _environment;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly EmailService _emailService;
        private readonly IWebHostEnvironment _env;
        #endregion

        #region Constructor
        public CustomerController(ICustomerService customerService, IWebHostEnvironment environment, IHttpContextAccessor httpContextAccessor, EmailService emailService, IWebHostEnvironment env)
        {
            _customerService = customerService;
            _environment = environment;
            _httpContextAccessor = httpContextAccessor;
            _emailService = emailService;
            _env = env;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Get All Customers
        /// </summary>
        [HttpPost("get-customer-list")]
        public async Task<BaseAPIResponse<List<CustomerListModel>>> GetCustomerList(CommonListRequestModel model)
        {
            var response = new BaseAPIResponse<List<CustomerListModel>>();
            try
            {
                var customers = await _customerService.GetCustomerList(model);
                response.Data = customers;
                response.Success = true;
                response.Message = "Customers fetched successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"An error occurred: {ex.Message}";
            }
            return response;
        }

        /// <summary>
        /// Save Customer
        /// </summary>        
        [HttpPost("save-customer")]
        public async Task<BaseAPIResponse<long>> SaveCustomer(CustomerSaveModel model)
        {
            var response = new BaseAPIResponse<long>();
            try
            {
                // Fetch all users using the service
                var result = await _customerService.SaveCustomer(model);
                response.Success = true;

                if (result == 0)
                {
                    response.Message = "User updated successfully.";
                }
                else if (result > 0)
                {
                    response.Message = "User added successfully";
                }
                else if (result == -1)
                {
                    response.Success = false;
                    response.Message = "Email is already used";
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

        /// <summary>
        /// Get All Customers For Welcome Email Send
        /// </summary>
        [HttpGet("send-welcome-email")]
        public async Task<BaseAPIResponse<List<CustomerListModel>>> SendWelcomeEmail()
        {
            var response = new BaseAPIResponse<List<CustomerListModel>>();
            CommonListRequestModel model = new CommonListRequestModel
            {
                PageNumber = 1,
                PageSize = 1000,
                SearchTerm ="",
                SortColumn = "",
                SortDirection = ""
            };
            var customers = await _customerService.GetCustomerList(model);
            var filteredCustomers = customers.Where(x => x.IsWelcomeMailSent == false);
            if (filteredCustomers.Any())
            {
                try
                {
                    string filePath = Path.Combine(_env.WebRootPath, "EmailTemplates", "WelcomeTemplate.html");

                    // Fix: Use System.IO.File instead of ControllerBase.File
                    string htmlTemplate = await System.IO.File.ReadAllTextAsync(filePath);

                    foreach (var customer in filteredCustomers)
                    {
                        string customerName = $"{customer.FirstName} {customer.LastName}".Trim();
                        // Replace placeholders in template
                        string htmlBody = htmlTemplate
                            .Replace("{{CustomerName}}", customerName);
                        bool sent = await _emailService.SendWelcomeEmailAsync(
                            customer.Email,
                            customerName,
                            htmlBody
                        );

                        if (sent)
                        {
                            CustomerWelcomeEmailRequestModel requestModel = new CustomerWelcomeEmailRequestModel
                            {
                                CustomerId = customer.CustomerId,
                                EmailTemplate = htmlBody,
                            };
                            await _customerService.SendCustomerWelcomeMail(requestModel);
                        }
                        await Task.Delay(200);
                    }
                    response.Data = customers;
                    response.Success = true;
                    response.Message = "Customers fetched successfully.";
                }
                catch (Exception ex)
                {
                    response.Success = false;
                    response.Message = $"An error occurred: {ex.Message}";
                }
            }

            return response;
        }

        /// <summary>
        /// Delete Customer
        /// </summary>        
        [HttpDelete("delete-customer/{CustomerId}")]
        public async Task<BaseAPIResponse<long>> DeleteCustomer(long CustomerId)
        {
            var response = new BaseAPIResponse<long>();
            try
            {
                // Fetch all users using the service
                var result = await _customerService.DeleteCustomer(CustomerId);
                response.Success = true;
                if (result == 0)
                {
                    response.Message = "Customer deleted successfully.";
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
