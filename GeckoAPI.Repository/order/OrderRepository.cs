using Dapper;
using DemoWebAPI.Common;
using DemoWebAPI.model.Models;
using GeckoAPI.Model.models;
using MailKit.Search;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeckoAPI.Repository.order
{
    public class OrderRepository:BaseRepository,IOrderRepository
    {

        #region Constructor
        public OrderRepository(IOptions<DbConfig> config) : base(config)
        {
        }
        #endregion

        #region Methods
        public async Task<CheckoutOrderServiceResult> CheckoutOrder(CheckoutOrderRequestModel model)
        {
            var param = new DynamicParameters();
            param.Add("@CustomerId", model.CustomerId);
            param.Add("@CartSessionId", model.CartSessionId);
            param.Add("@BillingAddress", model.BillingAddress);
            param.Add("@ShippingAddress", model.ShippingAddress);
            param.Add("@ShippingSameAsBilling", model.ShippingSameAsBilling);
            param.Add("@PaymentMethod", model.PaymentMethod);
            param.Add("@OrderNotes", model.OrderNotes);
            param.Add("@PaymentIntentId", model.PaymentIntentId);
            param.Add("@StripePaymentStatus", model.StripePaymentStatus);

            var response = await QueryMultipleAsync<CheckoutOrderServiceResult>(
                StoredProcedures.CheckoutCart,
                param,
                async (multi) =>
                {
                    // Read ALL rows from the first (and only) result set
                    var allResults = (await multi.ReadAsync<CheckoutResultRow>()).ToList();

                    if (!allResults.Any())
                    {
                        return new CheckoutOrderServiceResult
                        {
                            Result = -1,
                            Message = "No results returned"
                        };
                    }

                    var firstRow = allResults.First();
                    var result = new CheckoutOrderServiceResult
                    {
                        Result = firstRow.Result,
                        Message = firstRow.Message
                    };

                    if (result.Result == -2)
                    {
                        // Map to out-of-stock items (all rows have the product details)
                        result.OutOfStockItems = allResults.Select(r => new OutOfStockItem
                        {
                            ProductId = r.ProductId ?? 0,
                            ProductName = r.ProductName,
                            RequestedQty = r.RequestedQty ?? 0,
                            AvailableQty = r.AvailableQty ?? 0
                        }).ToList();
                    }
                    else if (result.Result == 1)
                    {
                        result.OrderId = firstRow.OrderId ?? 0;
                        result.OrderNumber = firstRow.OrderNumber;
                        result.Total = firstRow.Total ?? 0;
                    }

                    return result;
                }
            );

            return response.Data;
        }



        public Task<List<OrderDetailResponse>> GetOrderDetail(long OrderId)
        {
            var param = new DynamicParameters();
            param.Add("@OrderId", OrderId);
            var response = Query<OrderDetailResponse>(StoredProcedures.GetOrderDetail, param);
            return Task.FromResult(response.Data.ToList());

        }

        public Task<List<OrderListResponse>> GetOrderList(long CustomerId)
        {
            var param = new DynamicParameters();
            param.Add("@CustomerId", CustomerId);
            var response = Query<OrderListResponse>(StoredProcedures.GetOrderList, param);
            return Task.FromResult(response.Data.ToList());
        }
        public Task<List<AdminOrderList>> GetAdminOrderList(CommonListRequestModel model)
        {
            var param = new DynamicParameters();
            param.Add("@PageNumber", model.PageNumber);
            param.Add("@PageSize", model.PageSize);
            param.Add("@SearchTerm", model.SearchTerm);
            param.Add("@SortColumn", model.SortColumn);
            param.Add("@SortDirection", model.SortDirection);
            var orders = Query<AdminOrderList>(StoredProcedures.GetAdminOrderList, param);
            return Task.FromResult(orders.Data.ToList());
        }
        #endregion
    }
}
