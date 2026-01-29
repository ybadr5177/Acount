using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountDAL.Eentiti.CozaStore
{
    public class SizeName : BaseEntity
    {
        public string? SizeNames { get; set; }

        public ICollection<DifferentSize> DifferentSize { get; set; }

    }
}
