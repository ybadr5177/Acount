using AccountDAL.Eentiti;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountDAL.Specifications.Delivery
{
    public class GetAllDeliverySpecification: BaseSpecification<DeliveryCost>
    {
        public GetAllDeliverySpecification()
        {
            AddInclude(sub => sub.DeliveryCountry);
        }
    }
}
