using DemoWebAPI.model.Models;
using GeckoAPI.Model.models;
using GeckoAPI.Service.order;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GeckoAPI.CustomerControllers
{
    [Route("api/customer/order")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        #region Fields
        private readonly IOrderService _orderService;
        #endregion

        #region Constructor
        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
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
                var orderdetail = await _orderService.GetOrderDetail(OrderId);
                response.Data = orderdetail;
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
        #endregion
    }
}
