using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AccountDAL.Specifications
{
    public class ThenIncludeChain<T, TProperty>/*: IThenIncludeChain<T>*/
    {
        //public Expression<Func<T, object>> RootInclude { get; set; }
        //public List<LambdaExpression> ThenIncludes { get; } = new();
        public Expression<Func<T, TProperty>> RootInclude { get; set; }
        public List<LambdaExpression> ThenIncludes { get; set; } = new();
    }
}
