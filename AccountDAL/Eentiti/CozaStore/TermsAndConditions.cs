using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountDAL.Eentiti.CozaStore
{
    public class TermsAndConditions:BaseEntity
    {
        public string Content { get; set; }

        public DateTime LastUpdated { get; set; } = DateTime.Now;
    }
}
