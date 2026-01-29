using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountDAL.Eentiti.CozaStore
{
    public class ProductSize:BaseEntity
    {
        public int ProductId { get; set; }
        public Product Product { get; set; }

        public int DifferentSizeId { get; set; }
        public DifferentSize Size { get; set; }

        public bool IsActive { get; set; } = true;
        public decimal Price { get; set; }
        public decimal DiscountPrice { get; set; }
        public int Stock { get; set; }
    }
}
