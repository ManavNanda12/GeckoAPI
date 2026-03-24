using GeckoAPI.Model.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeckoAPI.Repository.order
{
    public interface IOrderRepository
    {
        Task<CheckoutOrderServiceResult> CheckoutOrder(CheckoutOrderRequestModel model);
        Task<List<OrderListResponse>> GetOrderList(long CustomerId);
        Task<List<OrderDetailResponse>> GetOrderDetail(long OrderId);
        Task<List<AdminOrderList>> GetAdminOrderList(CommonListRequestModel model);
    }
}
