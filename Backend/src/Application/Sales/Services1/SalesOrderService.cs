using Application.Abstractions.UnitOfWork;
using Application.Results;
using Application.Sales.RequestResponse;
using Domain.Entities.Products;
using Domain.Enums;
using Domain.Exceptions;
using Domain.Sales;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Sales.Services1;

public sealed class SalesOrderService
{
    private readonly IUnitOfWork _uow;


    public SalesOrderService(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<Result<int>> CreateSalesOrderAsync(
        CreateSalesOrderRequest request,
        CancellationToken cancellationToken = default
        )
    {

        try
        {
            // add valdiation to request here


            List<Domain.Sales.SalesOrderItemRequest> items = new();

            foreach (var item in request.Items)
            {
                var product = await _uow.Products
                    .FindAsync(e => e.Id == item.ProductId,
                    cancellationToken,
                    "Inventories");
                if (product is null)
                {
                    return Result<int>.NotFound(nameof(product));
                }

                items.Add(new Domain.Sales.SalesOrderItemRequest()
                {
                    Product = product,
                    Quantity = item.Quantity
                });
            }
            if (items is null || !items.Any())
            {
                return Result<int>.Failure(
                    "Sales order must have at least one item.",
                    ErrorType.NotFound);
            }
            var order = SalesOrder.Create(
                request.CustomerId,
                items,
                request.SalesStatus,
                request.Description);
            if (order is null)
            {
                return Result<int>.Failure(
                    "Failed to create sales order.",
                    ErrorType.InternalServerError);
            }
            // to do make this in the background job

            foreach (var item in items)
            {
                foreach (var inventory in item.Product.Inventories)
                {
                    if (inventory.QuantityOnHand >= item.Quantity)
                    {
                        inventory.UpdateStock(inventory.QuantityOnHand - item.Quantity,
                            StockMovementTypeEnum.SalesOrder);
                        break;
                    }
                    else if (inventory.QuantityOnHand <= item.Quantity &&
                        inventory.QuantityOnHand > 0)
                    {
                        item.Quantity -= inventory.QuantityOnHand;
                        inventory.UpdateStock(0, StockMovementTypeEnum.SalesOrder);
                    }
                }
            }
            return Result<int>.Success(order.Id);
        }
        catch (DomainException dex)
        {
            return Result<int>.Failure(dex.Message, ErrorType.Conflict);
        }
        catch (Exception ex)
        {
            return Result<int>.Exception(nameof(CreateSalesOrderAsync), ex);
        }
    }
}
