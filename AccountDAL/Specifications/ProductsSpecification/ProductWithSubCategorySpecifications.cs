using AccountDAL.Eentiti.CozaStore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountDAL.Specifications.ProductsSpecification
{
    public class ProductWithSubCategorySpecifications:BaseSpecification<Product>
    {
        public ProductWithSubCategorySpecifications(ProductSpecParams param)
            :base(p=> 
              (!param.SubCategorysId.HasValue || p.SubCategoryId == param.SubCategorysId.Value) &&
              (!param.CategoryId.HasValue || p.SubCategory.CategoryId == param.CategoryId.Value)&&
              (string.IsNullOrEmpty(param.Search) ||
                 p.Address_En.ToLower().Contains(param.Search.ToLower()) ||
                 p.Address_Ar.ToLower().Contains(param.Search.ToLower()))



            )
        {
            AddInclude(P => P.SubCategory);
            AddThenInclude(p => p.SubCategory, ps => ps.Category);
            AddInclude(P => P.ImageName);
            AddInclude(P => P.ProductSizes);
            AddThenInclude(p => p.ProductSizes, ps => ps.Size.SizeName);





            ApplyPagination(param.PageSize * (param.PageIndex - 1), param.PageSize);

            if (!string.IsNullOrEmpty(param.Sort))
            {
                switch (param.Sort)
                {
                    case "priceAsc":
                        AddOrderBy(P => P.BasePrice);
                        break;
                    case "priceDesc":
                        AddOrderByDescending(P => P.BasePrice);
                        break;
                    default:
                        AddOrderBy(P => P.Address_En);
                        break;
                }
            }

        }

    }
}
