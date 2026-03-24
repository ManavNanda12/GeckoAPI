using GeckoAPI.Model.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeckoAPI.Repository.sitepolicy
{
    public interface ISitePolicyRepository
    {
        Task<long> SaveSitePolicy(SitePolicy model);
        Task<List<SitePolicy>> GetSitePolicies();
    }
}
