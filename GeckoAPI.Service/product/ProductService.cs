using GeckoAPI.Model.models;
using GeckoAPI.Repository.product;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace GeckoAPI.Service.product
{
    public class ProductService : IProductService
    {

        #region Fields
        private readonly IProductRepository _productRepository;
        //private readonly IDistributedCache _cache;
        #endregion

        #region Constructor
        // IDistributedCache cache
        public ProductService(IProductRepository productRepository) { 
            _productRepository = productRepository;
            //_cache = cache;
        }
        #endregion

        #region Methods

        #region Admin Methods
        public async Task<List<Products>> GetProductList(CommonListRequestModel model)
        {
            //string version = await _cache.GetStringAsync("products_version") ?? "1";
            //string cacheKey = $"products_{version}_{model.PageNumber}_{model.PageSize}_{model.SearchTerm}";



            // 1️⃣ Try cache
            //var cached = await _cache.GetStringAsync(cacheKey);

            //if (!string.IsNullOrEmpty(cached))
            //{
            //    return JsonSerializer.Deserialize<List<Products>>(cached);
            //}

            // 2️⃣ DB call
            var data = await _productRepository.GetProductList(model);

            // 3️⃣ Save to cache
            //await _cache.SetStringAsync(
            //    cacheKey,
            //    JsonSerializer.Serialize(data),
            //    new DistributedCacheEntryOptions
            //    {
            //        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            //    });

            return data;
        }


        public async Task<long> SaveProduct(ProductSaveRequestModel model)
        {
            var data = await _productRepository.SaveProduct(model);
            //await _cache.SetStringAsync("products_version", Guid.NewGuid().ToString());
            return data;
        }

        public async Task<long> UpdateProductStockDetails(ProductStock model)
        {
            var data = await _productRepository.UpdateProductStockDetails(model);
            //await _cache.SetStringAsync("products_version", Guid.NewGuid().ToString());
            return data;
        }
        public Task<ProductStock> GetProductStockDetails(long ProductId)
        {
            var data = _productRepository.GetProductStockDetails(ProductId);
            return data;
        }

        public async Task<long> SaveProductImage(SaveProductImageModel model)
        {
            var data = await _productRepository.SaveProductImage(model);
            //await _cache.SetStringAsync("products_version", Guid.NewGuid().ToString());
            return data;
        }

        #endregion

        #region Customer Methods
        public async Task<List<Products>> GetCustomerProductList(long CategoryId, long CustomerId)
        {
            //string version = await _cache.GetStringAsync("customer_products_version") ?? "1";
            //string cacheKey = $"customer_products_{version}_{CategoryId}_{CustomerId}";



            // 1️⃣ Try cache
            //var cached = await _cache.GetStringAsync(cacheKey);

            //if (!string.IsNullOrEmpty(cached))
            //{
            //    return JsonSerializer.Deserialize<List<Products>>(cached);
            //}

            // 2️⃣ DB call
            var data = await _productRepository.GetCustomerProductList(CategoryId, CustomerId);

            // 3️⃣ Save to cache
            //await _cache.SetStringAsync(
            //    cacheKey,
            //    JsonSerializer.Serialize(data),
            //    new DistributedCacheEntryOptions
            //    {
            //        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            //    });

            return data;
        }

        public Task<long> DeleteProductImage(long ImageId, long DeletedBy)
        {
            var data = _productRepository.DeleteProductImage(ImageId,DeletedBy);
            return data;
        }

        public Task<List<ProductImagesResponseModel>> GetProductImages(long ProductId)
        {
            var data = _productRepository.GetProductImages(ProductId);
            return data;
        }

        public Task<long> SetProductImagePrimary(long ImageId)
        {
            var data = _productRepository.SetProductImagePrimary(ImageId);
            return data;
        }
        #endregion
        #endregion
    }
}
