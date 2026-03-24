using GeckoAPI.Model.models;
using GeckoAPI.Repository.sitepolicy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeckoAPI.Service.sitepolicy
{
    public class SitePolicyService : ISitePolicyService
    {
        #region Fields
        private readonly ISitePolicyRepository _sitePolicyRepository;
        #endregion

        #region Constructor
        public SitePolicyService(ISitePolicyRepository sitePolicyRepository)
        {
            _sitePolicyRepository = sitePolicyRepository;
        }
        #endregion
        #region Methods
        public Task<List<SitePolicy>> GetSitePolicies()
        {
            var data = _sitePolicyRepository.GetSitePolicies();
            return data;
        }

        public Task<long> SaveSitePolicy(SitePolicy model)
        {
            var data = _sitePolicyRepository.SaveSitePolicy(model);
            return data;
        }
        #endregion
    }
}
