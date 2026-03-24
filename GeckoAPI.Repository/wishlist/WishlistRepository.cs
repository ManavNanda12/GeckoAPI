using Dapper;
using DemoWebAPI.Common;
using GeckoAPI.Model.models;
using System;
using System.Collections.Generic;
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
            param.Add("@CustomerId", model.CustomerId);
            param.Add("@ProductId", model.ProductId);
            var result = Execute(StoredProcedures.SaveWishlist, param);
            return result.Data;
        }

        public async Task<long> RemoveFromWishlist(Model.models.WishlistSaveRequestModel model)
        {
            var param = new DynamicParameters();
            param.Add("@CustomerId", model.CustomerId);
            param.Add("@ProductId", model.ProductId);
            var result = Execute(StoredProcedures.RemoveFromWhislist, param);
            return result.Data;
        }

        public Task<List<Products>> GetCustomerWishlist(long CustomerId)
        {
            var param = new DynamicParameters();
            param.Add("@CustomerId", CustomerId);
            var result = Query<Products>(StoredProcedures.GetWishlistProductList,param);
            return Task.FromResult(result.Data.ToList());
        }
        #endregion
    }
}
