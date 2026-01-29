using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountDAL.Eentiti.CozaStore
{
    public class DifferentSize:BaseEntity
    {
        public string? Label { get; set; } // S / M / L / 42 / 43 ...
        public int SizeNameId { get; set; }
        public SizeName SizeName { get; set; }

        public DimensionSize? Dimensions { get; set; } // Navigation property
        public ICollection<ProductSize> ProductSizes { get; set; }=new List<ProductSize>(); 
    }
}
