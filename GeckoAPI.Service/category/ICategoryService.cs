using GeckoAPI.Model.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeckoAPI.Service.category
{
    public interface ICategoryService
    {
        Task<List<CategoryListModel>> GetCategoryList();
        Task<int> SaveCategory(SaveCategoryModel model);
        Task<int> SaveCategoryImage(SaveCategoryImageModel model);
        Task<List<CategoryImageListModel>> GetCategoryImages(long CategoryId);
        Task<int> DeleteCategoryImages(CategoryImageDeleteRequestModel model);
    }
}
