using DemoWebAPI.model.Models;
using DemoWebAPI.Service.User;
using GeckoAPI.Common;
using GeckoAPI.Model.models;
using GeckoAPI.Service.category;
using GeckoAPI.Service.jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Buffers.Text;

namespace GeckoAPI.Controllers
{
    [Authorize]
    [Route("api/category")]
    [ApiController]
    public class CategoryController : ControllerBase
    {

        #region Fields
        private readonly ICategoryService _categoryService;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;
        private readonly IHttpContextAccessor _httpContextAccessor;
        #endregion

        #region Constructor
        public CategoryController(ICategoryService categoryService, IConfiguration configuration, IWebHostEnvironment environment, IHttpContextAccessor httpContextAccessor)
        {
            _categoryService = categoryService;
            _configuration = configuration;
            _environment = environment;
            _httpContextAccessor = httpContextAccessor;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Get All Categories
        /// </summary>
        [HttpGet("get-category-list")]
        public async Task<BaseAPIResponse<List<CategoryListModel>>> GetCategoryList()
        {
            var response = new BaseAPIResponse<List<CategoryListModel>>();
            try
            {
                var baseUrl = GetBaseUrl();
                var categories = await _categoryService.GetCategoryList();
                var categoryList = categories.Select(c => new CategoryListModel
                {
                    ParentCategoryID = c.ParentCategoryID,
                    CategoryId = c.CategoryId,
                    CategoryName = c.CategoryName,
                    ImageUrl = !string.IsNullOrEmpty(c.ImageUrl)
                  ? $"{baseUrl}/{c.ImageUrl}"
                  : null
                }).ToList();

                response.Data = categoryList;
                response.Success = true;
                response.Message = "Categories fetched successfully.";
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = $"An error occurred: {ex.Message}";
            }
            return response;
        }

        /// <summary>
        /// Save Category
        /// </summary>        
        [HttpPost("save-category")]
        public async Task<BaseAPIResponse<long>> SaveCategory ([FromForm] SaveCategoryModel model)
        {
            var response = new BaseAPIResponse<long>();
            try
            {
                if (model.ParentCategoryId == 0)
                {
                    model.ParentCategoryId = null;
                }
                model.CreatedBy = (int?)HttpContext.Items["UserId"];
                var result = await _categoryService.SaveCategory(model);
                response.Success = true;
                if (model.ImageFile != null)
                {
                    var requestedModel = new SaveCategoryImageModel
                    {
                        ImageFile = model.ImageFile,
                        CategoryId = model.CategoryId,
                        IsPrimary = true,
                        CreatedBy = 1
                    };
                  if(model.CategoryId == 0)
                    {
                        requestedModel.CategoryId = result;
                    }
                  var imageRes = await SaveCategoryImage(requestedModel);
                }
                if (result == 0)
                {
                    response.Message = "Category updated successfully.";
                }
                else if (result > 0)
                {
                    response.Message = "Category added successfully";
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
        /// Save Category Image
        /// </summary>        
        [HttpPost("save-category-image")]
        public async Task<BaseAPIResponse<long>> SaveCategoryImage([FromForm] SaveCategoryImageModel model)
        {
            var response = new BaseAPIResponse<long>();
            try
            {
                if(CommonHelper.IsValidImageFile(model.ImageFile) == false)
                {
                    response.Success = false;
                    response.Message = "Please upload a valid image file.";
                    return response;
                }
                var categoryImages = _categoryService.GetCategoryImages(model.CategoryId);
                if(categoryImages.Result.Count > 0)
                {
                    var deleteImages = new CategoryImageDeleteRequestModel
                    {
                        DeletedBy = 1,
                        ImageIds = string.Join(",", categoryImages.Result.Select(x => x.ImageID))
                    };
                    var isImagesDeleted = _categoryService.DeleteCategoryImages(deleteImages);
                    foreach (var img in categoryImages.Result)
                    {
                        if (!string.IsNullOrEmpty(img.ImageUrl))
                        {
                            // combine wwwroot with relative path
                            var fullPath = Path.Combine(_environment.WebRootPath, img.ImageUrl.Replace("/", Path.DirectorySeparatorChar.ToString()));

                            if (System.IO.File.Exists(fullPath))
                            {
                                System.IO.File.Delete(fullPath);
                            }
                        }
                    }
                }
                string categoryImagesPath = Path.Combine(_environment.WebRootPath, "CategoryImages");
                if (!Directory.Exists(categoryImagesPath))
                {
                    Directory.CreateDirectory(categoryImagesPath);
                }
                // Generate GUID for filename
                string fileExtension = Path.GetExtension(model.ImageFile.FileName);
                string guidFileName = $"{Guid.NewGuid()}{fileExtension}";
                string filePath = Path.Combine(categoryImagesPath, guidFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await model.ImageFile.CopyToAsync(fileStream);
                }
                model.ImageUrl = $"CategoryImages/{guidFileName}";

                var result = await _categoryService.SaveCategoryImage(model);
                response.Success = true;
                if (result > 0)
                {
                    response.Message = "Category image added successfully";
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
