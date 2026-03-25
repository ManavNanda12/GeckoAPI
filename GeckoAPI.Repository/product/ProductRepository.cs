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

namespace GeckoAPI.Repository.product
{
    public class ProductRepository : BaseRepository, IProductRepository
    {
        #region Constructor
        public ProductRepository(IOptions<DbConfig> config) : base(config)
        {

        }
        #endregion
        #region Methods

        #region Admin Methods

        public Task<List<Products>> GetProductList(CommonListRequestModel model)
        {
            var param = new DynamicParameters();
            param.Add("@PageNumber", model.PageNumber, DbType.Int32);
            param.Add("@PageSize", model.PageSize, DbType.Int32);
            param.Add("@SearchTerm", model.SearchTerm);
            param.Add("@SortColumn", model.SortColumn);
            param.Add("@SortDirection", model.SortDirection);

            var query = GetPgFunctionQuery(
                StoredProcedures.GetProductList,
                true,
                "@PageNumber,@PageSize,@SearchTerm,@SortColumn,@SortDirection"
            );

            var response = Query<Products>(query, param);
            return Task.FromResult(response.Data.ToList());
        }

        public Task<long> SaveProduct(ProductSaveRequestModel model)
        {
            var param = new DynamicParameters();
            param.Add("@ProductId", model.ProductID, DbType.Int32);
            param.Add("@CategoryId", model.CategoryID, DbType.Int32);
            param.Add("@ProuductName", model.ProductName);
            param.Add("@ProductDescription", model.ProductDescription);
            param.Add("@Price", model.Price);
            param.Add("@Sku", model.SKU);
            param.Add("@CreatedBy", model.CreatedBy);

            var query = GetPgFunctionQuery(
                StoredProcedures.SaveProduct,
                false,
                "@ProductId,@CategoryId,@ProuductName,@ProductDescription,@Price,@Sku,@CreatedBy"
            );

            var response = Execute(query, param);
            return Task.FromResult(response.Data);
        }

        public Task<long> UpdateProductStockDetails(ProductStock model)
        {
            var param = new DynamicParameters();
            param.Add("@StockId", model.StockId, DbType.Int32);
            param.Add("@ProductId", model.ProductId, DbType.Int32);
            param.Add("@Quantity", model.Quantity, DbType.Int32);
            param.Add("@UpdatedBy", model.UpdatedBy, DbType.Int32);

            var query = GetPgFunctionQuery(
                StoredProcedures.UpdateProductStock,
                false,
                "@StockId,@ProductId,@Quantity,@UpdatedBy"
            );

            var response = Execute(query, param);
            return Task.FromResult(response.Data);
        }

        public Task<ProductStock> GetProductStockDetails(long ProductId)
        {
            var param = new DynamicParameters();
            param.Add("@ProductId", ProductId, DbType.Int32);

            var query = GetPgFunctionQuery(
                StoredProcedures.GetProductStockDetails,
                true,
                "@ProductId"
            );

            var response = QueryFirstOrDefault<ProductStock>(query, param);
            return Task.FromResult(response.Data);
        }

        public Task<long> SaveProductImage(SaveProductImageModel model)
        {
            var param = new DynamicParameters();
            param.Add("@productID", model.ProductId,DbType.Int32);
            param.Add("@ImageUrl", model.ImageUrl);
            param.Add("@IsPrimary", model.IsPrimary);
            param.Add("@CreatedBy", model.CreatedBy, DbType.Int32);

            var query = GetPgFunctionQuery(
                StoredProcedures.SaveProductImage,
                false,
                "@productID,@ImageUrl,@IsPrimary,@CreatedBy"
            );

            var response = Execute(query, param);
            return Task.FromResult((long)response.Data);
        }

        #endregion


        #region Customer Methods

        public Task<List<Products>> GetCustomerProductList(long CategoryId,long CustomerId)
        {
            var param = new DynamicParameters();
            param.Add("@CategoryId", CategoryId, DbType.Int32);
            param.Add("@CustomerId", CustomerId, DbType.Int32);
            var query = GetPgFunctionQuery(StoredProcedures.GetProductListByCategoryId_InStockOnly,true, "@CategoryId,@CustomerId");
            var response = Query<Products>(query, param);
            return Task.FromResult(response.Data.ToList());
        }

        public Task<long> DeleteProductImage(long ImageId, long DeletedBy)
        {
            var param = new DynamicParameters();
            param.Add("@ImageId", ImageId, DbType.Int32);
            param.Add("@DeletedBy", DeletedBy, DbType.Int32);
            var query = GetPgFunctionQuery(StoredProcedures.DeleteProductImages, false, "@ImageId,@DeletedBy");
            var response = Execute(query, param);
            return Task.FromResult(response.Data);
        }

        public Task<List<ProductImagesResponseModel>> GetProductImages(long ProductId)
        {
            var param = new DynamicParameters();
            param.Add("@ProductId", ProductId);
            var query = GetPgFunctionQuery(StoredProcedures.GetProductImages, true, "@ProductId");
            var response = Query<ProductImagesResponseModel>(query, param);
            return Task.FromResult(response.Data.ToList());
        }

        public Task<long> SetProductImagePrimary(long ImageId)
        {
            var param = new DynamicParameters();
            param.Add("@ImageId", ImageId);
            var query = GetPgFunctionQuery(StoredProcedures.SetPrimaryProductImage, false, "@ImageId");
            var response = Execute(query, param);
            return Task.FromResult(response.Data);
        }

        #endregion
        #endregion
    }
}
