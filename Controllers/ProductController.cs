using DemoWebAPI.model.Models;
using GeckoAPI.Common;
using GeckoAPI.Model.models;
using GeckoAPI.Service.product;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Buffers.Text;

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
                var baseUrl = GetBaseUrl();
                var products = await _productService.GetProductList(model);
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

        [HttpPost("save-product-image")]
        public async Task<BaseAPIResponse<long>> SaveProductImage([FromForm] SaveProductImageModel model)
        {
            var response = new BaseAPIResponse<long>();
            try
            {
                // Validation
                if (model.ImageFile == null || model.ImageFile.Count == 0)
                {
                    response.Success = false;
                    response.Message = "Please upload at least one image.";
                    return response;
                }

                var uploadedImages = await  _productService.GetProductImages(model.ProductId);
                if (model.ImageFile.Count > 5 || uploadedImages.Count == 5)
                {
                    response.Success = false;
                    response.Message = "You can upload a maximum of 5 images per product.";
                    return response;
                }

                // Get current user
                model.CreatedBy = (int?)HttpContext.Items["UserId"] ?? 1;

                // If updating, delete old images first
                //if (model.ProductId > 0)
                //{
                //    var existingImages = await _productService.GetProductImages(model.ProductId);
                //    foreach (var img in existingImages)
                //    {
                //        var fullPath = Path.Combine(_environment.WebRootPath, img.ImageUrl.Replace("/", Path.DirectorySeparatorChar.ToString()));
                //        if (System.IO.File.Exists(fullPath))
                //        {
                //            System.IO.File.Delete(fullPath);
                //        }
                //    }
                //}

                // Create ProductImages directory if it doesn't exist
                string productImagesPath = Path.Combine(_environment.WebRootPath, "ProductImages");
                if (!Directory.Exists(productImagesPath))
                {
                    Directory.CreateDirectory(productImagesPath);
                }

                long lastSavedId = 0;

                // 🔁 Save each image one by one
                foreach (var file in model.ImageFile)
                {
                    // Generate GUID for filename
                    string fileExtension = Path.GetExtension(file.FileName);
                    string guidFileName = $"{Guid.NewGuid()}{fileExtension}";
                    string filePath = Path.Combine(productImagesPath, guidFileName);

                    // Save file to disk
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }

                    // Create single image model with URL
                    var singleImageModel = new SaveProductImageModel
                    {
                        ProductId = model.ProductId,
                        ImageUrl = $"ProductImages/{guidFileName}", // Store the URL path
                        IsPrimary = model.IsPrimary ?? false,
                        CreatedBy = model.CreatedBy
                    };

                    // Call service for each image
                    var result = await _productService.SaveProductImage(singleImageModel);
                    lastSavedId = result;
                }

                response.Success = true;
                response.Message = "Product images saved successfully.";
                response.Data = lastSavedId;
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

        /// <summary>
        /// Delete Product Image
        /// </summary>
        [HttpPost("delete-product-image")]
        public async Task<BaseAPIResponse<long>> DeleteProductImage(ProductDeleteRequestModel model)
        {
            var response = new BaseAPIResponse<long>();
            try
            {
                var deletedBy = (int?)HttpContext.Items["UserId"];
                var productImages = await _productService.GetProductImages(model.ProductId);
                var isDeleted = await _productService.DeleteProductImage(model.ImageId,(long)deletedBy);
                var imageUrl = productImages.FirstOrDefault(x => x.ImageID == model.ImageId)?.ImageUrl ?? "";
                if(isDeleted > 0)
                {
                    if (!string.IsNullOrEmpty(imageUrl))
                    {
                        DeletePhysicalFile(imageUrl);
                    }
                    response.Success = true;
                    response.Message = "Product image deleted successfully.";
                }
                else
                {
                    response.Success = false;
                    response.Message = "Please try later, Some issue occuring deleting image";
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
        /// Make Product Image Primary
        /// </summary>
        [HttpGet("set-primary-image/{ImageId}")]
        public async Task<BaseAPIResponse<long>> SetProductImagePrimary(long ImageId)
        {
            var response = new BaseAPIResponse<long>();
            try
            {
                var isPrimarySet = await _productService.SetProductImagePrimary(ImageId);
                if (isPrimarySet > 0)
                {
                    response.Success = true;
                    response.Message = "Product image set as primary successfully.";
                }
                else
                {
                    response.Success = false;
                    response.Message = "Please try later, Some issue occuring setting image primary";
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

        /// <summary>
        /// Delete physical file from server
        /// </summary>
        private void DeletePhysicalFile(string imageUrl)
        {
            try
            {
                var webRootPath = _environment.WebRootPath;
                imageUrl = imageUrl.TrimStart('/');
                var filePath = Path.Combine(webRootPath, imageUrl);
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting physical file: {ex.Message}");
            }
        }
        #endregion
    }
}
