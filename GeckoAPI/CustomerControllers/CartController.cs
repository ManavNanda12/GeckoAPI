using DemoWebAPI.model.Models;
using GeckoAPI.Model.models;
using GeckoAPI.Service.cart;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GeckoAPI.CustomerControllers
{
    [Route("api/customer/cart")]
    [ApiController]
    public class CartController : ControllerBase
    {
        #region Fields
        private readonly ICartService _cartService;
        private readonly IWebHostEnvironment _environment;
        private readonly IHttpContextAccessor _httpContextAccessor;
        #endregion
        #region Constructor
        public CartController(ICartService cartService, IWebHostEnvironment environment, IHttpContextAccessor httpContextAccessor)
        {
            _cartService = cartService;
            _environment = environment;
            _httpContextAccessor = httpContextAccessor;
        }
        #endregion
        #region Methods
        /// <summary>
        /// Add To Cart
        /// </summary>
        [HttpPost("add-to-cart")]
        public async Task<BaseAPIResponse<long>> AddToCart(AddToCartSaveModel model)
        {
            var response = new BaseAPIResponse<long>();
            try
            {
                var isSaved = await _cartService.AddToCart(model);
                if(isSaved == 0)
                {
                    response.Data = isSaved;
                    response.Success = false;
                    response.Message = "Failed to add item to cart.";
                    return response;
                }
                response.Data = isSaved;
                response.Success = true;
                response.Message = "Item added to cart successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"An error occurred: {ex.Message}";
            }
            return response;
        }
        /// <summary>
        /// Get Cart Contents
        /// </summary>
        [HttpPost("get-cart-details")]
        public async Task<BaseAPIResponse<List<CartItemDetails>>> GetCartContents(Cart model)
        {
            var response = new BaseAPIResponse<List<CartItemDetails>>();
            try
            {
                var cartContents = await _cartService.GetCartContents(model.SessionId, model.CustomerId);
                response.Data = cartContents;
                response.Success = true;
                response.Message = "Cart contents fetched successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"An error occurred: {ex.Message}";
            }
            return response;
        }
        /// <summary>
        /// Update Cart Item Quantity
        /// </summary>
        [HttpPost("update-cart-item-quantity")]
        public async Task<BaseAPIResponse<long>> UpdateCartItemQuantity(UpdateCartItemsSaveModel model)
        {
            var response = new BaseAPIResponse<long>();
            try
            {
                var isUpdated = await _cartService.UpdateCartItemQuantity(model);
                if(isUpdated < 1)
                {
                    response.Success = false;
                    response.Message = "Failed to update cart item quantity.";
                    return response;
                }

                response.Success = true;
                response.Message = "Cart item quantity updated successfully.";
            }
            catch (
            Exception ex)
            {
                response.Success = false;
                response.Message = $"An error occurred: {ex.Message}";
            }
            return response;
        }

        /// <summary>
        /// Update Cart Customer Id
        /// </summary>
        [HttpPost("update-cart-customer-id")]
        public async Task<BaseAPIResponse<long>> UpdateCartCustomerId(UpdateCartCutomerIdRequestModel model)
        {
            var response = new BaseAPIResponse<long>();
            try
            {
                var isUpdated = await _cartService.UpdateCartCustomerId(model);
                if (isUpdated < 1)
                {
                    response.Success = false;
                    response.Message = "Invalid cart.";
                    return response;
                }

                response.Success = true;
                response.Message = "Cart customer id updated successfully.";
            }
            catch (
            Exception ex)
            {
                response.Success = false;
                response.Message = $"An error occurred: {ex.Message}";
            }
            return response;
        }
        #endregion
    }
}
