using AccountDAL.Eentiti.CozaStore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountDAL.Specifications.ProductsSpecification
{
    public class ProductWithFiltersForCountSpecification : BaseSpecification<Product>
    {
        public ProductWithFiltersForCountSpecification(ProductSpecParams param)
           : base(p =>
              (!param.SubCategorysId.HasValue || p.SubCategoryId == param.SubCategorysId.Value) &&
              (!param.CategoryId.HasValue || p.SubCategory.CategoryId == param.CategoryId.Value) &&
              (string.IsNullOrEmpty(param.Search) ||
                 p.Address_En.ToLower().Contains(param.Search.ToLower()) ||
                 p.Address_Ar.ToLower().Contains(param.Search.ToLower()))



            )
        {

        }
    }
}
