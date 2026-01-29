using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountDAL.Eentiti.CozaStore
{
    public class DimensionSize:BaseEntity
    {
        public decimal Width { get; set; }
        public decimal Height { get; set; }
        public decimal Depth { get; set; }

        public int DifferentSizeId { get; set; }
        public DifferentSize DifferentSize { get; set; }
    }
}
