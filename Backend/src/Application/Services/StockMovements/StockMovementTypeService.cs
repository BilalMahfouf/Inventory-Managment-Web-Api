using Application.Abstractions.Services.User;
using Application.Abstractions.UnitOfWork;
using Application.DTOs.StockMovements.Request;
using Application.DTOs.StockMovements.Response;
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

namespace Application.Services.StockMovements
{
    public class StockMovementTypeService : DeleteService<StockMovementType>
    {
        private readonly IValidator<StockMovementTypeRequest> _validator;

        public StockMovementTypeService(
            IUnitOfWork uow
            , IValidator<StockMovementTypeRequest> validator
            , ICurrentUserService currentUserService)
            :base(uow.StockMovementTypes,currentUserService,uow)
        {
            _validator = validator;
        }

        public  StockMovementTypeReadResponse Map(StockMovementType stockMovementType)
        {
            return new StockMovementTypeReadResponse()
            {
                Id = stockMovementType.Id,
                Name = stockMovementType.Name,
                Description = stockMovementType.Description,
                Direction = nameof(stockMovementType.Direction),
                CreatedAt = stockMovementType.CreatedAt,
                CreatedByUserId = stockMovementType.CreatedByUserId,
                CreatedByUserName = stockMovementType.CreatedByUser?.UserName,
                IsDeleted = stockMovementType.IsDeleted,
                DeleteAt = stockMovementType.DeletedAt,
                DeletedByUserId = stockMovementType.DeletedByUserId,
                DeletedByUserName = stockMovementType.DeletedByUser?.UserName

            };
        }

        public async Task<Result<IEnumerable<StockMovementTypeReadResponse>>>
            GetAllAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var stockMovementTypes = await _uow.StockMovementTypes.GetAllAsync(
                    cancellationToken: cancellationToken
                    , includeProperties: "CreatedByUser,DeletedByUser");
                if (stockMovementTypes is null || !stockMovementTypes.Any())
                {
                    return Result<IEnumerable<StockMovementTypeReadResponse>>
                        .NotFound(nameof(stockMovementTypes));
                }
                var response = stockMovementTypes.Select(sm => Map(sm));
                return Result<IEnumerable<StockMovementTypeReadResponse>>
                    .Success(response);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<StockMovementTypeReadResponse>>
                    .Exception(nameof(GetAllAsync), ex);
            }
        }

        public async Task<Result<StockMovementTypeReadResponse>> FindAsync(int id
            , CancellationToken cancellationToken = default)
        {
            if (id <= 0)
            {
                return Result<StockMovementTypeReadResponse>.InvalidId();
            }
            try
            {
                var smt = await _uow.StockMovementTypes.FindAsync(e => e.Id == id
                    , cancellationToken: cancellationToken
                    , "CreatedByUser,DeletedByUser");
                if (smt is null)
                {
                    return Result<StockMovementTypeReadResponse>
                        .NotFound("Stock Movement Type");
                }
                return Result<StockMovementTypeReadResponse>
                    .Success(Map(smt));
            }
            catch (Exception ex)
            {
                return Result<StockMovementTypeReadResponse>
                    .Exception(nameof(FindAsync), ex);
            }
        }

        public async Task<Result<StockMovementTypeReadResponse>> AddAsync(
            StockMovementTypeRequest request
            , CancellationToken cancellationToken = default)
        {
            var validationResult = await _validator.ValidateAsync(request
                , cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
                return Result<StockMovementTypeReadResponse>
                    .Failure(errors, ErrorType.BadRequest);
            }
            try
            {
                var newSmt = new StockMovementType()
                {
                    Name = request.Name,
                    Description = request.Description,
                    Direction = (StockMovementDirection)request.Direction,
                    IsDeleted = false
                };
                _uow.StockMovementTypes.Add(newSmt);
                await _uow.SaveChangesAsync(cancellationToken);
                return await FindAsync(newSmt.Id, cancellationToken);

            }
            catch (Exception ex)
            {
                return Result<StockMovementTypeReadResponse>
                    .Exception(nameof(AddAsync), ex);
            }
        }

        public async Task<Result<StockMovementTypeReadResponse>> UpdateAsync(int id
            , StockMovementTypeRequest request
            , CancellationToken cancellationToken = default)
        {
            if (id <= 0)
            {
                return Result<StockMovementTypeReadResponse>.InvalidId();
            }
            var validationResult = await _validator.ValidateAsync(request
                , cancellationToken);
            if (!validationResult.IsValid)
            {
                var errors = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
                return Result<StockMovementTypeReadResponse>
                    .Failure(errors, ErrorType.BadRequest);
            }
            try
            {
                var isExists = await _uow.StockMovementTypes
                    .IsExistAsync(e => e.Id != id && e.Name == request.Name
                    , cancellationToken: cancellationToken);
                if (isExists)
                {
                    return Result<StockMovementTypeReadResponse>
                        .Failure("Stock Movement Type with the same name already exists"
                        , ErrorType.Conflict);
                }
                var existingSmt = await _uow.StockMovementTypes.FindAsync(e => e.Id == id
                    , cancellationToken: cancellationToken);
                if (existingSmt is null)
                {
                    return Result<StockMovementTypeReadResponse>
                        .NotFound("Stock Movement Type");
                }
                existingSmt.Name = request.Name;
                existingSmt.Description = request.Description;
                existingSmt.Direction = (StockMovementDirection) request.Direction;

                _uow.StockMovementTypes.Update(existingSmt);
                await _uow.SaveChangesAsync(cancellationToken);
                return await FindAsync(existingSmt.Id, cancellationToken);
            }
            catch (Exception ex)
            {
                return Result<StockMovementTypeReadResponse>
                    .Exception(nameof(UpdateAsync), ex);
            }
        }
    }
}
