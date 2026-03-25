using Dapper;
using DemoWebAPI.Common;
using DemoWebAPI.model.Models;
using GeckoAPI.Common;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
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
            var param = new DynamicParameters();

            var query = GetPgFunctionQuery(
                StoredProcedures.GetAllUsers,
                true
            );

            var response = Query<UserModel>(query, param);
            return Task.FromResult(response.Data.ToList());
        }

        public Task<UserModel> GetUserById(long UserId)
        {
            var param = new DynamicParameters();
            param.Add("@UserId", UserId,DbType.Int32);

            var query = GetPgFunctionQuery(
                StoredProcedures.GetUserById,
                true,
                "@UserId"
            );

            var response = QueryFirstOrDefault<UserModel>(query, param);
            return Task.FromResult(response.Data);
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

            param.Add("@UserId", model.UserId, DbType.Int32);
            param.Add("@UserName", model.UserName);
            param.Add("@UserEmail", model.UserEmail);

            var query = GetPgFunctionQuery(
                StoredProcedures.SaveUser,
                false,
                "@PasswordHash,@PasswordSalt,@UserId,@UserName,@UserEmail"
            );

            var response = Execute(query, param);
            return Task.FromResult(response.Data);
        }

        public Task<long> DeleteUser(long UserId)
        {
            var param = new DynamicParameters();
            param.Add("@UserId", UserId, DbType.Int32);

            var query = GetPgFunctionQuery(
                StoredProcedures.DeleteUser,
                false,
                "@UserId"
            );

            var response = Execute(query, param);
            return Task.FromResult(response.Data);
        }

        public Task<UserModel> GetUserByEmail(string UserEmail)
        {
            var param = new DynamicParameters();
            param.Add("@UserEmail", UserEmail);

            var query = GetPgFunctionQuery(
                StoredProcedures.GetUserByEmail,
                true,
                "@UserEmail"
            );

            var response = QueryFirstOrDefault<UserModel>(query, param);
            return Task.FromResult(response.Data);
        }

        public Task<long> AddUserToken(UserJWTModel model)
        {
            var param = new DynamicParameters();
            param.Add("@UserId", model.UserId, DbType.Int32);
            param.Add("@JwtToken", model.JWTToken);
            param.Add("@JwtCreatedDate", model.JWTCreatedDate);
            param.Add("@JwtExpiryDate", model.JWTExpiryDate);

            var query = GetPgFunctionQuery(
                StoredProcedures.AddUserToken,
                false,
                "@UserId,@JwtToken,@JwtCreatedDate,@JwtExpiryDate"
            );

            var response = Execute(query, param);
            return Task.FromResult(response.Data);
        }

        public Task<long> SaveUserLoginAttempt(LoginAttemptSaveModel model)
        {
            var param = new DynamicParameters();
            param.Add("@UserId", model.UserId, DbType.Int32);
            param.Add("@AttemptTime", model.AttemptTime);
            param.Add("@IsSuccess", model.IsSuccess);

            var query = GetPgFunctionQuery(
                StoredProcedures.SaveLoginAttemptLogs,
                false,
                "@UserId,@AttemptTime,@IsSuccess"
            );

            var response = Execute(query, param);
            return Task.FromResult(response.Data);
        }

        public Task<List<LoginAttemptModel>> GetUserLoginAttempts(long UserId)
        {
            var param = new DynamicParameters();
            param.Add("@UserId", UserId, DbType.Int32);
            param.Add("@Time", DateTime.UtcNow);

            var query = GetPgFunctionQuery(
                StoredProcedures.GetAttemptedLogs,
                true,
                "@UserId,@Time"
            );

            var response = Query<LoginAttemptModel>(query, param);
            return Task.FromResult(response.Data.ToList());
        }

        public Task<long> ChangeUserLockStatus(ChangeLockStatusModel model)
        {
            var param = new DynamicParameters();
            param.Add("@UserId", model.UserId, DbType.Int32);
            param.Add("@LockStatus", model.IsLocked);
            param.Add("@LockedTime", model.LockedTime);

            var query = GetPgFunctionQuery(
                StoredProcedures.ChangeLockStatus,
                false,
                "@UserId,@LockStatus,@LockedTime"
            );

            var response = Execute(query, param);
            return Task.FromResult(response.Data);
        }
        #endregion
    }
}
