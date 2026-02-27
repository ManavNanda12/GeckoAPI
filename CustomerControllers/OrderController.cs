using DemoWebAPI.model.Models;
using GeckoAPI.Model.models;
using GeckoAPI.Service.coupon;
using GeckoAPI.Service.order;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Buffers.Text;

namespace GeckoAPI.CustomerControllers
{
    [Route("api/customer/order")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        #region Fields
        private readonly IOrderService _orderService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICouponService _couponService;
        #endregion

        #region Constructor
        public OrderController(IOrderService orderService, IHttpContextAccessor httpContextAccessor , ICouponService couponService)
        {
            _orderService = orderService;
            _httpContextAccessor = httpContextAccessor;
            _couponService = couponService;
        }
        #endregion
        #region Methods
        [HttpPost("checkout")]
        public async Task<BaseAPIResponse<CheckoutResponseModel>> CheckoutOrder([FromBody] CheckoutOrderRequestModel model)
        {
            var response = new BaseAPIResponse<CheckoutResponseModel>();
            try
            {
                var checkoutResult = await _orderService.CheckoutOrder(model);

                if (checkoutResult.Result == -1)
                {
                    // Cart not found or already processed
                    response.Success = false;
                    response.Message = checkoutResult.Message;
                }
                else if (checkoutResult.Result == -2)
                {
                    // Insufficient stock
                    response.Success = false;
                    response.Message = checkoutResult.Message;
                    response.Data = new CheckoutResponseModel
                    {
                        OutOfStockItems = checkoutResult.OutOfStockItems
                    };
                }
                else if (checkoutResult.Result == 1)
                {
                    // Success
                    response.Success = true;
                    response.Message = checkoutResult.Message;
                    response.Data = new CheckoutResponseModel
                    {
                        OrderId = checkoutResult.OrderId,
                        OrderNumber = checkoutResult.OrderNumber,
                        Total = checkoutResult.Total
                    };
                }

                return response;
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"An error occurred: {ex.Message}";
            }
            return response;
        }

        // Response model for API
        public class CheckoutResponseModel
        {
            public long OrderId { get; set; }
            public string OrderNumber { get; set; }
            public decimal Total { get; set; }
            public List<OutOfStockItem> OutOfStockItems { get; set; }
        }


        /// <summary>
        /// Get Order List
        /// </summary>
        [HttpGet("get-order-list/{CustomerId}")]
        public async Task<BaseAPIResponse<List<OrderListResponse>>> GetOrderList(long CustomerId)
        {
            var response = new BaseAPIResponse<List<OrderListResponse>>();
            try
            {
                var orders = await _orderService.GetOrderList(CustomerId);
                response.Data = orders;
                response.Success = true;
                response.Message = "Orders fetched successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"An error occurred: {ex.Message}";
            }
            return response;
        }

        /// <summary>
        /// Get Order Detail
        /// </summary>
        [HttpGet("get-order-detail/{OrderId}")]
        public async Task<BaseAPIResponse<List<OrderDetailResponse>>> GetOrderDetail(long OrderId)
        {
            var response = new BaseAPIResponse<List<OrderDetailResponse>>();
            try
            {
                var baseUrl = this.GetBaseUrl();
                var orderdetail = await _orderService.GetOrderDetail(OrderId);
                var updatedOrderDetail = orderdetail.Select(c => new OrderDetailResponse
                {
                    ProductId = c.ProductId,
                    LineTotal = c.LineTotal,
                    Discount = c.Discount,
                    ProductName = c.ProductName,
                    ProductDescription = c.ProductDescription,
                    DiscountAmount = c.DiscountAmount,
                    CouponCode = c.CouponCode,
                    PaymentStatus = c.PaymentStatus,
                    PaymentMethod = c.PaymentMethod,
                    StripePaymentStatus = c.StripePaymentStatus,
                    CreatedAt = c.CreatedAt,
                    OrderStatus = c.OrderStatus,
                    Id = c.Id,
                    Quantity = c.Quantity,
                    TaxAmount = c.TaxAmount,
                    ShippingAmount = c.ShippingAmount,
                    Total = c.Total,
                    UnitPrice = c.UnitPrice,
                    ImageUrl = !string.IsNullOrEmpty(c.ImageUrl)
            ? $"{baseUrl}/{c.ImageUrl}"
            : null
                }).ToList();
                response.Data = updatedOrderDetail;
                response.Success = true;
                response.Message = "Orders details fetched successfully.";
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

        /// <summary>
        /// Apply coupon
        /// </summary>        
        [HttpPost("apply-coupon")]
        public async Task<BaseAPIResponse<ApplyCouponResult>> ApplyCoupon(ApplyCouponRequestModel model)
        {
            var response = new BaseAPIResponse<ApplyCouponResult>();
            try
            {

                var isCouponApplied = await _couponService.ApplyCoupon(model);
                response.Data = isCouponApplied;
                if (isCouponApplied.StatusCode <= 0)
                {
                    response.Success = false;
                    response.Message = isCouponApplied.Message;
                }
                else
                {
                    response.Success = true;
                    response.Message = isCouponApplied.Message;
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
        /// Remove Coupon
        /// </summary>        
        [HttpPost("remove-coupon/{CartSessionId}")]
        public async Task<BaseAPIResponse<long>> RemoveCoupon(string CartSessionId)
        {
            var response = new BaseAPIResponse<long>();
            try
            {

                var isCouponRemoved = await _couponService.RemoveCoupon(CartSessionId);
                if (isCouponRemoved <= 0)
                {
                    response.Success = false;
                    response.Message = "Cart not found or already checked out";
                }
                else
                {
                    response.Success = true;
                    response.Message = "Coupon removed successfully";
                }

            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"An error occurred: {ex.Message}";
            }
            return response;
        }
        #endregion
    }
}
