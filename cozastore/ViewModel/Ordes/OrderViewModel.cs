using AccountDAL.Eentiti.Order_Aggregate;

namespace cozastore.ViewModel.Ordes
{
    public class OrderViewModel
    {
        public string BasketId { get; set; }
        public int DeliveryCostId { get; set; } = 1;

        public AddressViewModel ShippingAddress { get; set; }

        // جديد: طريقة الدفع
        public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.Cash;
    }
}
