using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountDAL.Eentiti
{
    public class DeliveryCountry: BaseEntity
    {
        public string Name_EN { get; set; }
        public string Name_AR { get; set; }

        // Navigation
        public ICollection<DeliveryCost> DeliveryCost { get; set; }
    }
}
