using GeckoAPI.Model.models;
using GeckoAPI.Repository.dashboard;
using GeckoAPI.Repository.wishlist;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeckoAPI.Service.wishlist
{
    public class WishlistService:IWishlistService
    {
        #region Fields
        public readonly IWishlistRepository _wishlistRepository;
        #endregion
        #region Constructor
        public WishlistService(IWishlistRepository wishlistRepository)
        {
            _wishlistRepository = wishlistRepository;
        }
        #endregion
        #region Methods
        public Task<long> RemoveFromWishlist(WishlistSaveRequestModel model)
        {
            var data = _wishlistRepository.RemoveFromWishlist(model);
            return data;
        }
        public Task<long> AddToWishlist(WishlistSaveRequestModel model)
        {
            var data = _wishlistRepository.AddToWishlist(model);
            return data;
        }

        public Task<List<Products>> GetCustomerWishlist(long CustomerId)
        {
            var data = _wishlistRepository.GetCustomerWishlist(CustomerId);
            return data;
        }
        #endregion
    }
}
