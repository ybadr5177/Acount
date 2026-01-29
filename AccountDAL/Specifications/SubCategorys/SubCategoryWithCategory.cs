using AccountDAL.Eentiti.CozaStore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountDAL.Specifications.SubCategorys
{
    public class SubCategoryWithCategory:BaseSpecification<SubCategory>
    {
        public SubCategoryWithCategory(int Categoryid):base (subc=> subc.CategoryId== Categoryid)
        { 
            AddInclude(sub =>sub.Category);
        }
    }
}
