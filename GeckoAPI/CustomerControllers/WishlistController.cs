using DemoWebAPI.model.Models;
using GeckoAPI.Model.models;
using GeckoAPI.Service.product;
using GeckoAPI.Service.wishlist;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GeckoAPI.CustomerControllers
{
    [Route("api/customer/wishlist")]
    [ApiController]
    public class WishlistController : ControllerBase
    {

        #region Fields
        private readonly IWishlistService _wishlistService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        #endregion

        #region Constructor
        public WishlistController(IWishlistService wishlistService, IHttpContextAccessor httpContextAccessor)
        {
            _wishlistService = wishlistService;
            _httpContextAccessor = httpContextAccessor;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Save Wishlist Item
        /// </summary>        
        [HttpPost("save-wishlist-item")]
        public async Task<BaseAPIResponse<long>> SaveWishlistItem(WishlistSaveRequestModel model)
        {
            var response = new BaseAPIResponse<long>();
            try
            {

                var isAdded = await _wishlistService.AddToWishlist(model);
                if(isAdded == -1)
                {
                    response.Success = false;
                    response.Message = "Item already in wishlist.";
                }
                else
                {
                    response.Success = true;
                    response.Message = "Item added to wishlist successfully.";
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"An error occurred: {ex.Message}";
            }
            return response;
        }

        /// <summary>
        /// Remove Wishlist Item
        /// </summary>        
        [HttpPost("remove-wishlist-item")]
        public async Task<BaseAPIResponse<long>> RemoveWishlistItem(WishlistSaveRequestModel model)
        {
            var response = new BaseAPIResponse<long>();
            try
            {

                var isRemoved = await _wishlistService.RemoveFromWishlist(model);
                if (isRemoved == 1)
                {
                    response.Success = true;
                    response.Message = "Item removed from wishlist successfully.";
                }
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"An error occurred: {ex.Message}";
            }
            return response;
        }

        /// <summary>
        /// Get wishlist products
        /// </summary>
        [HttpGet("get-customer-wishlist")]
        public async Task<BaseAPIResponse<List<Products>>> GetProductList()
        {
            var response = new BaseAPIResponse<List<Products>>();
            try
            {
                var baseUrl = GetBaseUrl();
                object userIdObject = HttpContext.Items["UserId"];
                long CustomerId = userIdObject != null ? Convert.ToInt64(userIdObject) : 0;
                var products = await _wishlistService.GetCustomerWishlist(CustomerId);
                var productList = products.Select(c => new Products
                {
                    ProductID = c.ProductID,
                    CategoryID = c.CategoryID,
                    CategoryName = c.CategoryName,
                    ProductName = c.ProductName,
                    ProductDescription = c.ProductDescription,
                    Price = c.Price,
                    TotalRecords = c.TotalRecords,
                    IsWishlistItem = c.IsWishlistItem,
                    SKU = c.SKU,
                    ProductImage = !string.IsNullOrEmpty(c.ProductImage)
            ? $"{baseUrl}/{c.ProductImage}"
            : null
                }).ToList();
                response.Data = productList;
                response.Success = true;
                response.Message = "Products fetched successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"An error occurred: {ex.Message}";
            }
            return response;
        }

        private string GetBaseUrl()
        {
            var request = _httpContextAccessor.HttpContext?.Request;
            if (request == null) return string.Empty;

            return $"{request.Scheme}://{request.Host}";
        }
        #endregion
    }
}
