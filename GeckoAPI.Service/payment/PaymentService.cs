using GeckoAPI.Model.models;
using GeckoAPI.Repository.payment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeckoAPI.Service.payment
{
    public class PaymentService:IPaymentService
    {

        #region Fields
        public readonly IPaymentRepository _paymentRepository;
        #endregion

        #region Constructor
        public PaymentService(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }
        #endregion

        #region Methods
        public async Task<string> CreatePaymentIntent(PaymentIntentRequestModel model)
        {
            var data = await _paymentRepository.CreatePaymentIntent(model);
            return data;
        }

        public async Task<string> CreateSubscription(CreateSessionRequest model)
        {
            var data = await _paymentRepository.CreateSubscription(model);
            return data;
        }

        public async Task<long> SaveWebhookEvent(SaveStripeWebhookEventRequest model)
        {
            var data = await _paymentRepository.SaveWebhookEvent(model);
            return data;
        }

        public async Task<long> SaveSubscriptionEvent(SaveCustomerSubscriptionRequestModel model)
        {
            var data = await _paymentRepository.SaveSubscriptionEvent(model);
            return data;
        }

        public async Task<string> ChangePlanAsync(ChangePlanRequest model)
        {
            var data = await _paymentRepository.ChangePlanAsync(model);
            return data;
        }

        public async Task<PlanCheckResponseModel> CheckPlan(PlanCheckRequestModel model)
        {
            var data = await _paymentRepository.CheckPlan(model);
            return data;
        }

        #endregion
    }
}
