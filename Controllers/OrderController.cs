using DemoWebAPI.model.Models;
using GeckoAPI.Model.models;
using GeckoAPI.Service.customer;
using GeckoAPI.Service.order;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GeckoAPI.Controllers
{
    [Route("api/order")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        #region Fields
        private readonly IOrderService _orderService;
        private readonly IWebHostEnvironment _environment;
        private readonly IHttpContextAccessor _httpContextAccessor;
        #endregion

        #region Constructor
        public OrderController(IOrderService orderService, IWebHostEnvironment environment, IHttpContextAccessor httpContextAccessor)
        {
            _orderService = orderService;
            _environment = environment;
            _httpContextAccessor = httpContextAccessor;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Get All Order
        /// </summary>
        [HttpPost("get-order-list")]
        public async Task<BaseAPIResponse<List<AdminOrderList>>> GetOrderList(CommonListRequestModel model)
        {
            var response = new BaseAPIResponse<List<AdminOrderList>>();
            try
            {
                var customers = await _orderService.GetAdminOrderList(model);
                response.Data = customers;
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
                    PaymentStatus = c.PaymentStatus,
                    PaymentMethod = c.PaymentMethod,
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

        #endregion
    }
}
