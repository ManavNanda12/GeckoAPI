using Dapper;
using DemoWebAPI.Common;
using DemoWebAPI.model.Models;
using GeckoAPI.Common;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoWebAPI.Repository.User
{
    public class UserRepository : BaseRepository, IUserRepository
    {

        #region Constructor
        public UserRepository(IOptions<DbConfig> config) : base(config)
        {
        }
        #endregion

        #region Methods
        public Task<List<UserModel>> GetAllUsers()
        {
            var users = Query<UserModel>(StoredProcedures.GetAllUsers);
            return Task.FromResult(users.Data.ToList());
        }

        public Task<UserModel> GetUserById(long UserId)
        {
            var param = new DynamicParameters();
            param.Add("@UserId", UserId);
            var user = QueryFirstOrDefault<UserModel>(StoredProcedures.GetUserById,param);
            return Task.FromResult(user.Data);
        }

        public Task<long> SaveUser(UserModel model)
        {
            var param = new DynamicParameters();
            if (model.UserId == 0)
            {
                model.Password ??= "Admin@123";
                CommonHelper.CreatePasswordHash(model.Password, out string passwordHash, out string passwordSalt);
                param.Add("@PasswordHash", passwordHash);
                param.Add("@PasswordSalt", passwordSalt);
            }
            else
            {
                param.Add("@PasswordHash", model.PasswordHash ?? "");
                param.Add("@PasswordSalt", model.PasswordSalt ?? "");
            }
            param.Add("@UserId", model.UserId);
            param.Add("@UserName", model.UserName);
            param.Add("@UserEmail", model.UserEmail);
            var response = Execute(StoredProcedures.SaveUser,param);
            return Task.FromResult(response.Data);
        }

        public Task<long> DeleteUser(long UserId)
        {
            var param = new DynamicParameters();
            param.Add("@UserId", UserId);
            var response = Execute(StoredProcedures.DeleteUser,param);
            return Task.FromResult(response.Data);
        }

        public Task<UserModel> GetUserByEmail(string UserEmail)
        {
            var param = new DynamicParameters();
            param.Add("@UserEmail", UserEmail);
            var response = QueryFirstOrDefault<UserModel>(StoredProcedures.GetUserByEmail, param);
            return Task.FromResult(response.Data);
        }

        public Task<long> AddUserToken(UserJWTModel model)
        {
            var param = new DynamicParameters();
            param.Add("@UserId", model.UserId);
            param.Add("@JwtToken", model.JWTToken);
            param.Add("@JwtCreatedDate", model.JWTCreatedDate);
            param.Add("@JwtExpiryDate", model.JWTExpiryDate);
            var response = Execute(StoredProcedures.AddUserToken, param);
            return Task.FromResult(response.Data);
        }

        public Task<long> SaveUserLoginAttempt(LoginAttemptSaveModel model)
        {
            var param = new DynamicParameters();
            param.Add("@UserId", model.UserId);
            param.Add("@AttemptTime", model.AttemptTime);
            param.Add("@IsSuccess", model.IsSuccess);
            var response = Execute(StoredProcedures.SaveLoginAttemptLogs, param);
            return Task.FromResult(response.Data);
        }

        public Task<List<LoginAttemptModel>> GetUserLoginAttempts(long UserId)
        {
            var param = new DynamicParameters();
            param.Add("@UserId", UserId);
            param.Add("@Time", DateTime.UtcNow);
            var response = Query<LoginAttemptModel>(StoredProcedures.GetAttemptedLogs, param);
            return Task.FromResult(response.Data.ToList());
        }

        public Task<long> ChangeUserLockStatus(ChangeLockStatusModel model)
        {
            var param = new DynamicParameters();
            param.Add("@UserId", model.UserId);
            param.Add("@LockStatus", model.IsLocked);
            param.Add("@LockedTime", model.LockedTime);
            var response = Execute(StoredProcedures.ChangeLockStatus, param);
            return Task.FromResult(response.Data);
        }
        #endregion
    }
}
