using AccountDAL.Eentiti;
using AccountDAL.Eentiti.Order_Aggregate;

namespace cozastore.ViewModel.Ordes
{
    public class OrderToReturnViewModel
    {
        public int Id { get; set; }
        //public List<OrderItemViewModel> Items { get; set; } // Navigational Property [Many]
        //public DateTimeOffset OrderDate { get; set; }

        //public string DeliveryMethod { get; set; }
        //public decimal DeliveryCost { get; set; }
        //public string Status { get; set; }
        //public int PaymentIntentId { get; set; }
        //public decimal SubTotal { get; set; }
        //public decimal Total { get; set; }
        public AccountDAL.Eentiti.Order_Aggregate.Address ShipToAddress { get; set; }
        public string BuyerEmail { get; set; }
        public List<OrderItemViewModel> Items { get; set; } // Navigational Property [Many]
        public DateTimeOffset OrderDate { get; set; }
        public decimal DeliveryCost { get; set; } 
        public string DeliveryTime { get; set; } 

        public OrderStatus Status { get; set; }
        // جديد: طريقة الدفع
        public PaymentMethod PaymentMethod { get; set; }
        public decimal SubTotal { get; set; }



    }
}
