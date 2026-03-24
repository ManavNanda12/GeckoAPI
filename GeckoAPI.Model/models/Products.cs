using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeckoAPI.Model.models
{
    public class Products
    {
        public long ProductID { get; set; }
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
        public long CategoryID { get; set; }
        public string? CategoryName { get; set; }
        public decimal Price { get; set; }
        public string? ProductImage { get; set; }
        public long? TotalRecords { get; set; }
        public string? SKU { get; set; }
        public bool? IsWishlistItem { get; set; }

    }

    public class ProductSaveRequestModel:Products
    {
        public long CreatedBy { get; set; }
    }

    public class SaveProductImageModel
    {
        public int ProductId { get; set; }
        public string? ImageUrl { get; set; }
        public bool? IsPrimary { get; set; }
        public int? CreatedBy { get; set; }
        public List<IFormFile>? ImageFile { get; set; } 
    }

    public class ProductImagesResponseModel:SaveProductImageModel{
        public long ImageID { get; set; }
    }

    public class ProductDeleteRequestModel
    {
        public long ProductId { get; set; }
        public long ImageId { get; set; }
    }
}
