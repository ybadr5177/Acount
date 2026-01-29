using AccountDAL.Eentiti.CozaStore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountDAL.Eentiti
{
    public class BasketItem
    {
       
        public int Id { get; set; }
     

        public string ProductName { get; set; }
        public string ProductDescription { get; set; }

       
        public decimal Price { get; set; }
       

        public int Quantity { get; set; }

        public decimal BaseDiscountPrice { get; set; }
        public string PictureUrl { get; set; }
        

        public ICollection<ProductSize> ProductSizes { get; set; } = new List<ProductSize>();
        public string? ProductSize { get; set; }
        public string Size { get; set; }


        public string? CategoryorSubCategory { get; set; }
        
    }
}
