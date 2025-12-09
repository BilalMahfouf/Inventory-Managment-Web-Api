using Application.PagedLists;
using Domain.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Application.Abstractions.Repositories.Base
{
    public interface IBaseRepository<TEntity> where TEntity : class
    {
        Task<IEnumerable<TEntity>> GetAllAsync(
            Expression<Func<TEntity, bool>> filter = null,
            CancellationToken cancellationToken = default ,string includeProperties = "");
        Task<TEntity?> FindAsync(Expression<Func<TEntity, bool>> predicate
           , CancellationToken cancellationToken = default, string includeProperties = "");
        void Add(TEntity entity);
        void Update(TEntity entity);
        void Delete(TEntity entity);

        Task<bool> IsExistAsync(Expression<Func<TEntity, bool>> predicate
            ,CancellationToken cancellationToken = default);

        IQueryable<TEntity> AsQueryable();

        Task<int> GetCountAsync(
           Expression<Func<TEntity, bool>>? filter = null
           , CancellationToken cancellationToken = default);
        Task<decimal> SumAsync(
           Expression<Func<TEntity, decimal>> selector
           , Expression<Func<TEntity, bool>>? filter = null
           , CancellationToken cancellationToken = default);
        Task<IEnumerable<TEntity>> GetAllWithPaginationAsync(
            int page=1,
            int pageSize=10,
            CancellationToken cancellationToken = default,
            string includeProperties = ""); 
    }
}
