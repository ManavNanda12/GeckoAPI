using GeckoAPI.Model.models;
using GeckoAPI.Repository.plan;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeckoAPI.Service.plan
{
    public class PlanService : IPlanService
    {
        #region Fields
        public readonly IPlanRepository _planRepository;
        #endregion

        #region Constructor
        public PlanService(IPlanRepository planRepository)
        {
            _planRepository = planRepository;
        }
        #endregion

        #region Methods
        public Task<List<PlanListResponseModel>> GetPlanList(long customerId)
        {
            var data = _planRepository.GetPlanList(customerId);
            return data;
        }

        public Task<List<PlanSubscriptionDetailModel>> GetPlanSubscriptionDetails(long PlanId)
        {
            var data = _planRepository.GetPlanSubscriptionDetails(PlanId);
            return data;
        }
        #endregion
    }
}
