using AccountDAL.Eentiti.Order_Aggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountDAL.Specifications.Orders
{
    public class OrderWithItemsAndDeliveryMethodSpecifications: BaseSpecification<Order>
    {
        public OrderWithItemsAndDeliveryMethodSpecifications(string buyerEmail)
           : base(O => O.BuyerEmail == buyerEmail)
        {
            Includes.Add(O => O.Items);
            Includes.Add(O => O.DeliveryCost);
            Includes.Add(O => O.ShipToAddress);


            AddOrderByDescending(O => O.OrderDate);
        }
        public OrderWithItemsAndDeliveryMethodSpecifications(int orderId, string buyerEmail)
            : base(O => (O.BuyerEmail == buyerEmail) && (O.Id == orderId))
        {
            Includes.Add(O => O.Items);
            Includes.Add(O => O.DeliveryCost);
        }
    }
}
