using Dapper;
using DemoWebAPI.Common;
using DemoWebAPI.model.Models;
using GeckoAPI.Model.models;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace GeckoAPI.Repository.dashboard
{
    public class DashboardRepository:BaseRepository,IDashboardRepository   
    {
        #region Constructor
        public DashboardRepository(IOptions<DbConfig> config) : base(config)
        {
        }
        #endregion
        #region Methods
        public Task<Dashboard> GetDashboardCount()
        {
            var dashboard = QueryFirstOrDefault<Dashboard>(StoredProcedures.GetDashboardCounts);
            return Task.FromResult(dashboard.Data);
        }

        public Task<List<MonthlySalesResponseModel>> GetMonthlySalesStats(long year)
        {
            var param = new DynamicParameters();
            param.Add("@Year", year);
            var response = Query<MonthlySalesResponseModel>(StoredProcedures.MonhtlySalesAmount, param);
            return Task.FromResult(response.Data.ToList());
        }

        public Task<List<MostOrderedProductsResponseModel>> GetMostOrderedProductStats(long filter)
        {
            var param = new DynamicParameters();
            param.Add("@Filter", filter);
            var response = Query<MostOrderedProductsResponseModel>(StoredProcedures.GetMostOrderedProducts, param);
            return Task.FromResult(response.Data.ToList());
        }
        #endregion
    }
}
