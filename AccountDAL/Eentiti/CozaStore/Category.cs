using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountDAL.Eentiti.CozaStore
{
    public class Category:BaseEntity
    {
        public string Name_EN { get; set; }
        public string Name_Ar { get; set; }
        public string Picture { get; set; }

        public ICollection<SubCategory> SubCategories { get; set; } = new List<SubCategory>();

	}
}
