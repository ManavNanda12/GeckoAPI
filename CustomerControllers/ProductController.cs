using DemoWebAPI.model.Models;
using GeckoAPI.Model.models;
using GeckoAPI.Service.product;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GeckoAPI.CustomerControllers
{
    [Route("api/customer/product")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        #region Fields
        private readonly IProductService _productService;
        private readonly IWebHostEnvironment _environment;
        private readonly IHttpContextAccessor _httpContextAccessor;
        #endregion

        #region Constructor
        public ProductController(IProductService productService, IWebHostEnvironment environment, IHttpContextAccessor httpContextAccessor)
        {
            _productService = productService;
            _environment = environment;
            _httpContextAccessor = httpContextAccessor;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Get All Products
        /// </summary>
        [HttpGet("get-product-list/{CategoryId}")]
        public async Task<BaseAPIResponse<List<Products>>> GetProductList(long CategoryId)
        {
            var response = new BaseAPIResponse<List<Products>>();
            try
            {
                var products = await _productService.GetCustomerProductList(CategoryId);
                response.Data = products;
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
        #endregion
    }
}
