using GeckoAPI.Model.models;
using GeckoAPI.Repository.cart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeckoAPI.Service.cart
{
    public class CartService:ICartService
    {
        #region Fields
        private readonly ICartRepository _cartRepository;
        #endregion

        #region Constructor
        public CartService(ICartRepository cartRepository)
        {
            _cartRepository = cartRepository;
        }
        #endregion

        #region Methods
        public Task<long> AddToCart(AddToCartSaveModel model)
        {
            var result = _cartRepository.AddToCart(model);
            return result;
        }

        public Task<List<CartItemDetails>> GetCartContents(string? sessionId, long? userId)
        {
           var result = _cartRepository.GetCartContents(sessionId, userId);
            return result;
        }

        public Task<long> UpdateCartCustomerId(UpdateCartCutomerIdRequestModel model)
        {
            var result = _cartRepository.UpdateCartCustomerId(model);
            return result;
        }

        public Task<long> UpdateCartItemQuantity(UpdateCartItemsSaveModel model)
        {
            var result = _cartRepository.UpdateCartItemQuantity(model);
            return result;
        }
        #endregion


    }
}
