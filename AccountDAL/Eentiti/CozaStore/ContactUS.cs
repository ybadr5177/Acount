using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountDAL.Eentiti.CozaStore
{
    public class ContactUS:BaseEntity
    {
        public string Address { get; set; }
        public string facebook { get; set; }
        public string instagram { get; set; }
        public string x { get; set; }
        public string whatsapp { get; set; }
        public string phonenumber { get; set; }
        public string email { get; set; }

    }
}
