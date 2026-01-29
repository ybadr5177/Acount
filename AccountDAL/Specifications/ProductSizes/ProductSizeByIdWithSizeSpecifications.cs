using AccountDAL.Eentiti.CozaStore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountDAL.Specifications.ProductSizes
{
    public class ProductSizeByIdWithSizeSpecifications : BaseSpecification<ProductSize>
    {
        public ProductSizeByIdWithSizeSpecifications(int id):base(productSizeId=> productSizeId.Id== id)
        {
            AddInclude(x => x.Size);

        }
    }
}
