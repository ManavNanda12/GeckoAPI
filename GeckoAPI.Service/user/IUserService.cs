using DemoWebAPI.model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoWebAPI.Service.User
{
    public interface IUserService
    {
        Task<List<UserModel>> GetAllUsers();
        Task<UserModel> GetUserById(long UserId);
        Task<long> SaveUser(UserModel model);
        Task<long> DeleteUser(long UserId);
        Task<UserModel> GetUserByEmail(string UserEmail);
        Task<long> AddUserToken(UserJWTModel model);
        Task<long> SaveUserLoginAttempt(LoginAttemptSaveModel model);
        Task<List<LoginAttemptModel>> GetUserLoginAttempts(long UserId);
        Task<long> ChangeUserLockStatus(ChangeLockStatusModel model);
    }
}
