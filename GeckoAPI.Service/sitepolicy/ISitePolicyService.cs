using GeckoAPI.Model.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeckoAPI.Service.sitepolicy
{
    public interface ISitePolicyService
    {
        Task<long> SaveSitePolicy(SitePolicy model);
        Task<List<SitePolicy>> GetSitePolicies();
    }
}
