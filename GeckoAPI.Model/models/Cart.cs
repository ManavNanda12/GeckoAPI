using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace GeckoAPI.Model.models
{
    public class Cart
    {
        public string? SessionId { get; set; }
        public long? CustomerId { get; set; }
    }

    public class AddToCartSaveModel
    {
        public string? SessionId { get; set; }
        public long? CustomerId { get; set; }
        public long ProductId { get; set; }
        public decimal Price { get; set; }
        public decimal Discount { get; set; }
        public long Quantity { get; set; }
    }

    public class UpdateCartItemsSaveModel
    {

        public string? SessionId { get; set; }
        public long CustomerId { get; set; }
        public long ProductId { get; set; }
        public long NewQuantity { get; set; }
    }

    public class RemoveItemFromCartRequestModel: Cart
    {
        public long ProductId { get; set; }
    }

    public class CartItemDetails
    {
        public long ItemId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
        public string ImageUrl { get; set; }
        public decimal Price { get; set; }
        public decimal Discount { get; set; }
        public int Quantity { get; set; }
        public DateTime CreatedAt { get; set; }
        public decimal ItemTotal { get; set; }
        public long CartId { get; set; }
        public long? CustomerId { get; set; }

        // New stock-related properties
        public int StockQuantity { get; set; }
        public int ReservedQuantity { get; set; }
        public int AvailableQuantity { get; set; }
        public string StockStatus { get; set; }
        public int MaxAvailableQuantity { get; set; }
        public bool IsQuantityAvailable { get; set; }

        public int CouponId { get; set; }
        public string CouponCode { get; set; }
        public string DiscountType { get; set; }
        public decimal DiscountValue { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal SubTotal { get; set; }
        public decimal TaxPercentage { get; set; }
        public decimal SubscriptionDiscount { get; set; }
        public bool IsFreeShipping { get; set; }
        public string CurrentPlanName { get; set; }
    }

    public class UpdateCartCutomerIdRequestModel
    {
        public long CustomerId { get; set; }
        public long CartId { get; set; }
    }
}
