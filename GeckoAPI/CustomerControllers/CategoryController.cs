using DemoWebAPI.model.Models;
using GeckoAPI.Model.models;
using GeckoAPI.Service.category;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GeckoAPI.CustomerControllers
{
    [Route("api/customer/category")]
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

        private string GetBaseUrl()
        {
            var request = _httpContextAccessor.HttpContext?.Request;
            if (request == null) return string.Empty;

            return $"{request.Scheme}://{request.Host}";
        }

        #endregion
    }
}
