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
            string includeProperties = "",CancellationToken cancellationToken=default);
        Task<TEntity?> FindAsync(Expression<Func<TEntity, bool>> predicate
            , string includeProperties = "", CancellationToken cancellationToken = default);
        void Add(TEntity entity);
        void Update(TEntity entity);
    }
}
