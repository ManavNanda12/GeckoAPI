using GeckoAPI.Model.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeckoAPI.Service.cart
{
    public interface ICartService
    {
        Task<long> AddToCart(AddToCartSaveModel model);
        Task<List<CartItemDetails>> GetCartContents(string? sessionId, long? userId);
        Task<long> UpdateCartItemQuantity(UpdateCartItemsSaveModel model);
        Task<long> UpdateCartCustomerId(UpdateCartCutomerIdRequestModel model);
    }
}
