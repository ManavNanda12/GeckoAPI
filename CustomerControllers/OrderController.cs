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
        public async Task<BaseAPIResponse<long>> CheckoutOrder([FromBody] CheckoutOrderRequestModel model)
        {
            var response = new BaseAPIResponse<long>();
            try
            {
                var isOrdered = await _orderService.CheckoutOrder(model);
                if(isOrdered == -1)
                {
                    response.Success = false;
                    response.Message = "Cart is not valid.";
                    return response;
                }
                else if(isOrdered > 0)
                {
                    response.Success = true;
                    response.Message = "Order placed successfully.";
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
