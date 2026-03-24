using Dapper;
using DemoWebAPI.Common;
using DemoWebAPI.model.Models;
using GeckoAPI.Model.models;
using Microsoft.Extensions.Options;

namespace GeckoAPI.Repository.plan
{
    public class PlanRepository:BaseRepository,IPlanRepository
    {
        #region Constsructor
        public PlanRepository(IOptions<DbConfig> config) : base(config)
        {
        }
        #endregion

        #region Methods
        public Task<List<PlanListResponseModel>> GetPlanList(long customerId)
        {
            var param = new DynamicParameters();
            param.Add("@CustomerId", customerId);
            var response = Query<PlanListResponseModel>(StoredProcedures.GetPlans, param);
            return Task.FromResult(response.Data.ToList());
        }

        public Task<List<PlanSubscriptionDetailModel>> GetPlanSubscriptionDetails(long PlanId)
        {
            var param = new DynamicParameters();
            param.Add("@PlanId", PlanId);
            var response = Query<PlanSubscriptionDetailModel>(StoredProcedures.GetPlanSubscriptonHistory, param);
            return Task.FromResult(response.Data.ToList());
        }
        #endregion
    }
}
