using DemoWebAPI.model.Models;
using GeckoAPI.Common;
using GeckoAPI.Model.models;
using GeckoAPI.Service.product;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GeckoAPI.Controllers
{
    [Route("api/product")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        #region Fields
        private readonly IProductService _productService;
        private readonly IWebHostEnvironment _environment;
        private readonly IHttpContextAccessor _httpContextAccessor;
        #endregion

        #region Constructor
        public ProductController(IProductService productService, IWebHostEnvironment environment , IHttpContextAccessor httpContextAccessor)
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
        [HttpPost("get-product-list")]
        public async Task<BaseAPIResponse<List<Products>>> GetProductList(CommonListRequestModel model)
        {
            var response = new BaseAPIResponse<List<Products>>();
            try
            {
                var products = await _productService.GetProductList(model);
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

        /// <summary>
        /// Save Product
        /// </summary>
        [HttpPost("save-product")]
        public async Task<BaseAPIResponse<long>> SaveProduct(ProductSaveRequestModel model)
        {
            var response = new BaseAPIResponse<long>();
            try
            {
                var isAdded = await _productService.SaveProduct(model);
                response.Success = true;
                if (isAdded > 0)
                {
                    response.Message = "Product added successfully.";
                }
                else if (isAdded == 0)
                {
                    response.Message = "Product upated successfully.";
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
        /// Get product stock detail
        /// </summary>
        [HttpGet("get-stock-detail/{Id}")]
        public async Task<BaseAPIResponse<ProductStock>> GetProductStockDetails(long Id)
        {
            var response = new BaseAPIResponse<ProductStock>();
            try
            {
                var products = await _productService.GetProductStockDetails(Id);
                response.Data = products;
                response.Success = true;
                response.Message = "Product stock details fetched successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"An error occurred: {ex.Message}";
            }
            return response;
        }

        /// <summary>
        /// Save Product stock details
        /// </summary>
        [HttpPost("save-stock-detail")]
        public async Task<BaseAPIResponse<long>> UpdateProductStockDetails(ProductStock model)
        {
            var response = new BaseAPIResponse<long>();
            try
            {
                model.UpdatedBy = (int?)HttpContext.Items["UserId"];
                var isAdded = await _productService.UpdateProductStockDetails(model);
                response.Success = true;
                if(isAdded == -1)
                {
                    response.Success = false;
                    response.Message = "Please provide valid product quantity";
                    return response;
                }
                if(isAdded == -2)
                {
                    response.Success = false;
                    response.Message = "Product stock already exists. Please provide valid details";
                    return response;
                }
                if (isAdded > 0)
                {
                    response.Message = "Product added successfully.";
                }
                else if (isAdded == 0)
                {
                    response.Message = "Product upated successfully.";
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
        /// Save Product Image
        /// </summary>        
        [HttpPost("save-product-image")]
        public async Task<BaseAPIResponse<long>> SaveCategoryImage([FromForm] SaveProductImageModel model)
        {
            var response = new BaseAPIResponse<long>();
            try
            {
                if (CommonHelper.IsValidImageFile(model.ImageFile) == false)
                {
                    response.Success = false;
                    response.Message = "Please upload a valid image file.";
                    return response;
                }
                string productImagesPath = Path.Combine(_environment.WebRootPath, "ProductImages");
                if (!Directory.Exists(productImagesPath))
                {
                    Directory.CreateDirectory(productImagesPath);
                }
                // Generate GUID for filename
                string fileExtension = Path.GetExtension(model.ImageFile.FileName);
                string guidFileName = $"{Guid.NewGuid()}{fileExtension}";
                string filePath = Path.Combine(productImagesPath, guidFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await model.ImageFile.CopyToAsync(fileStream);
                }
                model.ImageUrl = $"ProductImages/{guidFileName}";

                var result = await _productService.SaveProductImage(model);
                response.Success = true;
                if (result > 0)
                {
                    response.Message = "Product image added successfully";
                }
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
