using Dapper;
using DemoWebAPI.Common;
using DemoWebAPI.model.Models;
using GeckoAPI.Common;
using GeckoAPI.Model.models;
using Microsoft.Extensions.Options;
using NpgsqlTypes;
using System.Data;

namespace GeckoAPI.Repository.customer
{
    public class CustomerRepository : BaseRepository, ICustomerRepository
    {
        #region Constructor
        public CustomerRepository(IOptions<DbConfig> config) : base(config)
        {
        }
        #endregion

        #region Methods

        public Task<Model.models.Customer> GetCustomerById(long CustomerId)
        {
            var param = new DynamicParameters();
            param.Add("CustomerId", CustomerId);

            var query = GetPgFunctionQuery(StoredProcedures.GetCustomerById, true, "@CustomerId");

            var customer = QueryFirstOrDefault<Model.models.Customer>(query, param);
            return Task.FromResult(customer.Data);
        }

        public Task<Model.models.Customer> GetCustomerByEmail(string CustomerEmail)
        {
            var param = new DynamicParameters();
            param.Add("CustomerEmail", CustomerEmail);

            var query = GetPgFunctionQuery(StoredProcedures.GetCustomerByEmail, true, "@CustomerEmail");

            var response = QueryFirstOrDefault<Model.models.Customer>(query, param);
            return Task.FromResult(response.Data);
        }

        public Task<long> AddCustomerToken(CustomerJWTModel model)
        {
            var param = new DynamicParameters();
            param.Add("CustomerId", model.CustomerId);
            param.Add("JwtToken", model.JWTToken);
            param.Add("JwtCreatedDate", DateTime.SpecifyKind(model.JWTCreatedDate, DateTimeKind.Unspecified));
            param.Add("JwtExpiryDate", DateTime.SpecifyKind(model.JWTExpiryDate, DateTimeKind.Unspecified));
            var query = GetPgFunctionQuery(
                StoredProcedures.AddCustomerToken,
                false,
                "@CustomerId, @JwtToken, @JwtCreatedDate, @JwtExpiryDate"
            );
            var response = Execute(query, param);
            return Task.FromResult(response.Data);
        }

        public Task<List<CustomerListModel>> GetCustomerList(CommonListRequestModel model)
        {
            var param = new DynamicParameters();
            param.Add("PageNumber", model.PageNumber, DbType.Int32);
            param.Add("PageSize", model.PageSize, DbType.Int32);
            param.Add("SearchTerm", model.SearchTerm);
            param.Add("SortColumn", model.SortColumn);
            param.Add("SortDirection", model.SortDirection);

            var query = GetPgFunctionQuery(
                StoredProcedures.GetCustomerList,
                true,
                "@PageNumber, @PageSize, @SearchTerm, @SortColumn, @SortDirection"
            );

            var customers = Query<CustomerListModel>(query, param);
            return Task.FromResult(customers.Data.ToList());
        }

        public Task<long> SaveCustomer(CustomerSaveModel model)
        {
            var param = new DynamicParameters();

            if (model.CustomerId == 0)
            {
                CommonHelper.CreatePasswordHash(
                    model.GeneratedPassword ?? "Admin@123",
                    out string passwordHash,
                    out string passwordSalt
                );

                param.Add("PasswordHash", passwordHash);
                param.Add("PasswordSalt", passwordSalt);
            }
            else
            {
                param.Add("PasswordHash", model.PasswordHash ?? "");
                param.Add("PasswordSalt", model.PasswordSalt ?? "");
            }

            param.Add("CustomerId", model.CustomerId);
            param.Add("FirstName", model.FirstName);
            param.Add("LastName", model.LastName);
            param.Add("Email", model.Email);
            param.Add("CountryCode", model.CountryCode);
            param.Add("ContactNumber", model.ContactNumber);
            param.Add("CreatedBy", model.CreatedBy);

            var query = GetPgFunctionQuery(
                StoredProcedures.SaveCustomer,
                false,
                "@CustomerId, @FirstName, @LastName, @Email, @CountryCode, @ContactNumber, @CreatedBy , @PasswordHash, @PasswordSalt"
            );

            var isSaved = Execute(query, param);
            return Task.FromResult(isSaved.Data);
        }

        public Task<long> SendCustomerWelcomeMail(CustomerWelcomeEmailRequestModel model)
        {
            var param = new DynamicParameters();
            param.Add("CustomerId", model.CustomerId);
            param.Add("EmailTemplate", model.EmailTemplate);

            var query = GetPgFunctionQuery(
                StoredProcedures.UpdateCustomerWelcomeStatus,
                false,
                "@CustomerId, @EmailTemplate"
            );

            var isSaved = Execute(query, param);
            return Task.FromResult(isSaved.Data);
        }

        public Task<long> DeleteCustomer(long CustomerId)
        {
            var param = new DynamicParameters();
            param.Add("CustomerId", CustomerId);

            var query = GetPgFunctionQuery(
                StoredProcedures.DeleteCustomer,
                false,
                "@CustomerId"
            );

            var response = Execute(query, param);
            return Task.FromResult(response.Data);
        }

        public Task<long> UpdateFCMToken(FCMTokenUpdateRequestModel model)
        {
            var param = new DynamicParameters();
            param.Add("CustomerId", model.CustomerId);
            param.Add("FCMToken", model.FCMToken);

            var query = GetPgFunctionQuery(
                StoredProcedures.UpdateFCMToken,
                false,
                "@CustomerId, @FCMToken"
            );

            var response = Execute(query, param);
            return Task.FromResult(response.Data);
        }

        public Task<List<InactiveCartUserNotificationResponseModel>> GetInactiveCartCustomers()
        
        {
            var param = new DynamicParameters();
            param.Add("Minutes", 5);

            var query = GetPgFunctionQuery(
                StoredProcedures.GetInactiveCartUsers,
                true,
                "@Minutes"
            );

            var customers = Query<InactiveCartUserNotificationResponseModel>(query, param);
            return Task.FromResult(customers.Data.ToList());
        }

        public Task<long> SaveNotification(NotificationSaveRequestModel model)
        {
            var param = new DynamicParameters();
            param.Add("CartId", model.CartId, DbType.Int32);
            param.Add("CustomerId", model.CustomerId, DbType.Int32);
            param.Add("NotificationType", model.NotificationType);

            var query = GetPgFunctionQuery(
                StoredProcedures.InsertCartNotificationLog,
                false,
                "@CartId, @CustomerId, @NotificationType"
            );

            var response = Execute(query, param);
            return Task.FromResult(response.Data);
        }

        #endregion
    }
}