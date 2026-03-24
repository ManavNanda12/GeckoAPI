using Dapper;
using DemoWebAPI.Common;
using DemoWebAPI.model.Models;
using GeckoAPI.Common;
using GeckoAPI.Model.models;
using Microsoft.Extensions.Options;

namespace GeckoAPI.Repository.customer
{
    public class CustomerRepository:BaseRepository,ICustomerRepository
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
            param.Add("@CustomerId", CustomerId);
            var customer = QueryFirstOrDefault<Model.models.Customer>(StoredProcedures.GetCustomerById, param);
            return Task.FromResult(customer.Data);
        }

        public Task<Model.models.Customer> GetCustomerByEmail(string CustomerEmail)
        {
            var param = new DynamicParameters();
            param.Add("@CustomerEmail", CustomerEmail);
            var response = QueryFirstOrDefault<Model.models.Customer>(StoredProcedures.GetCustomerByEmail, param);
            return Task.FromResult(response.Data);
        }

        public Task<long> AddCustomerToken(CustomerJWTModel model)
        {
            var param = new DynamicParameters();
            param.Add("@CustomerId", model.CustomerId);
            param.Add("@JwtToken", model.JWTToken);
            param.Add("@JwtCreatedDate", model.JWTCreatedDate);
            param.Add("@JwtExpiryDate", model.JWTExpiryDate);
            var response = Execute(StoredProcedures.AddCustomerToken, param);
            return Task.FromResult(response.Data);
        }

        public Task<List<CustomerListModel>> GetCustomerList(CommonListRequestModel model)
        {
            var param = new DynamicParameters();
            param.Add("@PageNumber", model.PageNumber);
            param.Add("@PageSize", model.PageSize);
            param.Add("@SearchTerm", model.SearchTerm);
            param.Add("@SortColumn", model.SortColumn);
            param.Add("@SortDirection", model.SortDirection);
            var customers = Query<CustomerListModel>(StoredProcedures.GetCustomerList, param);
            return Task.FromResult(customers.Data.ToList());
        }

        public Task<long> SaveCustomer(CustomerSaveModel model)
        {
            var param = new DynamicParameters();
            if (model.CustomerId == 0)
            {
                CommonHelper.CreatePasswordHash(model.GeneratedPassword ?? "Admin@123", out string passwordHash, out string passwordSalt);
                param.Add("@PasswordHash", passwordHash);
                param.Add("@PasswordSalt", passwordSalt);
            }
            else
            {
                param.Add("@PasswordHash", model.PasswordHash ?? "");
                param.Add("@PasswordSalt", model.PasswordSalt ?? "");
            }
            param.Add("@CustomerId", model.CustomerId);
            param.Add("@FirstName", model.FirstName);
            param.Add("@LastName", model.LastName);
            param.Add("@Email", model.Email);
            param.Add("@CountryCode", model.CountryCode);
            param.Add("@ContactNumber", model.ContactNumber);
            param.Add("@CreatedBy", model.CreatedBy);
            var isSaved = Execute(StoredProcedures.SaveCustomer, param);
            return Task.FromResult(isSaved.Data);
        }

        public Task<long> SendCustomerWelcomeMail(CustomerWelcomeEmailRequestModel model)
        {
            var param = new DynamicParameters();
            param.Add("@CustomerId", model.CustomerId);
            param.Add("@EmailTemplate", model.EmailTemplate);
            var isSaved = Execute(StoredProcedures.UpdateCustomerWelcomeStatus, param);
            return Task.FromResult(isSaved.Data);
        }

        public Task<long> DeleteCustomer(long CustomerId)
        {
            var param = new DynamicParameters();
            param.Add("@CustomerId", CustomerId);
            var response = Execute(StoredProcedures.DeleteCustomer, param);
            return Task.FromResult(response.Data);
        }

        public Task<long> UpdateFCMToken(FCMTokenUpdateRequestModel model)
        {
            var param = new DynamicParameters();
            param.Add("@CustomerId", model.CustomerId);
            param.Add("@FCMToken", model.FCMToken);
            var response = Execute(StoredProcedures.UpdateFCMToken, param);
            return Task.FromResult(response.Data);
        }

        public Task<List<InactiveCartUserNotificationResponseModel>> GetInactiveCartCustomers()
        {
            var param = new DynamicParameters();
            param.Add("@Minutes",5);
            var customers = Query<InactiveCartUserNotificationResponseModel>(StoredProcedures.GetInactiveCartUsers, param);
            return Task.FromResult(customers.Data.ToList());
        }

        public Task<long> SaveNotification(NotificationSaveRequestModel model)
        {
            var param = new DynamicParameters();
            param.Add("@CartId", model.CartId);
            param.Add("@CustomerId", model.CustomerId);
            param.Add("@NotificationType", model.NotificationType);
            var response = Execute(StoredProcedures.InsertCartNotificationLog, param);
            return Task.FromResult(response.Data);
        }


        #endregion
    }
}
