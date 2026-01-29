using AccountDAL.Eentiti.CozaStore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountDAL.Specifications.ProductsSpecification
{
    public class ProductWithProductSizeByIdSpecifications: BaseSpecification<Product>
    {
        public ProductWithProductSizeByIdSpecifications(int id):base(p=>p.Id == id)
        {
            AddInclude(P => P.ImageName);
            AddInclude(P => P.ProductSizes);
            AddThenInclude(p => p.ProductSizes, ps => ps.Size);
            
            

        }
    }
}
