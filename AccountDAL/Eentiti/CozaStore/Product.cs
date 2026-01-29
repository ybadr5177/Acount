using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountDAL.Eentiti.CozaStore
{
    public class Product : BaseEntity
    {
        public string Address_En { get; set; }
        public string Address_Ar { get; set; }
        public string description_En { get; set; }
        public string description_Ar { get; set; }
        public SizeType Type { get; set; }
        public bool IsActive { get; set; } = true; //  If you want to show or hide the product(لو عايز اظهر او اخفي المنتج)

        public decimal? BasePrice { get; set; }
        public decimal? BaseDiscountPrice { get; set; }


        public int ProductImageId { get; set; }
        public ICollection<ProductImage> ImageName { get; set; }

        public ICollection<ProductSize> ProductSizes { get; set; } = new List<ProductSize>();
        public int ProductSizeId { get; set; }
        public int? SubCategoryId { get; set; }
        public SubCategory SubCategory { get; set; }

        public int? CategoryId { get; set; }   // لو المنتج مباشرة تحت فئة
        public Category Category { get; set; }


    }
}
