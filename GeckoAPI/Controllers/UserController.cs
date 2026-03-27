using DemoWebAPI.model.Models;
using DemoWebAPI.Service.User;
using GeckoAPI.Common;
using GeckoAPI.Model.models;
using GeckoAPI.Service.jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DemoWebApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {

        #region Fields
        private readonly IUserService _userService;
        private readonly IJWTService _jwtService;
        private readonly IConfiguration _configuration;
        #endregion

        #region Constructor
        public UserController(IUserService userService , IJWTService jwtService, IConfiguration configuration)
        {
            _userService = userService;
            _jwtService = jwtService;
            _configuration = configuration;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Get All Users
        /// </summary>
        [HttpGet("get-users")]
        public async Task<BaseAPIResponse<List<UserModel>>> GetAllUsers()
        {
            var response = new BaseAPIResponse<List<UserModel>>();
            try
            {
                // Fetch all users using the service
                var users = await _userService.GetAllUsers();
                
                // Set the response data
                response.Data = users;
                response.Success = true;
                response.Message = "Users fetched successfully.";
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
        /// Get User By Id
        /// </summary>        
        [HttpGet("get-user-by-id/{UserId}")]
        public async Task<BaseAPIResponse<UserModel>> GetUserById(long UserId)
        {
            var response = new BaseAPIResponse<UserModel>();
            try
            {
                // Fetch all users using the service
                var user = await _userService.GetUserById(UserId);

                // Set the response data
                response.Data = user;
                response.Success = true;
                response.Message = "User fetched successfully.";
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
        /// Save User
        /// </summary>        
        [HttpPost("save-user")]
        public async Task<BaseAPIResponse<long>> GetUserById(UserModel model)
        {
            var response = new BaseAPIResponse<long>();
            try
            {
                // Fetch all users using the service
                var result = await _userService.SaveUser(model);
                response.Success = true;
                if (result == 0)
                {
                    response.Message = "User updated successfully.";
                }
                else if (result > 0)
                {
                    response.Message = "User added successfully";
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
        /// Delete User
        /// </summary>        
        [HttpDelete("delete-user/{UserId}")]
        public async Task<BaseAPIResponse<long>> DeleteUser(long UserId)
        {
            var response = new BaseAPIResponse<long>();
            try
            {
                // Fetch all users using the service
                var result = await _userService.DeleteUser(UserId);
                response.Success = true;
                if (result == 0)
                {
                    response.Message = "User deleted successfully.";
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
        /// Login User
        /// </summary> 
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<BaseAPIResponse<UserJWTModel>> Login(UserLoginModel user)
        {
            var response = new BaseAPIResponse<UserJWTModel>();
            var saveModel = new LoginAttemptSaveModel();
            var lockModel = new ChangeLockStatusModel();

            try
            {
                var result = await _userService.GetUserByEmail(user.UserEmail);
                saveModel.UserId = result?.UserId ?? 0;
                saveModel.AttemptTime = DateTime.UtcNow;

                if (result == null)
                {
                    response.Success = false;
                    response.Message = "User does not exist.";
                    return response;
                }

                // check if user is currently locked
                if (result.IsLocked && result.LockedTime.HasValue &&
                    result.LockedTime.Value.AddMinutes(5) > DateTime.UtcNow)
                {
                    response.Success = false;
                    response.Message = "Account is locked. Try again after 5 minutes.";
                    return response;
                }


                // fetch last 5 minutes attempts from SP
                var loginAttempts = await _userService.GetUserLoginAttempts(result.UserId);

                if (loginAttempts != null && loginAttempts.Count >= 3)
                {
                    var lastThree = loginAttempts
                  .OrderByDescending(x => x.AttemptTime)
                  .Take(3)
                  .ToList();

                    if (lastThree.All(x => !x.IsSuccess))
                    {
                        // lock the user
                        lockModel.IsLocked = true;
                        lockModel.LockedTime = DateTime.UtcNow;
                        lockModel.UserId = result.UserId;

                        await _userService.ChangeUserLockStatus(lockModel);

                        response.Success = false;
                        response.Message = "User was locked for 5 minutes because of three wrong attempts";
                        return response;
                    }
                }

                // validate password
                bool isValid = CommonHelper.VerifyPasswordHash(
                    user.Password ?? "", result.PasswordHash ?? "", result.PasswordSalt ?? "");

                if (isValid)
                {
                    var token = _jwtService.GenerateToken(result.UserId.ToString(), result.UserName);
                    var expiryMinutes = _configuration.GetValue<int>("JwtSettings:ExpiryMinutes");

                    var userModel = new UserJWTModel
                    {
                        UserId = result.UserId,
                        JWTToken = token,
                        JWTCreatedDate = DateTime.UtcNow,
                        JWTExpiryDate = DateTime.UtcNow.AddMinutes(expiryMinutes)
                    };

                    var tokenResponse = await _userService.AddUserToken(userModel);
                    if (tokenResponse > 0)
                    {
                        response.Data = userModel;
                        response.Success = true;
                        response.Message = "User logged in successfully.";

                        saveModel.IsSuccess = true;

                        // reset lock if previously locked
                        if (result.IsLocked)
                        {
                            lockModel.IsLocked = false;
                            lockModel.UserId = result.UserId;
                            await _userService.ChangeUserLockStatus(lockModel);
                        }
                    }
                }
                else
                {
                    response.Success = false;
                    response.Message = "Invalid credentials";
                    saveModel.IsSuccess = false;
                }

                await _userService.SaveUserLoginAttempt(saveModel);
                return response;
            }
            catch (Exception ex)
            {
                // log the error internally
                response.Success = false;
                response.Message = "An unexpected error occurred while logging in.";
            }
            return response;
        }

        #endregion
    }
}
