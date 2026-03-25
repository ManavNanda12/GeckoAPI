using Dapper;
using DemoWebAPI.Common;
using GeckoAPI.Model.models;
using System.Data;

namespace GeckoAPI.Repository.sitepolicy
{
    public class SitePolicyRepository:BaseRepository, ISitePolicyRepository
    {
        #region Constructor
        public SitePolicyRepository(Microsoft.Extensions.Options.IOptions<DemoWebAPI.model.Models.DbConfig> config) : base(config)
        {
        }
        #endregion

        #region Methods
        public Task<List<SitePolicy>> GetSitePolicies()
        {
            var param = new DynamicParameters();

            var query = GetPgFunctionQuery(
                StoredProcedures.GetPolicies,
                true
            );

            var response = Query<SitePolicy>(query, param);
            return Task.FromResult(response.Data.ToList());
        }

        public Task<long> SaveSitePolicy(SitePolicy model)
        {
            var param = new DynamicParameters();
            param.Add("@SitePolicyId", model.SitePolicyId ,DbType.Int32);
            param.Add("@PolicyDescription", model.PolicyDescription);
            param.Add("@UpdatedBy", model.UpdatedBy, DbType.Int32);

            var query = GetPgFunctionQuery(
                StoredProcedures.UpdatePolicy,
                false,
                "@SitePolicyId,@PolicyDescription,@UpdatedBy"
            );

            var response = Execute(query, param);
            return Task.FromResult(response.Data);
        }
        #endregion
    }
}
