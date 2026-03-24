using DemoWebAPI.model.Models;
using DemoWebAPI.Repository.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoWebAPI.Service.User
{
    public class UserService : IUserService
    {

        #region Constructor
        private readonly IUserRepository _userRepository;
        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        #endregion

        #region Methods
        public async Task<List<UserModel>> GetAllUsers()
        {
            var users = await _userRepository.GetAllUsers();
            return users;
        }

        public async Task<UserModel> GetUserById(long UserId)
        {
            var user = await _userRepository.GetUserById(UserId);
            return user;
        }

        public async Task<long> SaveUser(UserModel model)
        {
            var result = await _userRepository.SaveUser(model);
            return result;
        }
        public async Task<long> DeleteUser(long UserId)
        {
            var result = await _userRepository.DeleteUser(UserId);
            return result;
        }

        public async Task<UserModel> GetUserByEmail(string UserEmail)
        {
            var result = await _userRepository.GetUserByEmail(UserEmail);
            return result;
        }

        public async Task<long> AddUserToken(UserJWTModel model)
        {
            var result = await _userRepository.AddUserToken(model);
            return result;
        }

        public Task<long> SaveUserLoginAttempt(LoginAttemptSaveModel model)
        {
            var result =  _userRepository.SaveUserLoginAttempt(model);
            return result;
        }

        public Task<List<LoginAttemptModel>> GetUserLoginAttempts(long UserId)
        {
            var result =  _userRepository.GetUserLoginAttempts(UserId);
            return result;
        }

        public Task<long> ChangeUserLockStatus(ChangeLockStatusModel model)
        {
            var result =  _userRepository.ChangeUserLockStatus(model);
            return result;
        }
        #endregion
    }
}
