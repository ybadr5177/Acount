using AccountDAL.Eentiti;
using AccountDAL.Specifications;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BAL
{
    public class SpecificationEvaluator<TEntity> where TEntity : BaseEntity
    {
        public static IQueryable<TEntity> GetQuery(IQueryable<TEntity> inputQuery, ISpecification<TEntity> spec)
        {
            var query = inputQuery;

            if (spec.Criteria != null)
            {
                query = query.Where(spec.Criteria);
            }


            if (spec.OrderBy != null)
            {
                query = query.OrderBy(spec.OrderBy);
            }


            if (spec.OrderByDescending != null)
            {
                query = query.OrderByDescending(spec.OrderByDescending);
            }

            if (spec.IsPaginationEnabled)
                query = query.Skip(spec.Skip).Take(spec.Take);

            //query = spec.Includes.Aggregate(query, (currentQuery, include) => currentQuery.Include(include));

            //foreach (var include in spec.Includes)
            //    query = include(query);


            //foreach (var chain in spec.ThenIncludes)
            //{
            //    var includeQuery = query.Include(chain.RootInclude);

            //    foreach (var then in chain.ThenIncludes)
            //    {
            //        var method = typeof(EntityFrameworkQueryableExtensions)
            //            .GetMethods()
            //            .FirstOrDefault(m =>
            //                m.Name == "ThenInclude" &&
            //                m.GetParameters().Length == 2 &&
            //                m.GetGenericArguments().Length == 3);

            //        if (method != null)
            //        {
            //            var genericMethod = method.MakeGenericMethod(
            //                typeof(TEntity),
            //                then.Parameters[0].Type,  // TPreviousProperty
            //                then.ReturnType           // TProperty
            //            );

            //            includeQuery = genericMethod.Invoke(
            //                null,
            //                new object[] { includeQuery, then }
            //            ) as IQueryable<TEntity>;
            //        }
            //    }

            //    query = includeQuery;
            //}

            foreach (var include in spec.Includes)
                query = query.Include(include);


            foreach (var obj in spec.ThenIncludes)
            {
                var baseType = obj.GetType();
                var root = (LambdaExpression)baseType.GetProperty("RootInclude").GetValue(obj);
                var list = (IEnumerable<LambdaExpression>)baseType.GetProperty("ThenIncludes").GetValue(obj);

                // النوع الأول من Include
                var rootReturnType = root.Type.GetGenericArguments()[1];
                var includeMethod = typeof(EntityFrameworkQueryableExtensions)
                    .GetMethods()
                    .First(m => m.Name == "Include" && m.GetParameters().Length == 2)
                    .MakeGenericMethod(typeof(TEntity), rootReturnType);

                var queryWithInclude = includeMethod.Invoke(null, new object[] { query, root });

                // محتاج تحتفظ بالنوع الحالي أثناء التكرار
                var currentType = rootReturnType;

                foreach (var then in list)
                {
                    var thenReturnType = then.Type.GetGenericArguments()[1];

                    // نتحقق هل currentType عبارة عن IEnumerable<T>
                    Type actualCurrentType = currentType;
                    if (typeof(System.Collections.IEnumerable).IsAssignableFrom(currentType) && currentType != typeof(string))
                    {
                        if (currentType.IsGenericType)
                            actualCurrentType = currentType.GetGenericArguments()[0];
                    }

                    // نحدد إذا العلاقة Collection ولا لا
                    bool isEnumerable = typeof(System.Collections.IEnumerable).IsAssignableFrom(currentType)
                                        && currentType != typeof(string);

                    MethodInfo thenIncludeMethod;
                    if (isEnumerable)
                    {
                        thenIncludeMethod = typeof(EntityFrameworkQueryableExtensions)
                            .GetMethods()
                            .Where(m => m.Name == "ThenInclude" && m.GetParameters().Length == 2)
                            .First(m =>
                                m.GetGenericArguments().Length == 3 &&
                                m.ToString().Contains("IEnumerable"));
                    }
                    else
                    {
                        thenIncludeMethod = typeof(EntityFrameworkQueryableExtensions)
                            .GetMethods()
                            .Where(m => m.Name == "ThenInclude" && m.GetParameters().Length == 2)
                            .First(m =>
                                m.GetGenericArguments().Length == 3 &&
                                !m.ToString().Contains("IEnumerable"));
                    }

                    // هنا نستخدم actualCurrentType بدل typeof(TEntity)
                    var genericThenInclude = thenIncludeMethod.MakeGenericMethod(typeof(TEntity), actualCurrentType, thenReturnType);

                    queryWithInclude = genericThenInclude.Invoke(null, new object[] { queryWithInclude, then });
                    currentType = thenReturnType;
                }

                query = (IQueryable<TEntity>)queryWithInclude;
            }

            // Apply ThenIncludes
            //foreach (var obj in spec.ThenIncludes)
            //{
            //    var baseType = obj.GetType();
            //    var root = (LambdaExpression)baseType.GetProperty("RootInclude").GetValue(obj);
            //    var list = (IEnumerable<LambdaExpression>)baseType.GetProperty("ThenIncludes").GetValue(obj);
            //    var returnType = root.Type.GetGenericArguments()[1];

            //    var includeMethod = typeof(EntityFrameworkQueryableExtensions)
            //        .GetMethods()
            //        .First(m => m.Name == "Include" && m.GetParameters().Length == 2)
            //        .MakeGenericMethod(typeof(TEntity), returnType);

            //    var queryWithInclude = includeMethod.Invoke(null, new object[] { query, root });

            //    foreach (var then in list)
            //    {
            //        var thenReturnType = then.Type.GetGenericArguments()[1];
            //        bool isEnumerable = typeof(System.Collections.IEnumerable).IsAssignableFrom(returnType)
            //               && returnType != typeof(string);
            //        MethodInfo thenIncludeMethod;
            //        //var thenIncludeMethod = typeof(EntityFrameworkQueryableExtensions)
            //        //    .GetMethods()
            //        //    .First(m =>
            //        //        m.Name == "ThenInclude"
            //        //        && m.GetParameters().Length == 2
            //        //        && m.GetGenericArguments().Length == 3)
            //        //    .MakeGenericMethod(
            //        //        typeof(TEntity),
            //        //        returnType,
            //        //        thenReturnType
            //        //    );
            //        if (isEnumerable)
            //        {
            //            // إذا كانت العلاقة Collection
            //            thenIncludeMethod = typeof(EntityFrameworkQueryableExtensions)
            //                .GetMethods()
            //                .Where(m => m.Name == "ThenInclude" && m.GetParameters().Length == 2)
            //                .First(m =>
            //                    m.GetGenericArguments().Length == 3 &&
            //                    m.ToString().Contains("IEnumerable"));
            //        }
            //        else
            //        {
            //            // إذا كانت العلاقة عادية
            //            thenIncludeMethod = typeof(EntityFrameworkQueryableExtensions)
            //                .GetMethods()
            //                .Where(m => m.Name == "ThenInclude" && m.GetParameters().Length == 2)
            //                .First(m =>
            //                    m.GetGenericArguments().Length == 3 &&
            //                    !m.ToString().Contains("IEnumerable"));
            //        }
            //        var genericThenInclude = thenIncludeMethod.MakeGenericMethod( typeof(TEntity),  returnType,thenReturnType);

            //        queryWithInclude = genericThenInclude.Invoke(null, new object[] { queryWithInclude, then });
            //        returnType = thenReturnType;
            //    }


            //query = (IQueryable<TEntity>)queryWithInclude;
            //}

            return query;
        }
    }
}
