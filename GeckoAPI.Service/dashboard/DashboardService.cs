using GeckoAPI.Model.models;
using GeckoAPI.Repository.dashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeckoAPI.Service.dashboard
{
    public class DashboardService: IDashboardService
    {

        #region Fields
        public readonly IDashboardRepository _dashboardRepository;
        #endregion
        #region Constructor
        public DashboardService(IDashboardRepository dashboardRepository) {
            _dashboardRepository = dashboardRepository;
        }
        #endregion
        #region Methods
        public Task<Dashboard> GetDashboardCount()
        {
           var data = _dashboardRepository.GetDashboardCount();
           return data;
        }

        public Task<List<MonthlySalesResponseModel>> GetMonthlySalesStats(long year)
        {
            var data = _dashboardRepository.GetMonthlySalesStats(year);
            return data;
        }

        public Task<List<MostOrderedProductsResponseModel>> GetMostOrderedProductStats(long filter)
        {
            var data = _dashboardRepository.GetMostOrderedProductStats(filter);
            return data;
        }
        #endregion
    }
}
