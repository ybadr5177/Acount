using AccountDAL.config;
using AccountDAL.Eentiti;
using AccountDAL.Eentiti.CozaStore;
using AccountDAL.Repositories;
using AccountDAL.Specifications;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BAL
{
    public class GenericRepository<T>:IGenericRepository<T> where T : BaseEntity
    {
        private readonly CozaStoreContext _context;

        public GenericRepository(CozaStoreContext context)
        {
            _context = context;
        }
        public async Task<IReadOnlyList<T>> GetAllAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }

        public async Task<T> GetByIdAsync(int id)
            => await _context.Set<T>().FindAsync(id);

        public async Task<IReadOnlyList<T>> GetAllWithSpecAsync(ISpecification<T> spec)
            => await ApplySpecification(spec).ToListAsync();


        public async Task<T> GetByIdWithSpecAsync(ISpecification<T> spec)
           => await ApplySpecification(spec).FirstOrDefaultAsync();


        public async Task<int> GetCountAsync(ISpecification<T> spec)
            => await ApplySpecification(spec).CountAsync();

        private IQueryable<T> ApplySpecification(ISpecification<T> spec)
        {
            return SpecificationEvaluator<T>.GetQuery(_context.Set<T>().AsQueryable(), spec);
        }

        public async Task AddAsync(T entity)
        => await _context.Set<T>().AddAsync(entity);

        public void Update(T entity)
        => _context.Set<T>().Update(entity);

        public void Delete(T entity)
        => _context.Set<T>().Remove(entity);

        public Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
       => _context.Set<T>().AnyAsync(predicate);

        public Task<T> GetLatestAsync(Expression<Func<T, DateTime>> orderBy)
      => _context.Set<T>().OrderByDescending(orderBy) .FirstOrDefaultAsync();

        public async Task<T> GetFirstAsync()
        =>await _context.Set<T>().FirstOrDefaultAsync();


      


        ///favorites
        public async Task<Favorites> GetFavoriteAsync(string userEmail, int productId)
        {
            return await _context.Favorites
                .FirstOrDefaultAsync(f => f.UserEmail == userEmail && f.ProductId == productId);
        }

        public async Task<IReadOnlyList<Favorites>> GetFavoritesByUserAsync(string userEmail)
        {
            return await _context.Favorites
                .Where(f => f.UserEmail == userEmail)
                .ToListAsync();
        }

        public async Task<bool> IsFavoriteAsync(string userEmail, int productId)
        {
            return await _context.Favorites
                .AnyAsync(f => f.UserEmail == userEmail && f.ProductId == productId);
        }

        public async Task<T> findStringAsync(Expression<Func<T, string>> predicate, string value)
        {
            return await _context.Set<T>()
               .FirstOrDefaultAsync(e => EF.Property<string>(e, ((MemberExpression)predicate.Body).Member.Name) == value);
        }
    }
}
