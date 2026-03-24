using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeckoAPI.Model.models
{
    public class Coupon
    {
        public long CouponId { get; set; }

        // Basic Details
        public string CouponCode { get; set; }
        public string CouponName { get; set; }
        public string Description { get; set; }

        // Discount Details
        public string DiscountType { get; set; }   // "Percentage" or "Flat"
        public decimal DiscountValue { get; set; }

        // Validity
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        // Usage Limits
        public int? MaxUsageCount { get; set; }
        public int UsageCount { get; set; }
        public int? MaxUsagePerUser { get; set; }

        // Status
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public long? CreatedBy { get; set; }

        // Optional computed/display fields (not from DB)
        public int? TotalRecords { get; set; }      // Used when returning paged results
    }


    public class ApplyCouponResult:Coupon
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }  
    }

    public class ApplyCouponRequestModel
    {
        public string CouponCode { get; set; }
        public string CartSessionId { get; set; }
    }

    public class CouponUsedListResponseModel
    {
        public long CustomerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string CouponCode { get; set; }
        public string CouponName { get; set; }
        public string Description { get; set; }
        public string DiscountType { get; set; }
        public decimal DiscountValue { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public long MaxUsageCount { get; set; }
        public long MaxUsagePerUser { get; set; }
        public long UsageCount { get; set; }
        public DateTime LastUsedDate { get; set; }
        public long CouponUsedTime { get; set; }
    }

}
