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
        #endregion
    }
}
