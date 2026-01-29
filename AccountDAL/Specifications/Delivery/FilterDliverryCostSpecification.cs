using AccountDAL.Eentiti;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountDAL.Specifications.Delivery
{
    public class FilterDliverryCostSpecification:BaseSpecification<DeliveryCost>
    {
        public FilterDliverryCostSpecification(DliverySpecParams  dliverySpecParams) : base(p =>
                        (p.Name_EN == dliverySpecParams.State) &&
            (p.ShippingType == dliverySpecParams.Type) &&
            (p.DeliveryCountry.Name_EN == dliverySpecParams.Country)

            )
        {
            
        }
    }
}
