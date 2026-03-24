using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeckoAPI.Model.models
{
    public class Categories
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int? ParentCategoryId { get; set; }
        public DateTime CreatedAt { get; set; }
        public long CreatedBy { get; set; }
        public DateTime UpdatedAt { get; set; }
        public long? UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }
        public long? DeletedBy { get; set; }
    }

    public class  CategoryListModel
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string ImageUrl { get; set; }
        public long? ParentCategoryID { get; set; }

    }

    public class SaveCategoryModel
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int? ParentCategoryId { get; set; }
        public int? CreatedBy { get; set; }
        public IFormFile? ImageFile { get; set; }

    }

    public class SaveCategoryImageModel
    {
        public int CategoryId { get; set; }
        public string? ImageUrl { get; set; }
        public bool? IsPrimary { get; set; }
        public int? CreatedBy { get; set; }
        public IFormFile ImageFile { get; set; }
    }

    public class CategoryImageListModel
    {
        public string ImageUrl { get; set; }
        public long ImageID { get; set; }
    }

    public class CategoryImageDeleteRequestModel
    {
        public string ImageIds { get; set; }
        public long DeletedBy { get; set; }
    }
}
