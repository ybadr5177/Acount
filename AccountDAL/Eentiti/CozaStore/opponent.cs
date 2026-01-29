using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountDAL.Eentiti.CozaStore
{
    public class opponent:BaseEntity
    {
        public string Codes { get; set; }
        public decimal DiscountPercentage { get; set; }
    }
}
