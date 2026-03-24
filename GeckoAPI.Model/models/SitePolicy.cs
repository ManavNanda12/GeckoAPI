using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeckoAPI.Model.models
{
    public class SitePolicy
    {
        public long SitePolicyId { get; set; }
        public string? PolicyName { get; set; }
        public string PolicyDescription { get; set; }
        public long? UpdatedBy { get; set; }

    }
}
