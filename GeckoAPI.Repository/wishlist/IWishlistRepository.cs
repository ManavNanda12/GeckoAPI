using GeckoAPI.Model.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeckoAPI.Repository.wishlist
{
    public interface IWishlistRepository
    {
        Task<long> AddToWishlist(WishlistSaveRequestModel model);
        Task<long> RemoveFromWishlist(WishlistSaveRequestModel model);
        Task<List<Products>> GetCustomerWishlist(long CustomerId);
    }
}
