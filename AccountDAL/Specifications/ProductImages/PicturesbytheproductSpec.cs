using AccountDAL.Eentiti.CozaStore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountDAL.Specifications.ProductImages
{
    public class PicturesbytheproductSpec:BaseSpecification<ProductImage>
    {
        public PicturesbytheproductSpec(int id):base(p => p.ProductId == id)
        {
                    
        }
    }
}
