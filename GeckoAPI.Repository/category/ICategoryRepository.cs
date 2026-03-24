using GeckoAPI.Model.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace GeckoAPI.Repository.category
{
    public interface ICategoryRepository
    {
        Task<List<CategoryListModel>> GetCategoryList();
        Task<int> SaveCategory(SaveCategoryModel model);
        Task<int> SaveCategoryImage(SaveCategoryImageModel model);
        Task<List<CategoryImageListModel>> GetCategoryImages(long CategoryId);
        Task<int> DeleteCategoryImages(CategoryImageDeleteRequestModel model);
    }
}
