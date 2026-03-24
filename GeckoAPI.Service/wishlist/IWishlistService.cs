using GeckoAPI.Model.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeckoAPI.Service.wishlist
{
    public interface IWishlistService
    {
        Task<long> AddToWishlist(WishlistSaveRequestModel model);
        Task<long> RemoveFromWishlist(WishlistSaveRequestModel model);
        Task<List<Products>> GetCustomerWishlist(long CustomerId);
    }
}
