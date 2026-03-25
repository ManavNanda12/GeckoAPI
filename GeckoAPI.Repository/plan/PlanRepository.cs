using Dapper;
using DemoWebAPI.Common;
using DemoWebAPI.model.Models;
using GeckoAPI.Model.models;
using Microsoft.Extensions.Options;
using System.Data;

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
            param.Add("@CustomerId", customerId, DbType.Int32);

            var query = GetPgFunctionQuery(
                StoredProcedures.GetPlans,
                true,
                "@CustomerId"
            );

            var response = Query<PlanListResponseModel>(query, param);
            return Task.FromResult(response.Data.ToList());
        }

        public Task<List<PlanSubscriptionDetailModel>> GetPlanSubscriptionDetails(long PlanId)
        {
            var param = new DynamicParameters();
            param.Add("@PlanId", PlanId, DbType.Int32);

            var query = GetPgFunctionQuery(
                StoredProcedures.GetPlanSubscriptonHistory,
                true,
                "@PlanId"
            );

            var response = Query<PlanSubscriptionDetailModel>(query, param);
            return Task.FromResult(response.Data.ToList());
        }
        #endregion
    }
}
