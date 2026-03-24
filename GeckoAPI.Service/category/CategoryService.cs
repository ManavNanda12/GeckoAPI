using GeckoAPI.Model.models;
using GeckoAPI.Repository.category;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeckoAPI.Service.category
{
    public class CategoryService: ICategoryService
    {
        #region Fields
        private readonly ICategoryRepository _categoryRepository;
        #endregion

        #region Constructor
        public CategoryService(ICategoryRepository categoryRepository) { 
            _categoryRepository = categoryRepository;
        }
        #endregion

        #region Methods
        public Task<List<CategoryListModel>> GetCategoryList()
        {
            var data = _categoryRepository.GetCategoryList();
            return data;
        }

        public Task<int> SaveCategory(SaveCategoryModel model)
        {
            var data = _categoryRepository.SaveCategory(model);
            return data;
        }

        public Task<int> SaveCategoryImage(SaveCategoryImageModel model)
        {
            var data = _categoryRepository.SaveCategoryImage(model);
            return data;
        }

        public Task<int> DeleteCategoryImages(CategoryImageDeleteRequestModel model)
        {
            var data = _categoryRepository.DeleteCategoryImages(model);
            return data;
        }

        public Task<List<CategoryImageListModel>> GetCategoryImages(long CategoryId)
        {
            var data = _categoryRepository.GetCategoryImages(CategoryId);
            return data;
        }
        #endregion
    }
}
