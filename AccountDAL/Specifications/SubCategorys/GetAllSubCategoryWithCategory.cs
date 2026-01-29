using AccountDAL.Eentiti.CozaStore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountDAL.Specifications.SubCategorys
{
    public class GetAllSubCategoryWithCategory:BaseSpecification<SubCategory>
    {
        public GetAllSubCategoryWithCategory()
        {
            AddInclude(sub => sub.Category);
        }
    }
}
