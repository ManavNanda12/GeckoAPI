using Dapper;
using DemoWebAPI.Common;
using GeckoAPI.Model.models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeckoAPI.Repository.wishlist
{
    public class WishlistRepository:BaseRepository, IWishlistRepository
    {
        #region Constructor
        public WishlistRepository(Microsoft.Extensions.Options.IOptions<DemoWebAPI.model.Models.DbConfig> config) : base(config)
        {
        }
        #endregion

        #region Methods
        public async Task<long> AddToWishlist(Model.models.WishlistSaveRequestModel model)
        {
            var param = new DynamicParameters();
            param.Add("@CustomerId", model.CustomerId, DbType.Int32);
            param.Add("@ProductId", model.ProductId, DbType.Int32);
            var query = GetPgFunctionQuery(
              StoredProcedures.SaveWishlist,
              false,
              "@CustomerId, @ProductId"
            );

            var result = Execute(query, param);
            return result.Data;
        }

        public async Task<long> RemoveFromWishlist(Model.models.WishlistSaveRequestModel model)
        {
            var param = new DynamicParameters();
            param.Add("@CustomerId", model.CustomerId, DbType.Int64);
            param.Add("@ProductId", model.ProductId, DbType.Int64);
            var query = GetPgFunctionQuery(
              StoredProcedures.RemoveFromWhislist,
              false,
              "@CustomerId, @ProductId"
            );
            var result = Execute(query, param);
            return result.Data;
        }

        public Task<List<Products>> GetCustomerWishlist(long CustomerId)
        {
            var param = new DynamicParameters();
            param.Add("@CustomerId", CustomerId, DbType.Int32);
            var query = GetPgFunctionQuery(
              StoredProcedures.GetWishlistProductList,
              true,
              "@CustomerId"
            );
            var result = Query<Products>(query,param);
            return Task.FromResult(result.Data.ToList());
        }
        #endregion
    }
}
