using Application.Abstractions.Repositories.Base;
using Domain.Abstractions;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Base
{
    public class BaseRepository<TEntity> : IBaseRepository<TEntity> 
        where TEntity : class,IEntity
    {
        protected readonly InventoryManagmentDBContext _context;
        protected readonly DbSet<TEntity> _dbSet;

        public BaseRepository(InventoryManagmentDBContext context)
        {
            _context = context;
            _dbSet = _context.Set<TEntity>();
        }

        public void Add(TEntity entity)
        {
            _dbSet.Add(entity);
        }

        public async Task<TEntity?> FindAsync(Expression<Func<TEntity, bool>> predicate
            ,CancellationToken cancellationToken = default, string includeProperties = "")
        {
            IQueryable<TEntity> query = _dbSet;
            

            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            return await query.FirstOrDefaultAsync(predicate,cancellationToken);
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync(
            Expression<Func<TEntity, bool>> filter = null
            , CancellationToken cancellationToken = default
            , string includeProperties = ""
            )
        {
            IQueryable<TEntity> query = _dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            return await query.AsNoTracking().ToListAsync(cancellationToken);
        }

        public void Update(TEntity entity)
        {
            _dbSet.Update(entity);
        }

        public void Delete (TEntity entity)
        {
            _dbSet.Remove(entity);
        }

        public async Task<bool> IsExistAsync(Expression<Func<TEntity, bool>> predicate
            , CancellationToken cancellationToken)
        {

            if (predicate != null)
            {
                return await _dbSet.AnyAsync(predicate, cancellationToken);
            }
            return false;
        }
    }
}
