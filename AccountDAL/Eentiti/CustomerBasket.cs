using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountDAL.Eentiti
{
    public class CustomerBasket
    {
        public string Id { get; set; }
        public List<BasketItem> Items { get; set; } = new List<BasketItem>();

        public CustomerBasket(string id)
        {
            Id = id;
        }

        public int? DeliveryCostId { get; set; }
        public int? DiscountId { get; set; }

        public string PaymentIntentId { get; set; }
        public string ClientSecret { get; set; }
        public decimal ShippingPrice { get; set; }
    }
}
