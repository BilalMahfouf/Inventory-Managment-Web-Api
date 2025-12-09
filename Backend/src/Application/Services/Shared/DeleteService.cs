using Application.Abstractions.Repositories.Base;
using Application.Abstractions.Services.User;
using Application.Abstractions.UnitOfWork;
using Application.Results;
using Domain.Abstractions;
using Domain.Enums;
using Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Shared
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
                return Result.InvalidId();
            }
            try
            {
                var entity = await _repository.FindAsync(e => e.Id == id
            , cancellationToken);
                if (entity is null)
                {
                    return Result.NotFound(typeof(TEntity).Name);
                }
                if (entity.IsDeleted)
                {
                    string errorMessage = $"{typeof(TEntity).Name} is already deleted";
                    return Result.Failure(errorMessage, ErrorType.BadRequest);
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
                return Result.Failure($"Error:{ex.Message}"
                    , ErrorType.InternalServerError);
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
                return Result.Failure($"Domain Error:{ex.Message}"
                    , ErrorType.Conflict);
            }
            catch (Exception ex)
            {
                return Result.Failure($"Error:{ex.Message}"
                    , ErrorType.InternalServerError);
            }
        }

    }
}
