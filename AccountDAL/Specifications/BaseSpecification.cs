using AccountDAL.Eentiti;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AccountDAL.Specifications
{
    public class BaseSpecification<T> : ISpecification<T>
    {
        public Expression<Func<T, bool>> Criteria { get; set; }
        public List<Expression<Func<T, object>>> Includes { get; set; } = new List<Expression<Func<T, object>>>();
        //public List<IThenIncludeChain<T>> ThenIncludes { get; } = new();
        //public List<Func<IQueryable<T>, IIncludableQueryable<T, object>>> Includes { get; } = new();
        public List<object> ThenIncludes { get; } = new();
        //public List<ThenIncludeChain<T>> ThenIncludes { get; } = new();

        public Expression<Func<T, object>> OrderBy { get; set; }
        public Expression<Func<T, object>> OrderByDescending { get; set; }

        public int Take { get; set; }
        public int Skip { get; set; }
        public bool IsPaginationEnabled { get; set; }

        public BaseSpecification()
        {

        }

        public BaseSpecification(Expression<Func<T, bool>> Criteria)
        {
            this.Criteria = Criteria;
        }

        public void AddInclude(Expression<Func<T, object>> includeExpression)
        {
            Includes.Add(includeExpression);
        }
        // public void AddThenInclude<TPreviousProperty>( Expression<Func<T, object>> rootInclude,
        //Expression<Func<TPreviousProperty, object>> thenInclude)
        // {
        //     var existing = ThenIncludes.FirstOrDefault(x => x.RootInclude.ToString() == rootInclude.ToString());

        //     if (existing == null)
        //     {
        //         existing = new ThenIncludeChain<T> { RootInclude = rootInclude };
        //         ThenIncludes.Add(existing);
        //     }

        //     existing.ThenIncludes.Add(thenInclude);
        // }
        public void AddThenInclude<TProperty, TNext>(
        Expression<Func<T, TProperty>> rootInclude,
        Expression<Func<TProperty, TNext>> thenInclude)
        {
            ThenIncludes.Add(new ThenIncludeChain<T, TProperty>
            {
                RootInclude = rootInclude,
                ThenIncludes = new List<LambdaExpression> { thenInclude }
            });
        }
        public void AddThenInclude<TCollectionItem, TNext>(
        Expression<Func<T, IEnumerable<TCollectionItem>>> rootInclude,
        Expression<Func<TCollectionItem, TNext>> thenInclude)
        {
            ThenIncludes.Add(new ThenIncludeChain<T, IEnumerable<TCollectionItem>>
            {
                RootInclude = rootInclude,
                ThenIncludes = new List<LambdaExpression> { thenInclude }
            });
        }

        //public void AddThenInclude<TPreviousProperty>(Expression<Func<T, object>> rootInclude, Expression<Func<TPreviousProperty, object>> thenInclude)
        //{
        //    var existing = ThenIncludes.FirstOrDefault(x => x.RootInclude.ToString() == rootInclude.ToString());

        //    if (existing == null)
        //    {
        //        existing = new ThenIncludeChain<T> { RootInclude = rootInclude };
        //        ThenIncludes.Add(existing);
        //    }

        //    existing.ThenIncludes.Add(thenInclude);
        //}
        //public void AddInclude(Func<IQueryable<T>, IIncludableQueryable<T, object>> includeExpression)
        //{
        //    Includes.Add(includeExpression);
        //}

        public void AddOrderBy(Expression<Func<T, object>> orderByExpression)
        {
            OrderBy = orderByExpression;
        }
        public void AddOrderByDescending(Expression<Func<T, object>> orderByDescExpression)
        {
            OrderByDescending = orderByDescExpression;
        }

        public void ApplyPagination(int skip, int take)
        {
            IsPaginationEnabled = true;
            Skip = skip;
            Take = take;
        }
    }
}
