using GeckoAPI.Model.models;

namespace GeckoAPI.Service.plan
{
    public interface IPlanService
    {
        Task<List<PlanListResponseModel>> GetPlanList(long customerId);
        Task<List<PlanSubscriptionDetailModel>> GetPlanSubscriptionDetails(long PlanId);
    }
}
