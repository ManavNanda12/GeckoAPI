using GeckoAPI.Model.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeckoAPI.Repository.payment
{
    public interface IPaymentRepository
    {
        Task<string> CreatePaymentIntent(PaymentIntentRequestModel model);
        Task<long> SaveWebhookEvent(SaveStripeWebhookEventRequest model);
        Task<string> CreateSubscription(CreateSessionRequest model);
        Task<long> SaveSubscriptionEvent(SaveCustomerSubscriptionRequestModel model);
        Task<string> ChangePlanAsync(ChangePlanRequest model);
        Task<PlanCheckResponseModel> CheckPlan(PlanCheckRequestModel model);
        Task UpgradeSubscription(string subscriptionId, string newPriceId, long customerId, long newPlanId);
        Task ScheduleDowngrade(string subscriptionId, string newPriceId, bool requestedIsFree, long customerId, long newPlanId);
    }
}
