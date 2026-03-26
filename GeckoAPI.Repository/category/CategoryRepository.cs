using Dapper;
using DemoWebAPI.Common;
using DemoWebAPI.model.Models;
using GeckoAPI.Model.models;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
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
            var query = GetPgFunctionQuery(
             StoredProcedures.GetCategoryList,
             true
           );
            var categories = Query<CategoryListModel>(query);
            return Task.FromResult(categories.Data.ToList());
        }
        public Task<int> SaveCategory(SaveCategoryModel model)
        {
            var param = new DynamicParameters();
            param.Add("@CategoryId", model.CategoryId, DbType.Int32);
            param.Add("@CategoryName", model.CategoryName);
            param.Add("@ParentCategoryId", model.ParentCategoryId, DbType.Int32);
            param.Add("@CreatedBy", model.CreatedBy, DbType.Int32);
            var query = GetPgFunctionQuery(StoredProcedures.SaveCategory, false, "@CategoryId,@ParentCategoryId,@CategoryName,@CreatedBy");
            var response = Execute(query, param);
            return Task.FromResult((int)response.Data);
        }
        public Task<int> SaveCategoryImage(SaveCategoryImageModel model)
        {
            var param = new DynamicParameters();
            param.Add("@CategoryID", model.CategoryId, DbType.Int32);
            param.Add("@ImageUrl", model.ImageUrl);
            param.Add("@IsPrimary", model.IsPrimary);
            param.Add("@CreatedBy", model.CreatedBy, DbType.Int32);
            var query = GetPgFunctionQuery(StoredProcedures.SaveCategoryImage, false, "@CategoryID,@ImageUrl,@IsPrimary,@CreatedBy");
            var response = Execute(query, param);
            return Task.FromResult((int)response.Data);
        }

        public Task<int> DeleteCategoryImages(CategoryImageDeleteRequestModel model)
        {
            var param = new DynamicParameters();
            param.Add("@DeletedBy", model.DeletedBy, DbType.Int32);
            param.Add("@ImageIds", model.ImageIds);
            var query = GetPgFunctionQuery(
                 StoredProcedures.DeleteCategoryImage,
                 false,
                 "@DeletedBy,@ImageIds"
            );
            var response = Execute(query, param);
            return Task.FromResult((int)response.Data);
        }

        public Task<List<CategoryImageListModel>> GetCategoryImages(long CategoryId)
        {
            var param = new DynamicParameters();
            param.Add("@CategoryId", CategoryId,DbType.Int32);
            var query = GetPgFunctionQuery(
                StoredProcedures.GetCategoryImageById,
                true,
                "@CategoryId"
            );
            var response = Query<CategoryImageListModel>(query, param);
            return Task.FromResult(response.Data.ToList());
        }
        #endregion
    }
}
