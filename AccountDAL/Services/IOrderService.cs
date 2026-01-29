using AccountDAL.Eentiti.Order_Aggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountDAL.Services
{
    public interface IOrderService
    {
        Task<Order> CreateOrderAsync(string buyerEmail, string basketId, int deliveryCostId, Address shippingAddress, PaymentMethod paymentMethod);

        Task<IReadOnlyList<Order>> GetOrdersForUser(string buyerEmail);

        Task<Order> GetOrderById(int orderId, string buyerEmail);

        Task<IReadOnlyList<DeliveryMethod>> GetDeliveryMethodAsync();
    }
}
