using GeckoAPI.Model.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeckoAPI.Repository.product
{
    public interface IProductRepository
    {
        #region Admin Methods
        Task<List<Products>> GetProductList(CommonListRequestModel model);
        Task<long> SaveProduct(ProductSaveRequestModel model);
        Task<ProductStock> GetProductStockDetails(long ProductId);
        Task<long> UpdateProductStockDetails(ProductStock model);
        Task<long> SaveProductImage(SaveProductImageModel model);
        Task<long> DeleteProductImage(long ImageId, long DeletedBy);
        Task<List<ProductImagesResponseModel>> GetProductImages(long ProductId);
        Task<long> SetProductImagePrimary(long ImageId);
        #endregion

        #region Customer Methods
        Task<List<Products>> GetCustomerProductList(long CategoryId, long CustomerId);
        #endregion
    }
}
