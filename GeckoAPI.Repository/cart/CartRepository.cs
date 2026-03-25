using Dapper;
using DemoWebAPI.Common;
using DemoWebAPI.model.Models;
using GeckoAPI.Model.models;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeckoAPI.Repository.cart
{
    public class CartRepository:BaseRepository, ICartRepository
    {
        #region Constructor
        public CartRepository(IOptions<DbConfig> config) : base(config)
        {
        }
        #endregion
        #region Methods
        public Task<long> AddToCart(AddToCartSaveModel model)
        {
            var param = new DynamicParameters();
            param.Add("@SessionId", model.SessionId);
            param.Add("@CustomerId", model.CustomerId, DbType.Int32);
            param.Add("@ProductId", model.ProductId, DbType.Int32);
            param.Add("@Price", model.Price);
            param.Add("@Quantity", model.Quantity, DbType.Int32);

            var query = GetPgFunctionQuery(
                StoredProcedures.AddToCart,
                false,
                "@SessionId,@CustomerId,@ProductId,@Price,@Quantity"
            );

            var response = Execute(query, param);
            return Task.FromResult((long)response.Data);
        }

        public Task<List<CartItemDetails>> GetCartContents(string? sessionId, long? customerId)
        {
            var param = new DynamicParameters();
            param.Add("@SessionId", sessionId);
            param.Add("@CustomerId", customerId, DbType.Int32);

            var query = GetPgFunctionQuery(
                StoredProcedures.GetCartContents,
                true,
                "@SessionId,@CustomerId"
            );

            var response = Query<CartItemDetails>(query, param);
            return Task.FromResult(response.Data.ToList());
        }

        public Task<long> UpdateCartCustomerId(UpdateCartCutomerIdRequestModel model)
        {
            var param = new DynamicParameters();
            param.Add("@CartId", model.CartId, DbType.Int32);
            param.Add("@CustomerId", model.CustomerId, DbType.Int32);

            var query = GetPgFunctionQuery(
                StoredProcedures.UpdateCartCustomerId,
                false,
                "@CartId,@CustomerId"
            );

            var response = Execute(query, param);
            return Task.FromResult((long)response.Data);
        }

        public Task<long> UpdateCartItemQuantity(UpdateCartItemsSaveModel model)
        {
            var param = new DynamicParameters();
            param.Add("@SessionId", model.SessionId);
            param.Add("@CustomerId", model.CustomerId, DbType.Int32);
            param.Add("@ProductId", model.ProductId, DbType.Int32);
            param.Add("@NewQuantity", model.NewQuantity, DbType.Int32);

            var query = GetPgFunctionQuery(
                StoredProcedures.UpdateCartItemQuantity,
                false,
                "@SessionId,@CustomerId,@ProductId,@NewQuantity"
            );

            var response = Execute(query, param);
            return Task.FromResult((long)response.Data);
        }
        #endregion
    }
}
