using DemoWebAPI.model.Models;
using DemoWebAPI.Repository.User;
using GeckoAPI.Model.models;
using GeckoAPI.Repository.customer;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeckoAPI.Service.customer
{
    public class CustomerService:ICustomerService
    {

        #region Constructor
        private readonly ICustomerRepository _customerRepository;
        public CustomerService(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }
        #endregion

        #region Methods
        public async Task<Model.models.Customer> GetCustomerById(long CustomerId)
        {
            var customer = await _customerRepository.GetCustomerById(CustomerId);
            return customer;
        }

        public async Task<Model.models.Customer> GetCustomerByEmail(string CustomerEmail)
        {
            var result = await _customerRepository.GetCustomerByEmail(CustomerEmail);
            return result;
        }

        public async Task<long> AddCustomerToken(CustomerJWTModel model)
        {
            var result = await _customerRepository.AddCustomerToken(model);
            return result;
        }

        public async Task<List<CustomerListModel>> GetCustomerList(CommonListRequestModel model)
        {
           var result = await _customerRepository.GetCustomerList(model);
           return result;
        }

        public async Task<long> SaveCustomer(CustomerSaveModel model)
        {
            var result = await _customerRepository.SaveCustomer(model);
            return result;
        }

        public async Task<long> SendCustomerWelcomeMail(CustomerWelcomeEmailRequestModel model)
        {
            var result = await _customerRepository.SendCustomerWelcomeMail(model);
            return result;
        }

        public async Task<long> DeleteCustomer(long CustomerId)
        {
            var result = await _customerRepository.DeleteCustomer(CustomerId);
            return result;
        }

        public async Task<long> UpdateFCMToken(FCMTokenUpdateRequestModel model)
        {
            var result = await _customerRepository.UpdateFCMToken(model);
            return result;
        }

        public async Task<List<InactiveCartUserNotificationResponseModel>> GetInactiveCartCustomers()
        {
            var result = await _customerRepository.GetInactiveCartCustomers();
            return result;
        }

        public async Task<long> SaveNotification(NotificationSaveRequestModel model)
        {
            var result = await _customerRepository.SaveNotification(model);
            return result;
        }

        #endregion
    }
}
