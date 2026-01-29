using AccountDAL.Eentiti;
using AccountDAL.Eentiti.CozaStore;
using AccountDAL.Eentiti.Order_Aggregate;
using AccountDAL.Repositories;
using AccountDAL.Services;
using AccountDAL.Specifications.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Service
{
    public class OrderService:IOrderService
    {
        private readonly IBasketRepository _basketRepo;
        private readonly IUnitOfWork _unitOfWork;

        public OrderService(
            IBasketRepository basketRepo,
            IUnitOfWork unitOfWork
            )
        {
            _basketRepo = basketRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task<Order> CreateOrderAsync(string buyerEmail, string basketId, int deliverCostId, AccountDAL.Eentiti.Order_Aggregate. Address shippingAddress, PaymentMethod paymentMethod)
        {
            // 1. Get Basket From Baskets Repo
            var basket = await _basketRepo.GetBasketAsync(basketId);

            // 2. Get Selected Items at Basket From Products Repo
            var orderItems = new List<OrderItem>();
            foreach (var item in basket.Items)
            {
                var product = await _unitOfWork.Repository<Product>().GetByIdAsync(item.Id);
                var productItemOrdered = new ProductItemOrdered(product.Id, product.Address_En, product.Address_Ar/*هنا  بدل العنون العربي عايز صورة*/);

                var orderItem = new OrderItem(productItemOrdered, product.BasePrice??0, item.Quantity);
                orderItems.Add(orderItem);
            }

            // 3. Get DeliveryCost From DeliveryCost Repo
            var deliveryCost = await _unitOfWork.Repository<DeliveryCost>().GetByIdAsync(deliverCostId);
          
            // 5. Calculate SubTotal
            var subTotal = orderItems.Sum(item => item.Price * item.Quantity) + deliveryCost.Cost;

            // 5. Check Discount
            opponent discount = null;

            if (basket.DiscountId != null)
            {
                discount = await _unitOfWork.Repository<opponent>()
                                            .GetByIdAsync(basket.DiscountId.Value);

                if (discount != null)
                {
                    // ✅ خصم ثابت
                    subTotal -= discount.DiscountPercentage;

                    if (subTotal < 0) subTotal = 0;
                }
            }

            // 6. Create Order
            var order = new Order(buyerEmail, orderItems, shippingAddress, deliveryCost, subTotal);
            order.PaymentIntentId = basket.PaymentIntentId?? "Cash";

            await _unitOfWork.Repository<Order>().AddAsync(order);

            // 7. Save To Database [TODO]

            var result = await _unitOfWork.Complete();
            if (result <= 0) return null;

            return order;
        }


        public async Task<Order> GetOrderById(int orderId, string buyerEmail)
        {
            var spec = new OrderWithItemsAndDeliveryMethodSpecifications(orderId, buyerEmail);
            var order = await _unitOfWork.Repository<Order>().GetByIdWithSpecAsync(spec);
            return order;
        }

        public async Task<IReadOnlyList<Order>> GetOrdersForUser(string buyerEmail)
        {
            var spec = new OrderWithItemsAndDeliveryMethodSpecifications(buyerEmail);
            var orders = await _unitOfWork.Repository<Order>().GetAllWithSpecAsync(spec);

            return orders;
        }


        public async Task<IReadOnlyList<DeliveryMethod>> GetDeliveryMethodAsync()
        {
            return await _unitOfWork.Repository<DeliveryMethod>().GetAllAsync();

        }
    }
}
