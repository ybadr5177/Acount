using AccountDAL.Eentiti;

namespace DashBoard.ViewModel
{
    public class DeliveryCostViewModel
    {
        public int Id { get; set; }
        public string Name_EN { get; set; }
        public string Name_AR { get; set; }   // اسم المحافظة أو الولاية

        public string ShippingType { get; set; }  // Fast / Standard
        public string DeliveryTime { get; set; }  // "1-2 Days" (اختياري لو محتاج)

        public decimal Cost { get; set; }         // تكلفة الشحن

        public int DeliveryCountryId { get; set; }
        public List<DeliveryCountryViewModel> DeliveryCountry { get; set; }

    }
}
