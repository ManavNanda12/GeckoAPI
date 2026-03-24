using Dapper;
using DemoWebAPI.Common;
using DemoWebAPI.model.Models;
using GeckoAPI.Model.models;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
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
            param.Add("@CustomerId", model.CustomerId);
            param.Add("@ProductId", model.ProductId);
            param.Add("@Price", model.Price);
            param.Add("@Quantity", model.Quantity);
            var response = Execute(StoredProcedures.AddToCart, param);
            return Task.FromResult((long)response.Data);

        }

        public Task<List<CartItemDetails>> GetCartContents(string? sessionId, long? customerId)
        {
            var param = new DynamicParameters();
            param.Add("@SessionId", sessionId);
            param.Add("@CustomerId", customerId);
            var cartItems = Query<CartItemDetails>(StoredProcedures.GetCartContents, param);
            return Task.FromResult(cartItems.Data.ToList());
        }

        public Task<long> UpdateCartCustomerId(UpdateCartCutomerIdRequestModel model)
        {
            var param = new DynamicParameters();
            param.Add("@CartId", model.CartId);
            param.Add("@CustomerId", model.CustomerId);
            var response = Execute(StoredProcedures.UpdateCartCustomerId, param);
            return Task.FromResult((long)response.Data);
        }

        public Task<long> UpdateCartItemQuantity(UpdateCartItemsSaveModel model)
        {
           var param = new DynamicParameters();
            param.Add("@SessionId", model.SessionId);
            param.Add("@CustomerId", model.CustomerId);
            param.Add("@ProductId", model.ProductId);
            param.Add("@NewQuantity", model.NewQuantity);
            var response = Execute(StoredProcedures.UpdateCartItemQuantity, param);
            return Task.FromResult((long)response.Data);
        }
        #endregion
    }
}
