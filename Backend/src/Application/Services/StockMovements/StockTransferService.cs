using Application.Abstractions.UnitOfWork;
using Application.DTOs.StockMovements.Request;
using Application.Results;
using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.StockMovements;

public sealed class StockTransferService
{
    private readonly IUnitOfWork _uow;

    public StockTransferService(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<Result<int>> TransferStockAsync(
        StockTransferRequest request,
        CancellationToken cancellationToken = default)
    {

        try
        {

            // to do add validation for request 

            var fromInventory = await _uow.Inventories
                .FindAsync(e => e.ProductId == request.ProductId
                    && e.LocationId == request.FromLocationId,
                    cancellationToken, "Product");
            if (fromInventory is null)
            {
                return Result<int>.NotFound(" From Inventory");
            }

            var toInventory = await _uow.Inventories
                .FindAsync(e => e.ProductId == request.ProductId
                    && e.LocationId == request.ToLocationId,
                    cancellationToken, "Product");
            if (toInventory is null)
            {
                return Result<int>.NotFound(" To Inventory");
            }
            fromInventory.UpdateStock(-request.Quantity, StockMovementTypeEnum.TransferOut);
            toInventory.UpdateStock(request.Quantity, StockMovementTypeEnum.TransferIn);

            var stockTransfer = StockTransfer.Create(
                request.ProductId,
                request.FromLocationId,
                request.ToLocationId,
                request.Quantity);

            _uow.Inventories.Update(fromInventory);
            _uow.Inventories.Update(toInventory);
            _uow.StockTransfers.Add(stockTransfer);

            await _uow.SaveChangesAsync(cancellationToken);
            return Result<int>.Success(stockTransfer.Id);
        }
        catch (DomainException ex)
        {
            return Result<int>.Failure(ex.Message, ErrorType.Conflict);
        }
        catch (Exception ex)
        {
            return Result<int>.Exception(nameof(TransferStockAsync), ex);
        }
    }
}
