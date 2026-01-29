using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AccountDAL.Specifications
{
    public interface ISpecification<T>
    {
        Expression<Func<T, bool>> Criteria { get; set; }

        List<Expression<Func<T, object>>> Includes { get; set; }
        //List<IThenIncludeChain<T>> ThenIncludes { get; }
        //List<Func<IQueryable<T>, IIncludableQueryable<T, object>>> Includes { get; }
        //List<ThenIncludeChain<T>> ThenIncludes { get; }
        List<object> ThenIncludes { get; }

        public Expression<Func<T, object>> OrderBy { get; set; }
        public Expression<Func<T, object>> OrderByDescending { get; set; }

        public int Take { get; set; }
        public int Skip { get; set; }
        public bool IsPaginationEnabled { get; set; }
        //void AddInclude(Expression<Func<T, object>> includeExpression);
        void AddInclude(Expression<Func<T, object>> includeExpression);
        //void AddThenInclude<TPreviousProperty>(
        //    Expression<Func<T, object>> rootInclude,
        //    Expression<Func<TPreviousProperty, object>> thenInclude);
        void AddThenInclude<TProperty, TNext>(
      Expression<Func<T, TProperty>> rootInclude,
      Expression<Func<TProperty, TNext>> thenInclude);
        void AddThenInclude<TCollectionItem, TNext>(
       Expression<Func<T, IEnumerable<TCollectionItem>>> rootInclude,
       Expression<Func<TCollectionItem, TNext>> thenInclude);
    }
}
