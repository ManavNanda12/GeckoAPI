using System.Globalization;

namespace GeckoAPI.Model.models
{
    public class Plan
    {
    }

    public class PlanListResponseModel
    {
        public int PlanId { get; set; }
        public string PlanName { get; set; }
        public string PlanDescription { get; set; }
        public string StripeProductId { get; set; }
        public string StripePriceId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string BillingInterval { get; set; }
        public string Benefits { get; set; }
        public int? ActivePlanId { get; set; }
        public bool IsCurrentPlan { get; set; }
        public string CurrentStripeSubscriptionId { get; set; }
        public DateTime CurrentPeriodStart { get; set; }
        public DateTime CurrentPeriodEnd { get; set; }
        public bool CancelAtPeriodEnd { get; set; }
    }

    public class PlanSubscriptionDetailModel
    {
        public string PlanName { get; set; }
        public string PlanDescription { get; set; }
        public DateTime CurrentPeriodStart { get; set; }
        public DateTime CurrentPeriodEnd { get; set; }
        public string SubscriptionStatus { get; set; }
        public bool CancelAtPeriodEnd { get; set; }
        public DateTime CreatedAt { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Benefits { get; set; }
        public string Currency { get; set; }
        public decimal Amount { get; set; }
    }

    public class ChatRequest
    {
        public string Message { get; set; }
        public long CustomerId { get; set; }
        public List<ChatHistoryItem> History { get; set; } = new();
        public string? CartId { get; set; }
    }

    public class ChatHistoryItem
    {
        public string Role { get; set; }    // "user" or "assistant"
        public string Content { get; set; }
    }

}
