using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountDAL.Eentiti
{
    public class DeliveryCost:BaseEntity
    {
        public DeliveryCost(string name_EN, string name_AR, string shippingType, decimal cost)
        {
            Name_EN = name_EN;
            Name_AR = name_AR;
            ShippingType=shippingType;
            Cost = cost;

        }
        public string Name_EN { get; set; }
        public string Name_AR { get; set; }   // اسم المحافظة أو الولاية

        public string ShippingType { get; set; }  // Fast / Standard
        public string DeliveryTime { get; set; }  // "1-2 Days" (اختياري لو محتاج)

        public decimal Cost { get; set; }         // تكلفة الشحن

        public int DeliveryCountryId { get; set; }
        public DeliveryCountry DeliveryCountry { get; set; }
    }
}
