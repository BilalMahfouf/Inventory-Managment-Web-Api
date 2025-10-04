using Application.Abstractions.Repositories.Base;
using Application.Abstractions.Services.User;
using Application.Abstractions.UnitOfWork;
using Application.DTOs.UnitOfMeasure;
using Application.Results;
using Application.Services.Shared;
using Domain.Entities;
using Domain.Enums;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.UnitOfMeasures
{
    public class UnitOfMeasureService : DeleteService<UnitOfMeasure>
    {
        private readonly IValidator<UnitOfMeasureRequest> _validator;
        public UnitOfMeasureService(IBaseRepository<UnitOfMeasure> repository
            , ICurrentUserService currentUserService
            , IUnitOfWork uow
            , IValidator<UnitOfMeasureRequest> validator)
            : base(repository, currentUserService, uow)
        {
            _validator = validator;
        }

        private UnitOfMeasureReadResponse Map(UnitOfMeasure entity)
        {
            var result = new UnitOfMeasureReadResponse()
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,

                CreatedAt = entity.CreatedAt,
                CreatedByUserId = entity.CreatedByUserId,
                CreatedByUserName = entity.CreatedByUser?.UserName,

                UpdatedAt = entity.UpdatedAt,
                UpdatedByUserId = entity.UpdatedByUserId,
                UpdatedByUserName = entity.UpdatedByUser?.UserName,

                IsDeleted = entity.IsDeleted,
                DeleteAt = entity.DeletedAt,
                DeletedByUserId = entity.DeletedByUserId,
                DeletedByUserName = entity.DeletedByUser?.UserName

            };
            return result;
        }

        public async Task<Result<IReadOnlyCollection<UnitOfMeasureReadResponse>>>
            GetAllAsync(CancellationToken cancellationToken)
        {
            try
            {
                var unitOfMeasures = await _repository.GetAllAsync(null!, cancellationToken
                    , "CreatedByUser,UpdatedByUser,DeletedByUser");
                if (unitOfMeasures is null || !unitOfMeasures.Any())
                {
                    return Result<IReadOnlyCollection<UnitOfMeasureReadResponse>>
                        .NotFound(nameof(unitOfMeasures));
                }
                var result = new List<UnitOfMeasureReadResponse>();
                foreach (var u in unitOfMeasures)
                {
                    var uResponse = Map(u);
                    result.Add(uResponse);
                }
                return Result<IReadOnlyCollection<UnitOfMeasureReadResponse>>
                    .Success(result);
            }
            catch (Exception ex)
            {
                return Result<IReadOnlyCollection<UnitOfMeasureReadResponse>>
                    .Failure($"Error:{ex.Message}"
                    , ErrorType.InternalServerError);
            }
        }

        public async Task<Result<UnitOfMeasureReadResponse>> FindAsync(int id
            , CancellationToken cancellationToken)
        {
            if (id <= 0)
            {
                return Result<UnitOfMeasureReadResponse>.InvalidId();
            }
            try
            {
                var unitOfMeasure = await _repository.FindAsync(u => u.Id == id
                , cancellationToken, "CreatedByUser,UpdatedByUser,DeletedByUser");
                if (unitOfMeasure is null)
                {
                    return Result<UnitOfMeasureReadResponse>
                        .NotFound(nameof(unitOfMeasure));
                }
                var result = Map(unitOfMeasure);
                return Result<UnitOfMeasureReadResponse>.Success(result);
            }
            catch (Exception ex)
            {
                return Result<UnitOfMeasureReadResponse>
                    .Failure($"Error:{ex.Message}"
                    , ErrorType.InternalServerError);
            }
        }

        public async Task<Result<UnitOfMeasureReadResponse>> AddAsync(
            UnitOfMeasureRequest request
            , CancellationToken cancellationToken)
        {
            var validationResult = _validator.Validate(request);
            if (!validationResult.IsValid)
            {
                var errors = string.Join("; "
                    , validationResult.Errors.Select(e => e.ErrorMessage));
                return Result<UnitOfMeasureReadResponse>
                    .Failure(errors, ErrorType.BadRequest);
            }
            try
            {
                var isExist = await _repository.IsExistAsync(u => u.Name == request.Name
                , cancellationToken);
                if (isExist)
                {
                    string errorMessage = $"{nameof(UnitOfMeasure)} with name {request.Name} already exists";
                    return Result<UnitOfMeasureReadResponse>
                        .Failure(errorMessage, ErrorType.Conflict);
                }
                var entity = new UnitOfMeasure()
                {
                    Name = request.Name,
                    Description = request.Description,
                    CreatedAt = DateTime.UtcNow,
                    CreatedByUserId = _currentUserService.UserId,
                    IsDeleted = false
                };
                _repository.Add(entity);
                await _uow.SaveChangesAsync(cancellationToken);
                return await FindAsync(entity.Id, cancellationToken);
            }
            catch (Exception ex)
            {
                return Result<UnitOfMeasureReadResponse>
                    .Failure($"Error:{ex.Message}"
                    , ErrorType.InternalServerError);
            }
        }

        public async Task<Result<UnitOfMeasureReadResponse>> UpdateAsync(int id
            , UnitOfMeasureRequest request
            , CancellationToken cancellationToken)
        {
            if (id <= 0)
            {
                return Result<UnitOfMeasureReadResponse>.InvalidId();
            }
            var validationResult = _validator.Validate(request);
            if (!validationResult.IsValid)
            {
                var errors = string.Join("; "
                    , validationResult.Errors.Select(e => e.ErrorMessage));
                return Result<UnitOfMeasureReadResponse>
                    .Failure(errors, ErrorType.BadRequest);
            }
            try
            {
                var entity = await _repository.FindAsync(u => u.Id == id
                , cancellationToken);
                if (entity is null)
                {
                    return Result<UnitOfMeasureReadResponse>
                        .NotFound(nameof(entity));
                }
                if (entity.IsDeleted)
                {
                    string errorMessage = $"{nameof(UnitOfMeasure)} is deleted";
                    return Result<UnitOfMeasureReadResponse>
                        .Failure(errorMessage, ErrorType.BadRequest);
                }
                // check if another entity with the same name exists
                var isExist = await _repository.IsExistAsync(u => u.Name == request.Name
                && u.Id != id, cancellationToken);
                if (isExist)
                {
                    string errorMessage = $"{nameof(UnitOfMeasure)} with name {request.Name} already exists";
                    return Result<UnitOfMeasureReadResponse>
                        .Failure(errorMessage, ErrorType.Conflict);
                }

                entity.Name = request.Name;
                entity.Description = request.Description;
                entity.UpdatedAt = DateTime.UtcNow;
                entity.UpdatedByUserId = _currentUserService.UserId;
                _repository.Update(entity);
                await _uow.SaveChangesAsync(cancellationToken);
                return await FindAsync(entity.Id, cancellationToken);
            }
            catch (Exception ex)
            {
                return Result<UnitOfMeasureReadResponse>
                    .Failure($"Error:{ex.Message}"
                    , ErrorType.InternalServerError);
            }
        }

        public async Task<Result<IEnumerable<object>>>
            GetUnitsNamesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var unitsNames = await _repository.GetAllAsync(
                    e => !e.IsDeleted, cancellationToken: cancellationToken);
                if (unitsNames is null || !unitsNames.Any())
                {
                    return Result<IEnumerable<object>>.NotFound("Units of Measure");
                }
                var result = unitsNames.Select(e=> new
                {
                    e.Id,
                    e.Name
                });
                if( result is null || !result.Any())

                {
                    return Result<IEnumerable<object>>.NotFound("Units of Measure names");
                }
                return Result<IEnumerable<object>>.Success(result);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<object>>.Exception(nameof(GetUnitsNamesAsync), ex);
            }



        }
    }
}
