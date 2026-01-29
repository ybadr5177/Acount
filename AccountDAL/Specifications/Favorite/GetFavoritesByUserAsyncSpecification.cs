using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AccountDAL.Eentiti;

namespace AccountDAL.Specifications.Favorite
{
    public class GetFavoritesByUserAsyncSpecification:BaseSpecification<Favorites>
    {
        public GetFavoritesByUserAsyncSpecification(string userName) : base(p => p.UserEmail == userName)
        {
            AddInclude(t => t.Product);
            AddThenInclude(P => P.Product,ps => ps.ImageName);
            AddThenInclude(P => P.Product, pz=> pz.ProductSizes);
            AddThenInclude(po => po.Product.ProductSizes, ps => ps.Size);
        }
    }
}
