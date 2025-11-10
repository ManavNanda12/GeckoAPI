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
        #endregion
    }
}
