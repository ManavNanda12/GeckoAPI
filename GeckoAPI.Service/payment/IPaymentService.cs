using GeckoAPI.Model.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeckoAPI.Service.payment
{
    public interface IPaymentService
    {
        Task<string> CreatePaymentIntent(PaymentIntentRequestModel model);
        Task<long> SaveWebhookEvent(SaveStripeWebhookEventRequest model);
        Task<string> CreateSubscription(CreateSessionRequest model);
        Task<long> SaveSubscriptionEvent(SaveCustomerSubscriptionRequestModel model);
        Task<string> ChangePlanAsync(ChangePlanRequest model);
        Task<PlanCheckResponseModel> CheckPlan(PlanCheckRequestModel model);
    }
}
