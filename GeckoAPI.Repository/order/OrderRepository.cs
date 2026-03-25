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
            param.Add("p_cartsessionid", model.CartSessionId);
            param.Add("p_customerid", Convert.ToInt32(model.CustomerId), DbType.Int32);
            param.Add("p_billingaddress", model.BillingAddress);
            param.Add("p_shippingaddress", model.ShippingAddress);
            param.Add("p_shippingsameasbilling", model.ShippingSameAsBilling, DbType.Boolean);
            param.Add("p_paymentmethod", model.PaymentMethod);
            param.Add("p_ordernotes", model.OrderNotes);
            param.Add("p_paymentintentid", model.PaymentIntentId);
            param.Add("p_stripepaymentstatus", model.StripePaymentStatus);

            var query = GetPgFunctionQuery(
                StoredProcedures.CheckoutCart,
                true,
                "@p_cartsessionid, @p_customerid, @p_billingaddress, @p_shippingaddress, @p_shippingsameasbilling, @p_paymentmethod, @p_ordernotes, @p_paymentintentid, @p_stripepaymentstatus"
            );

            var response = await QueryMultipleAsync<CheckoutOrderServiceResult>(
                query,
                param,
                async (multi) =>
                {
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
            param.Add("@OrderId", OrderId, DbType.Int32);

            var query = GetPgFunctionQuery(
                StoredProcedures.GetOrderDetail,
                true,
                "@OrderId"
            );

            var response = Query<OrderDetailResponse>(query, param);
            return Task.FromResult(response.Data.ToList());
        }

        public Task<List<OrderListResponse>> GetOrderList(long CustomerId)
        {
            var param = new DynamicParameters();
            param.Add("@CustomerId", CustomerId, DbType.Int32);

            var query = GetPgFunctionQuery(
                StoredProcedures.GetOrderList,
                true,
                "@CustomerId"
            );

            var response = Query<OrderListResponse>(query, param);
            return Task.FromResult(response.Data.ToList());
        }

        public Task<List<AdminOrderList>> GetAdminOrderList(CommonListRequestModel model)
        {
            var param = new DynamicParameters();
            param.Add("@PageNumber", model.PageNumber, DbType.Int32);
            param.Add("@PageSize", model.PageSize, DbType.Int32);
            param.Add("@SearchTerm", model.SearchTerm);
            param.Add("@SortColumn", model.SortColumn);
            param.Add("@SortDirection", model.SortDirection);

            var query = GetPgFunctionQuery(
                StoredProcedures.GetAdminOrderList,
                true,
                "@PageNumber,@PageSize,@SearchTerm,@SortColumn,@SortDirection"
            );

            var response = Query<AdminOrderList>(query, param);
            return Task.FromResult(response.Data.ToList());
        }
        #endregion
    }
}
