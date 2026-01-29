using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountDAL.Eentiti.CozaStore
{
    public class SubCategory:BaseEntity
    {
        public string Name_EN { get; set; }
        public string Name_Ar { get; set; }
        public string Picture { get; set; }

        public int CategoryId { get; set; }
		public Category Category { get; set; }
		public ICollection<Product> Products { get; set; } = new List<Product>();
	}
}
