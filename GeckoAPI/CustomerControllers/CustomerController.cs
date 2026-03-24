using DemoWebAPI.model.Models;
using DemoWebAPI.Service.User;
using FirebaseAdmin.Messaging;
using GeckoAPI.Common;
using GeckoAPI.Model.models;
using GeckoAPI.Service.customer;
using GeckoAPI.Service.jwt;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GeckoAPI.CustomerControllers
{
    [Route("api/customer")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        #region Fields
        private readonly ICustomerService _customerService;
        private readonly IJWTService _jwtService;
        private readonly IConfiguration _configuration;
        private readonly EmailService _emailService;
        private readonly IWebHostEnvironment _env;
        #endregion

        #region Constructor
        public CustomerController(ICustomerService customerService, IJWTService jwtService, IConfiguration configuration, EmailService emailService, IWebHostEnvironment env)
        {
            _customerService = customerService;
            _jwtService = jwtService;
            _configuration = configuration;
            _emailService = emailService;
            _env = env;
        }
        #endregion

        #region Methods

        /// <summary>
        /// Get Customer By Id
        /// </summary>        
        [HttpGet("get-customer-by-id/{customerId}")]
        public async Task<BaseAPIResponse<Customer>> GetCustomerById(long customerId)
        {
            var response = new BaseAPIResponse<Customer>();
            try
            {

                var customer = await _customerService.GetCustomerById(customerId);
                response.Data = customer;
                response.Success = true;
                response.Message = "Customer fetched successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"An error occurred: {ex.Message}";
            }
            return response;
        }


        /// <summary>
        /// Login Customer
        /// </summary> 
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<BaseAPIResponse<CustomerJWTModel>> Login(CustomerLoginModel customer)
        {
            var response = new BaseAPIResponse<CustomerJWTModel>();

            try
            {
                var result = await _customerService.GetCustomerByEmail(customer.CustomerEmail);

                if (result == null)
                {
                    response.Success = false;
                    response.Message = "Customer does not exist.";
                    return response;
                }

                // validate password
                bool isValid = CommonHelper.VerifyPasswordHash(
                    customer.Password ?? "", result.PasswordHash ?? "", result.PasswordSalt ?? "");

                if (isValid)
                {
                    var token = _jwtService.GenerateToken(result.CustomerId.ToString(), result.FirstName + ' ' + result.LastName);
                    var expiryMinutes = _configuration.GetValue<int>("JwtSettings:ExpiryMinutes");

                    var customerModel = new CustomerJWTModel
                    {
                        CustomerId = result.CustomerId,
                        JWTToken = token,
                        JWTCreatedDate = DateTime.UtcNow,
                        JWTExpiryDate = DateTime.UtcNow.AddMinutes(expiryMinutes)
                    };

                    var tokenResponse = await _customerService.AddCustomerToken(customerModel);
                    if (tokenResponse > 0)
                    {
                        response.Data = customerModel;
                        response.Success = true;
                        response.Message = "Customer logged in successfully.";
                    }
                }
                else
                {
                    response.Success = false;
                    response.Message = "Invalid credentials";
                }

                return response;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "An unexpected error occurred while logging in.";
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
                if (model.CustomerId == 0)
                {
                    model.GeneratedPassword = PasswordGenerator.GenerateRandomPassword();
                }
                var result = await _customerService.SaveCustomer(model);
                response.Success = true;

                if (result == 0 || model.CustomerId > 0)
                {
                    response.Message = "User updated successfully.";
                }
                else if (result > 0 && model.CustomerId == 0)
                {
                    string filePath = Path.Combine(_env.WebRootPath, "EmailTemplates", "CustomerPasswordTemplate.html");

                    // Fix: Use System.IO.File instead of ControllerBase.File
                    string htmlTemplate = await System.IO.File.ReadAllTextAsync(filePath);

                    string customerName = $"{model.FirstName} {model.LastName}".Trim();
                    string htmlBody = htmlTemplate
                                .Replace("{{CustomerName}}", customerName)
                                .Replace("{{MobileNumber}}", model.ContactNumber)
                                .Replace("{{Password}}", model.GeneratedPassword)
                                .Replace("{{Year}}", DateTime.Now.Year.ToString());


                    await _emailService.SendCustomerGeneratedPasswordMail(
                            model.Email,
                            customerName,
                            htmlBody
                    );
                    response.Message = "Your account has been created successfully. You will receive password soon.";
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

        [AllowAnonymous]
        [HttpPost("google-login")]
        public async Task<BaseAPIResponse<CustomerJWTModel>> GoogleLogin([FromBody] GoogleLoginModel model)
        {
            var response = new BaseAPIResponse<CustomerJWTModel>();

            try
            {
                // ✅ 1. Verify Google Token
                var payload = await VerifyGoogleToken(model.Token);

                if (payload == null)
                {
                    response.Success = false;
                    response.Message = "Invalid Google token.";
                    return response;
                }

                // ✅ 2. Check if user exists
                var user = await _customerService.GetCustomerByEmail(payload.Email);

                // 🔥 3. If NOT exists → CREATE USER
                if (user == null)
                {
                    var newCustomer = new CustomerSaveModel
                    {
                        CustomerId = 0,
                        FirstName = payload.GivenName ?? "",
                        LastName = payload.FamilyName ?? "",
                        Email = payload.Email,
                        CountryCode = "+91",
                        ContactNumber = "1234567890"
                        // Password fields auto handled
                    };

                    var saveResult = await _customerService.SaveCustomer(newCustomer);

                    if (saveResult <= 0)
                    {
                        response.Success = false;
                        response.Message = "Failed to create user.";
                        return response;
                    }

                    // ✅ Fetch newly created user
                    user = await _customerService.GetCustomerByEmail(payload.Email);
                }

                // ✅ 4. Generate JWT
                var token = _jwtService.GenerateToken(user.CustomerId.ToString(), user.FirstName + ' ' + user.LastName);
                var expiryMinutes = _configuration.GetValue<int>("JwtSettings:ExpiryMinutes");

                var customerModel = new CustomerJWTModel
                {
                    CustomerId = user.CustomerId,
                    JWTToken = token,
                    JWTCreatedDate = DateTime.UtcNow,
                    JWTExpiryDate = DateTime.UtcNow.AddMinutes(expiryMinutes)
                };

                await _customerService.AddCustomerToken(customerModel);

                response.Data = customerModel;
                response.Success = true;
                response.Message = "Google login successful.";

                return response;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "Error during Google login.";
                return response;
            }
        }

        private async Task<GoogleJsonWebSignature.Payload> VerifyGoogleToken(string token)
        {
            try
            {
                var settings = new GoogleJsonWebSignature.ValidationSettings()
                {
                    Audience = new List<string> { _configuration["GoogleAuth:ClientId"] }
                };

                var payload = await GoogleJsonWebSignature.ValidateAsync(token, settings);

                return payload;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Update FCM Token
        /// </summary>        
        [HttpPost("update-fcm-token")]
        public async Task<BaseAPIResponse<long>> UpdateFCMToken(FCMTokenUpdateRequestModel model)
        {
            var response = new BaseAPIResponse<long>();
            try
            {
                // Fetch all users using the service
                var result = await _customerService.UpdateFCMToken(model);
                response.Success = true;
                if (result == 1)
                {
                    response.Message = "Customer FCMToken updated successfully.";
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
