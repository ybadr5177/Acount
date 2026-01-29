using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountDAL.Eentiti.CozaStore
{
    public class SliderItem:BaseEntity

    {
        public string? ImagePath { get; set; } 

        public string Type { get; set; } = ""; 

        public int? ProductId { get; set; } 

        public string? ExternalLink { get; set; } 

        public string? Title { get; set; }

        public string? Description { get; set; }
        public string? Title_AR { get; set; }

        public string? Description_AR { get; set; }
    }
}
