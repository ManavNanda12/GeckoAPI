using Dapper;
using DemoWebAPI.Common;
using GeckoAPI.Model.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            var data = Query<SitePolicy>(StoredProcedures.GetPolicies);
            return Task.FromResult(data.Data.ToList());

        }

        public Task<long> SaveSitePolicy(SitePolicy model)
        {
            var param = new DynamicParameters();
            param.Add("@SitePolicyId", model.SitePolicyId);
            param.Add("@PolicyDescription", model.PolicyDescription);
            param.Add("@UpdatedBy", model.UpdatedBy);
            var result = Execute(StoredProcedures.UpdatePolicy, param);
            return Task.FromResult(result.Data);
        }
        #endregion
    }
}
