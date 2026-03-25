using Dapper;
using DemoWebAPI.Common;
using DemoWebAPI.model.Models;
using GeckoAPI.Model.models;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
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
            var param = new DynamicParameters();

            var query = GetPgFunctionQuery(
                StoredProcedures.GetDashboardCounts,
                true
            );

            var response = QueryFirstOrDefault<Dashboard>(query, param);
            return Task.FromResult(response.Data);
        }

        public Task<List<MonthlySalesResponseModel>> GetMonthlySalesStats(long year)
        {
            var param = new DynamicParameters();
            param.Add("@Year", year,DbType.Int32);

            var query = GetPgFunctionQuery(
                StoredProcedures.MonhtlySalesAmount,
                true,
                "@Year"
            );

            var response = Query<MonthlySalesResponseModel>(query, param);
            return Task.FromResult(response.Data.ToList());
        }

        public Task<List<MostOrderedProductsResponseModel>> GetMostOrderedProductStats(long filter)
        {
            var param = new DynamicParameters();
            param.Add("@Filter", filter, DbType.Int32);

            var query = GetPgFunctionQuery(
                StoredProcedures.GetMostOrderedProducts,
                true,
                "@Filter"
            );

            var response = Query<MostOrderedProductsResponseModel>(query, param);
            return Task.FromResult(response.Data.ToList());
        }
        #endregion
    }
}
