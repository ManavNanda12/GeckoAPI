using GeckoAPI.Model.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeckoAPI.Repository.dashboard
{
    public interface IDashboardRepository
    {
        Task<Dashboard> GetDashboardCount();
        Task<List<MonthlySalesResponseModel>> GetMonthlySalesStats(long year);
        Task<List<MostOrderedProductsResponseModel>> GetMostOrderedProductStats(long filter);
    }
}
