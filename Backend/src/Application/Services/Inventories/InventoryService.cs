using Application.Abstractions.Repositories.Base;
using Application.Abstractions.Services.User;
using Application.Abstractions.UnitOfWork;
using Application.DTOs.Inventories;
using Application.DTOs.Inventories.Request;
using Application.Helpers.Util;
using Application.Results;
using Application.Services.Shared;
using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Inventories
{
    public class InventoryService : DeleteService<Inventory>
    {
        private readonly IValidator<InventoryCreateRequest> _createValidator;
        private readonly IValidator<InventoryUpdateRequest> _updateValidator;

        public InventoryService(IUnitOfWork uow,
             IValidator<InventoryCreateRequest> createValidator,
             IValidator<InventoryUpdateRequest> updateValidator,
             ICurrentUserService currentUserService
            )
            : base(uow.Inventories, currentUserService, uow)
        {
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }


        public async Task<Result<IReadOnlyCollection<InventoryBaseReadResponse>>>
            GetAllAsync(CancellationToken cancellationToken = default)
        {
            try
            {

                var inventories = await _uow.Inventories.GetAllAsync(
                    cancellationToken: cancellationToken
                    , includeProperties: "Product,Location");
                if (inventories is null || !inventories.Any())
                {
                    return Result<IReadOnlyCollection<InventoryBaseReadResponse>>
                        .NotFound("Inventories");
                }
                var response = inventories.Select(i => Utility.Map(i))
                    .ToList()
                    .AsReadOnly();
                return Result<IReadOnlyCollection<InventoryBaseReadResponse>>
                    .Success(response);
            }
            catch (Exception ex)
            {
                return Result<IReadOnlyCollection<InventoryBaseReadResponse>>
                        .Exception(nameof(GetAllAsync), ex);
            }
        }

        public async Task<Result<InventoryBaseReadResponse>>
            FindAsync(int id, CancellationToken cancellationToken = default)
        {
            try
            {
                if (id <= 0)
                {
                    return Result<InventoryBaseReadResponse>.InvalidId();
                }
                var inventory = await _uow.Inventories.FindAsync(
                    predicate: i => i.Id == id,
                   includeProperties: "Product,Location",
                    cancellationToken: cancellationToken);
                if (inventory is null)
                {
                    return Result<InventoryBaseReadResponse>
                        .NotFound("Inventory");
                }
                var response = Utility.Map(inventory);
                return Result<InventoryBaseReadResponse>
                    .Success(response);
            }
            catch (Exception ex)
            {
                return Result<InventoryBaseReadResponse>
                        .Exception(nameof(FindAsync), ex);
            }
        }

        public async Task<Result<InventoryBaseReadResponse>>
            CreateAsync(InventoryCreateRequest request
            , CancellationToken cancellationToken = default)
        {
            try
            {
                var validationResult = await _createValidator
                    .ValidateAsync(request, cancellationToken);
                if (!validationResult.IsValid)
                {
                    var errors = string.Join("; ", validationResult.Errors
                        .Select(e => e.ErrorMessage));
                    return Result<InventoryBaseReadResponse>
                        .Failure(errors, ErrorType.BadRequest);
                }
                var isExist = await _uow.Inventories
                    .IsExistAsync(i => i.ProductId == request.ProductId
                    && i.LocationId == request.LocationId
                    , cancellationToken);
                if (isExist)
                {
                    return Result<InventoryBaseReadResponse>
                        .Failure(@"Inventory with the same ProductId 
                        and LocationId already exists"
                        , ErrorType.Conflict);
                }
                var product = await _uow.Products
                    .FindAsync(p => p.Id == request.ProductId
                    , cancellationToken: cancellationToken);
                if (product is null)
                {
                    return Result<InventoryBaseReadResponse>
                        .NotFound("Product");
                }
                var newInventory = Inventory.Create(
                    product,
                    request.LocationId,
                    request.QuantityOnHand,
                    request.ReorderLevel,
                    request.MaxLevel);

                _uow.Inventories.Add(newInventory);
                await _uow.SaveChangesAsync(cancellationToken);
                return await FindAsync(newInventory.Id, cancellationToken);
            }
            catch (DomainException ex)
            {
                return Result<InventoryBaseReadResponse>
                        .Failure(ex.Message, ErrorType.Conflict);
            }
            catch (Exception ex)
            {
                return Result<InventoryBaseReadResponse>
                        .Exception(nameof(CreateAsync), ex);
            }

        }

        public async Task<Result<InventoryBaseReadResponse>>
            UpdateAsync(int id, InventoryUpdateRequest request
            , CancellationToken cancellationToken = default)
        {
            try
            {
                if (id <= 0)
                {
                    return Result<InventoryBaseReadResponse>.InvalidId();
                }
                var validationResult = await _updateValidator
                    .ValidateAsync(request, cancellationToken);
                if (!validationResult.IsValid)
                {
                    var errors = string.Join("; ", validationResult.Errors
                        .Select(e => e.ErrorMessage));
                    return Result<InventoryBaseReadResponse>
                        .Failure(errors, ErrorType.BadRequest);
                }
                var existingInventory = await _uow.Inventories
                    .FindAsync(i => i.Id == id,
                    cancellationToken: cancellationToken,
                    includeProperties: "Product");
                if (existingInventory is null)
                {
                    return Result<InventoryBaseReadResponse>
                        .NotFound("Inventory");
                }

                existingInventory.UpdateInventoryLevels(
                    request.QuantityOnHand,
                    request.ReorderLevel,
                    request.MaxLevel);

                _uow.Inventories.Update(existingInventory);
                await _uow.SaveChangesAsync(cancellationToken);
                return await FindAsync(existingInventory.Id, cancellationToken);
            }
            catch (DomainException ex)
            {
                return Result<InventoryBaseReadResponse>
                        .Failure(ex.Message, ErrorType.Conflict);
            }
            catch (Exception ex)
            {
                return Result<InventoryBaseReadResponse>
                        .Exception(nameof(UpdateAsync), ex);
            }
        }

        public async Task<Result> DeleteAsync(int id
            , CancellationToken cancellationToken = default)
        {
            try
            {
                if (id <= 0)
                {
                    return Result.InvalidId();
                }
                var existingInventory = await _uow.Inventories
                    .FindAsync(i => i.Id == id, cancellationToken: cancellationToken);
                if (existingInventory is null)
                {
                    return Result.NotFound("Inventory");
                }
                _uow.Inventories.Delete(existingInventory);
                await _uow.SaveChangesAsync(cancellationToken);
                return Result.Success;
            }
            catch (Exception ex)
            {
                return Result.Exception(nameof(DeleteAsync), ex);
            }
        }

        public async Task<Result<IReadOnlyCollection<InventoryBaseReadResponse>>>
            GetInventoryLowStockAsync(
            CancellationToken cancellationToken = default)
        {
            try
            {
                var inventories = await _uow.Inventories.GetAllAsync(
                    filter: i => i.QuantityOnHand <= i.ReorderLevel,
                    includeProperties: "Product,Location",
                    cancellationToken: cancellationToken);
                if (inventories is null || !inventories.Any())
                {
                    return Result<IReadOnlyCollection<InventoryBaseReadResponse>>
                        .NotFound("Inventories with low stock");
                }
                var response = inventories.Select(i => Utility.Map(i))
                    .ToList()
                    .AsReadOnly();
                return Result<IReadOnlyCollection<InventoryBaseReadResponse>>
                    .Success(response);
            }
            catch (Exception ex)
            {
                return Result<IReadOnlyCollection<InventoryBaseReadResponse>>
                        .Exception(nameof(GetInventoryLowStockAsync), ex);
            }
        }

        public async Task<Result<decimal>>
            GetInventoryValuationAsync(CancellationToken cancellationToken)
        {
            try
            {
                var result = await _uow.Inventories
                    .GetInventoryValuationAsync(cancellationToken);
                return Result<decimal>.Success(result);
            }
            catch (Exception ex)
            {
                return Result<decimal>
                        .Exception(nameof(GetInventoryValuationAsync), ex);
            }
        }

        public async Task<Result<decimal>>
           GetInventoryCostAsync(CancellationToken cancellationToken)
        {
            try
            {
                var result = await _uow.Inventories
                    .GetInventoryCostAsync(cancellationToken);
                return Result<decimal>.Success(result);
            }
            catch (Exception ex)
            {
                return Result<decimal>
                        .Exception(nameof(GetInventoryValuationAsync), ex);
            }
        }

        public async Task<Result> DeleteByIdAsync(int id,
            CancellationToken cancellationToken)
        {
            try
            {
                var inventory = await _uow.Inventories
                               .FindAsync(i => i.Id == id,
                               cancellationToken: cancellationToken);
                if (inventory is null)
                {
                    return Result.NotFound("Inventory");
                }
                inventory.Delete();
                return await SoftDeleteAsync(inventory, cancellationToken);


            }
            catch (DomainException ex)
            {
                return Result.Failure(ex.Message, ErrorType.Conflict);
            }
            catch (Exception ex)
            {
                return Result.Exception(nameof(DeleteByIdAsync), ex);
            }
        }

    }
}

