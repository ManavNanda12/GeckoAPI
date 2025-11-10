using DemoWebAPI.model.Models;
using GeckoAPI.Model.models;
using GeckoAPI.Service.product;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Buffers.Text;

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
                var baseUrl = GetBaseUrl();
                var products = await _productService.GetCustomerProductList(CategoryId);
                var productList = products.Select(c => new Products
                {
                    ProductID = c.ProductID,
                    CategoryID = c.CategoryID,
                    CategoryName = c.CategoryName,
                    ProductName = c.ProductName,
                    ProductDescription = c.ProductDescription,
                    Price = c.Price,
                    TotalRecords = c.TotalRecords,
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

        /// <summary>
        /// Get product images
        /// </summary>
        [HttpGet("get-product-images/{ProductId}")]
        public async Task<BaseAPIResponse<List<ProductImagesResponseModel>>> GetProductImages(long ProductId)
        {
            var response = new BaseAPIResponse<List<ProductImagesResponseModel>>();
            try
            {
                var products = await _productService.GetProductImages(ProductId);
                var baseUrl = GetBaseUrl();
                var productImageList = products.Select(c => new ProductImagesResponseModel
                {
                    ImageID = c.ImageID,
                    ProductId = c.ProductId,
                    IsPrimary = c.IsPrimary,
                    ImageUrl = !string.IsNullOrEmpty(c.ImageUrl)
                 ? $"{baseUrl}/{c.ImageUrl}"
                 : null
                }).ToList();
                response.Data = productImageList;
                response.Success = true;
                response.Message = "Product images fetched successfully.";
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
