using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountDAL.Specifications.Delivery
{
    public class DliverySpecParams
    {
        public string Type { get; set; }   // fast | standard
        public string Country { get; set; }
        public string State { get; set; }
    }
}
