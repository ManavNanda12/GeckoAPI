using Dapper;
using DemoWebAPI.Common;
using DemoWebAPI.model.Models;
using GeckoAPI.Model.models;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeckoAPI.Repository.category
{
    public class CategoryRepository:BaseRepository,ICategoryRepository
    {
        #region Constructor
        public CategoryRepository(IOptions<DbConfig> config) : base(config)
        {
        }
        #endregion
        #region Methods
        public Task<List<CategoryListModel>> GetCategoryList()
        {
            var categories = Query<CategoryListModel>(StoredProcedures.GetCategoryList);
            return Task.FromResult(categories.Data.ToList());
        }
        public Task<int> SaveCategory(SaveCategoryModel model)
        {
            var param = new DynamicParameters();
            param.Add("@CategoryId", model.CategoryId);
            param.Add("@CategoryName", model.CategoryName);
            param.Add("@ParentCategoryId", model.ParentCategoryId);
            param.Add("@CreatedBy", model.CreatedBy);
            var response = Execute(StoredProcedures.SaveCategory, param);
            return Task.FromResult((int)response.Data);
        }
        public Task<int> SaveCategoryImage(SaveCategoryImageModel model)
        {
            var param = new DynamicParameters();
            param.Add("@CategoryID", model.CategoryId);
            param.Add("@ImageUrl", model.ImageUrl);
            param.Add("@IsPrimary", model.IsPrimary);
            param.Add("@CreatedBy", model.CreatedBy);
            var response = Execute(StoredProcedures.SaveCategoryImage, param);
            return Task.FromResult((int)response.Data);
        }

        public Task<int> DeleteCategoryImages(CategoryImageDeleteRequestModel model)
        {
            var param = new DynamicParameters();
            param.Add("@DeletedBy", model.DeletedBy);
            param.Add("@ImageIds", model.ImageIds);
            var response = Execute(StoredProcedures.DeleteCategoryImage, param);
            return Task.FromResult((int)response.Data);
        }

        public Task<List<CategoryImageListModel>> GetCategoryImages(long CategoryId)
        {
            var param = new DynamicParameters();
            param.Add("@CategoryId", CategoryId);
            var response = Query<CategoryImageListModel>(StoredProcedures.GetCategoryImageById, param);
            return Task.FromResult(response.Data.ToList());
        }
        #endregion
    }
}
