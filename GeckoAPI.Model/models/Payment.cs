using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeckoAPI.Model.models
{
    public class Payment
    {
    }

    public class PaymentIntentRequestModel
    {
        public long Amount { get; set; }
    }
    public class SaveStripeWebhookEventRequest
    {
        public string StripeEventId { get; set; }

        public string EventType { get; set; }

        public string ApiVersion { get; set; }

        public string? PaymentIntentId { get; set; }

        public string? CheckoutSessionId { get; set; }

        public string? ChargeId { get; set; }

        public string PayloadJson { get; set; }
    }

    public class CreateSessionRequest
    {
        public string PriceId { get; set; }
        public long CustomerId { get; set; }
        public long PlanId { get; set; }
    }

    public class SaveCustomerSubscriptionRequestModel
    {
        public long CustomerId { get; set; }

        public long PlanId { get; set; }

        public string StripeSubscriptionId { get; set; } = string.Empty;

        public string SubscriptionStatus { get; set; } = string.Empty;

        public DateTime? CurrentPeriodStart { get; set; }

        public DateTime? CurrentPeriodEnd { get; set; }

        public bool CancelAtPeriodEnd { get; set; }

    }

    public class ChangePlanRequest
    {
        public long CustomerId { get; set; }
        public long PlanId { get; set; }
        public string PriceId { get; set; } // Stripe price id (null/empty if Free)
        public bool IsFree { get; set; }
        public string? CurrentStripeSubscriptionId { get; set; }
    }

    public class PlanCheckRequestModel {
        public long CustomerId { get; set; }
        public long PlanId { get; set; }
    }

    public class PlanCheckResponseModel
    {
        public long CustomerId { get; set; }
        public long CurrentPlanId { get; set; }
        public long RequestedPlanId { get; set; }
        public string SubscriptionStatus { get; set; }
        public DateTime CurrentPeriodEnd { get; set; }
        public string ActionType { get; set; }
    }

}
