using GeckoAPI.Model.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeckoAPI.Service.customer
{
    public interface ICustomerService
    {
        Task<Customer> GetCustomerById(long CustomerId);
        Task<Customer> GetCustomerByEmail(string CustomerEmail);
        Task<long> AddCustomerToken(CustomerJWTModel model);
        Task<List<CustomerListModel>> GetCustomerList(CommonListRequestModel model);
        Task<long> SaveCustomer(CustomerSaveModel model);
        Task<long> SendCustomerWelcomeMail(CustomerWelcomeEmailRequestModel model);
        Task<long> DeleteCustomer(long CustomerId);
        Task<long> UpdateFCMToken(FCMTokenUpdateRequestModel model);
        Task<List<InactiveCartUserNotificationResponseModel>> GetInactiveCartCustomers();
        Task<long> SaveNotification(NotificationSaveRequestModel model);
    }
}
