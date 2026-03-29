using Application.Users.Contracts;
using Application.Shared.Contracts;
using Domain.Shared.Results;
using Domain.Shared.Abstractions;
using Domain.Shared.Errors;
using Domain.Shared.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Application.Shared.Services
{
    public abstract class DeleteService<TEntity>
        where TEntity : class, IEntity, ISoftDeletable
    {
        protected readonly IBaseRepository<TEntity> _repository;
        protected readonly ICurrentUserService _currentUserService;
        protected readonly IUnitOfWork _uow;
        public DeleteService(IBaseRepository<TEntity> repository, ICurrentUserService currentUserService, IUnitOfWork uow)
        {
            _repository = repository;
            _currentUserService = currentUserService;
            _uow = uow;
        }

        public virtual async Task<Result> SoftDeleteAsync(int id
            , CancellationToken cancellationToken)
        {
            if (id <= 0)
            {
                return Result.Failure(Error.InvalidId());
            }
            try
            {
                var entity = await _repository.FindAsync(e => e.Id == id
            , cancellationToken);
                if (entity is null)
                {
                    return Result.Failure(Error.NotFound(typeof(TEntity).Name));
                }
                if (entity.IsDeleted)
                {
                    string errorMessage = $"{typeof(TEntity).Name} is already deleted";
                    return Result.Failure(Error.Validation(errorMessage));
                }
                entity.IsDeleted = true;
                entity.DeletedAt = DateTime.UtcNow;
                entity.DeletedByUserId = _currentUserService.UserId;
                _repository.Update(entity);
                await _uow.SaveChangesAsync(cancellationToken);
                return Result.Success;
            }
            catch (Exception ex)
            {
                return Result.Failure($"Error:{ex.Message}");
            }
        }
        public virtual async Task<Result> SoftDeleteAsync(TEntity entity
                   , CancellationToken cancellationToken)
        {
            try
            {
                entity.IsDeleted = true;
                entity.DeletedAt = DateTime.UtcNow;
                entity.DeletedByUserId = _currentUserService.UserId;
                _repository.Update(entity);
                await _uow.SaveChangesAsync(cancellationToken);
                return Result.Success;
            }
            catch (DomainException ex)
            {
                return Result.Failure(Error.Conflict($"Domain Error:{ex.Message}"));
            }
            catch (Exception ex)
            {
                return Result.Failure($"Error:{ex.Message}");
            }
        }

    }
}
