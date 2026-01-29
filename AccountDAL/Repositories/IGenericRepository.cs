using AccountDAL.Eentiti;
using AccountDAL.Eentiti.CozaStore;
using AccountDAL.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AccountDAL.Repositories
{
    public interface IGenericRepository<T> where T : BaseEntity
    {

        Task<T> GetFirstAsync();
        Task<T> GetByIdAsync(int id);
        Task<IReadOnlyList<T>> GetAllAsync();

        Task<T> GetByIdWithSpecAsync(ISpecification<T> spec);
        Task<IReadOnlyList<T>> GetAllWithSpecAsync(ISpecification<T> spec);

        Task<int> GetCountAsync(ISpecification<T> spec);

        Task<T> GetLatestAsync(Expression<Func<T, DateTime>> orderBy);

        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
        Task<T> findStringAsync(Expression<Func<T, string>> predicate,string value);

        Task AddAsync(T entity);

        void Update(T entity);

        void Delete(T entity);


        ///favorites
        Task<Favorites> GetFavoriteAsync(string userEmail, int productId);
        Task<IReadOnlyList<Favorites>> GetFavoritesByUserAsync(string userEmail);
        Task<bool> IsFavoriteAsync(string userEmail, int productId);
    }
}
