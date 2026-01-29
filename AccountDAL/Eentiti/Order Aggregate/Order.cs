using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountDAL.Eentiti.Order_Aggregate
{
    public class Order:BaseEntity
    {
        public Order()
        {

        }
        public Order(string buyerEmail, List<OrderItem> items, Address shipToAddress, DeliveryCost deliveryCost, decimal subTotal)
        {
            BuyerEmail = buyerEmail;
            Items = items;
            ShipToAddress = shipToAddress;
            DeliveryCost = deliveryCost;
            SubTotal = subTotal;

        }

        public string BuyerEmail { get; set; }
        public List<OrderItem> Items { get; set; } // Navigational Property [Many]
        public DateTimeOffset OrderDate { get; set; } = DateTimeOffset.Now;
        public Address ShipToAddress { get; set; }
        public DeliveryCost DeliveryCost { get; set; } // Navigational Property [ONE]
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        // ✅ حالة الشحن (الافتراضية "Pending")
        public ShippingStatus ShippingStatus { get; set; } = ShippingStatus.Pending;
        // جديد: طريقة الدفع
        public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.Cash;
        public string PaymentIntentId { get; set; }
        public decimal SubTotal { get; set; }

        public decimal GetTotal()
            => SubTotal + DeliveryCost.Cost;
    }
}
