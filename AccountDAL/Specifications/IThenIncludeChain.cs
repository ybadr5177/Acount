using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AccountDAL.Specifications
{
    public interface IThenIncludeChain<T>
    {
        Expression<Func<T, object>> RootInclude { get; set; }
        List<LambdaExpression> ThenIncludes { get; }
    }
}
