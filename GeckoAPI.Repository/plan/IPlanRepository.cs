using GeckoAPI.Model.models;

namespace GeckoAPI.Repository.plan
{
    public interface IPlanRepository
    {
        Task<List<PlanListResponseModel>> GetPlanList(long customerId);
        Task<List<PlanSubscriptionDetailModel>> GetPlanSubscriptionDetails(long PlanId);
    }
}
