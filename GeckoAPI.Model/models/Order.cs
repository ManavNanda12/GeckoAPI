using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace GeckoAPI.Model.models
{
    public class Order
    {
    }

    public class CheckoutOrderRequestModel
    {
        public string CartSessionId { get; set; }
        public int? CustomerId { get; set; }
        public string BillingAddress { get; set; }
        public string ShippingAddress { get; set; }
        public bool ShippingSameAsBilling { get; set; }
        public string PaymentMethod { get; set; }
        public string OrderNotes { get; set; }
        public string PaymentIntentId { get; set; }
        public string StripePaymentStatus { get; set; }

    }

    public class OrderListResponse
    {
        public long Id { get; set; }
        public string OrderNumber { get; set; }
        public long OrderStatus { get; set; }
        public decimal Total { get; set; }
        public DateTime CreatedAt { get; set; }

        public long ProductId { get; set; }
        public string ProductName { get; set; }
        public string StripePaymentStatus { get; set; }
        public long PaymentStatus { get; set; }
    }

    public class OrderDetailResponse
    {
        public long Id { get; set; }
        public long ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }

        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal Discount { get; set; }
        public decimal LineTotal { get; set; }

        public decimal Total { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal ShippingAmount { get; set; }
        public decimal DiscountAmount { get; set; }

        public string PaymentMethod { get; set; }
        public string StripePaymentStatus { get; set; }
        public long PaymentStatus { get; set; }
        public long OrderStatus { get; set; }
        public DateTime CreatedAt { get; set; }
        public string ImageUrl { get; set; }
        public string CouponCode { get; set; }
        public decimal SubscriptionDiscount { get; set; }
        public string PlanName { get; set; }
        public decimal SubscriptionBenefitValue { get; set; }
    }

    public class AdminOrderList
    {
        public long Id { get; set; }
        public string OrderNumber { get; set; }
        public long OrderStatus { get; set; }
        public long PaymentStatus { get; set; }
        public string BillingAddress { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal Total { get; set; }
        public decimal SubTotal { get; set; }
        public long CustomerId { get; set; }
        public string FullName { get; set; }
        public long TotalRecords { get; set; }
    }

    public class CheckoutMainResult
    {
        public int Result { get; set; }
        public string Message { get; set; }
        public long? OrderId { get; set; }
        public string OrderNumber { get; set; }
        public decimal? Total { get; set; }
    }

    // Service response model
    public class CheckoutOrderServiceResult
    {
        public int Result { get; set; }
        public string Message { get; set; }
        public long OrderId { get; set; }
        public string OrderNumber { get; set; }
        public decimal Total { get; set; }
        public List<OutOfStockItem> OutOfStockItems { get; set; }
    }

    // Model for out-of-stock items (second result set)
    public class OutOfStockItem
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int RequestedQty { get; set; }
        public int AvailableQty { get; set; }
    }

    public class CheckoutResultRow
    {
        public int Result { get; set; }
        public string Message { get; set; }

        // Success fields
        public long? OrderId { get; set; }
        public string OrderNumber { get; set; }
        public decimal? Total { get; set; }

        // Out-of-stock fields
        public int? ProductId { get; set; }
        public string ProductName { get; set; }
        public int? RequestedQty { get; set; }
        public int? AvailableQty { get; set; }
    }


}
