using GeckoAPI.Model.models;
using GeckoAPI.Repository.order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeckoAPI.Service.order
{
    public class OrderService: IOrderService
    {
        #region Fields
        private readonly IOrderRepository _orderRepository;
        #endregion
        #region Constructor
        public OrderService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }
        #endregion
        #region Methods
        public Task<CheckoutOrderServiceResult> CheckoutOrder(CheckoutOrderRequestModel model)
        {
            var data = _orderRepository.CheckoutOrder(model);
            return data;
        }

        public Task<List<OrderDetailResponse>> GetOrderDetail(long OrderId)
        {
            var data = _orderRepository.GetOrderDetail(OrderId);
            return data;
        }

        public Task<List<OrderListResponse>> GetOrderList(long CustomerId)
        {
            var data = _orderRepository.GetOrderList(CustomerId);
            return data;
        }
        public Task<List<AdminOrderList>> GetAdminOrderList(CommonListRequestModel model)
        {
            var data = _orderRepository.GetAdminOrderList(model);
            return data;
        }
        #endregion
    }
}
