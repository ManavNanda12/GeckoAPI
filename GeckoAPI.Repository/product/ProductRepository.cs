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
            param.Add("@PageNumber", model.PageNumber);
            param.Add("@PageSize", model.PageSize);
            param.Add("@SearchTerm", model.SearchTerm);
            param.Add("@SortColumn", model.SortColumn);
            param.Add("@SortDirection", model.SortDirection);
            var products = Query<Products>(StoredProcedures.GetProductList,param);
            return Task.FromResult(products.Data.ToList());
        }

        public Task<long> SaveProduct(ProductSaveRequestModel model)
        {
            var param = new DynamicParameters();
            param.Add("@ProductId", model.ProductID);
            param.Add("@CategoryId", model.CategoryID);
            param.Add("@ProuductName", model.ProductName);
            param.Add("@ProductDescription", model.ProductDescription);
            param.Add("@Price", model.Price);
            param.Add("@Sku", model.SKU);
            param.Add("@CreatedBy", model.CreatedBy);
            var products = Execute(StoredProcedures.SaveProduct, param);
            return Task.FromResult(products.Data);
        }

        public Task<long> UpdateProductStockDetails(ProductStock model)
        {
            var param = new DynamicParameters();
            param.Add("@StockId", model.StockId);
            param.Add("@ProductId", model.ProductId);
            param.Add("@Quantity", model.Quantity);
            param.Add("@UpdatedBy", model.UpdatedBy);
            var result = Execute(StoredProcedures.UpdateProductStock, param);
            return Task.FromResult(result.Data);
        }

        public Task<ProductStock> GetProductStockDetails(long ProductId)
        {
            var param = new DynamicParameters();
            param.Add("@ProductId", ProductId);
            var result = QueryFirstOrDefault<ProductStock>(StoredProcedures.GetProductStockDetails, param);
            return Task.FromResult(result.Data);
        }

        public Task<long> SaveProductImage(SaveProductImageModel model)
        {
            var param = new DynamicParameters();
            param.Add("@productID", model.ProductId);
            param.Add("@ImageUrl", model.ImageUrl);
            param.Add("@IsPrimary", model.IsPrimary);
            param.Add("@CreatedBy", model.CreatedBy);
            var response = Execute(StoredProcedures.SaveProductImage, param);
            return Task.FromResult((long)response.Data);
        }

        #endregion


        #region Customer Methods

        public Task<List<Products>> GetCustomerProductList(long CategoryId,long CustomerId)
        {
            var param = new DynamicParameters();
            param.Add("@CategoryId", CategoryId);
            param.Add("@CustomerId", CustomerId);
            var response = Query<Products>(StoredProcedures.GetProductListByCategoryId_InStockOnly, param);
            return Task.FromResult(response.Data.ToList());
        }

        public Task<long> DeleteProductImage(long ImageId, long DeletedBy)
        {
            var param = new DynamicParameters();
            param.Add("@ImageId", ImageId);
            param.Add("@DeletedBy", DeletedBy);
            var response = Execute(StoredProcedures.DeleteProductImages, param);
            return Task.FromResult(response.Data);
        }

        public Task<List<ProductImagesResponseModel>> GetProductImages(long ProductId)
        {
            var param = new DynamicParameters();
            param.Add("@ProductId", ProductId);
            var response = Query<ProductImagesResponseModel>(StoredProcedures.GetProductImages, param);
            return Task.FromResult(response.Data.ToList());
        }

        public Task<long> SetProductImagePrimary(long ImageId)
        {
            var param = new DynamicParameters();
            param.Add("@ImageId", ImageId);
            var response = Execute(StoredProcedures.SetPrimaryProductImage, param);
            return Task.FromResult(response.Data);
        }

        #endregion
        #endregion
    }
}
