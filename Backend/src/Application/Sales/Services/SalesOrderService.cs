using Application.Shared.Contracts;
using Application.Sales.RequestResponse;
using Domain.Inventories.Entities;
using Domain.Inventories.Enums;
using Domain.Shared.Errors;
using Domain.Shared.Exceptions;
using Domain.Shared.Results;

namespace Application.Sales.Services;

public sealed class SalesOrderService
{
    private readonly IUnitOfWork _uow;

    public SalesOrderService(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<Result<int>> CreateSalesOrderAsync(
        CreateSalesOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (request.Items is null || !request.Items.Any())
            {
                return Result<int>.Failure("Sales order must have at least one item.", ErrorType.Validation);
            }

            if (!request.IsWalkIn && request.CustomerId is null)
            {
                return Result<int>.Failure("Customer is required for non walk-in orders.", ErrorType.Validation);
            }

            if (request.IsWalkIn && request.CustomerId is not null)
            {
                return Result<int>.Failure("Walk-in orders cannot have a customer.", ErrorType.Validation);
            }

            var items = new List<Domain.Sales.Entities.SalesOrderItemRequest>();

            foreach (var item in request.Items)
            {
                if (item.Quantity <= 0)
                {
                    return Result<int>.Failure("Item quantity must be greater than zero.", ErrorType.Validation);
                }

                var inventory = await _uow.Inventories.FindAsync(
                    e => e.ProductId == item.ProductId && e.LocationId == item.LocationId,
                    cancellationToken,
                    includeProperties: nameof(Inventory.Product));

                if (inventory is null)
                {
                    return Result<int>.Failure(Error.NotFound("Inventory not found"));
                }

                if (inventory.ProductId != item.ProductId)
                {
                    return Result<int>.Failure(
                        $"Inventory {inventory.Id} does not belong to product {item.ProductId}.",
                        ErrorType.Validation);
                }

                if (inventory.QuantityOnHand < item.Quantity)
                {
                    return Result<int>.Failure(
                        $"Insufficient stock for inventory {inventory.Id}. Available {inventory.QuantityOnHand}, requested {item.Quantity}.",
                        ErrorType.Conflict);
                }

                items.Add(new Domain.Sales.Entities.SalesOrderItemRequest
                {
                    InventoryId = inventory.Id,
                    Inventory = inventory,
                    Product = inventory.Product,
                    Quantity = item.Quantity,
                });
            }

            SalesOrder order;
            if (request.IsWalkIn)
            {
                order = SalesOrder.CreateWalkIn(
                    items,
                    request.PaymentStatus,
                    request.Description);
            }
            else
            {
                order = SalesOrder.Create(
                    request.CustomerId,
                    items,
                    request.Description,
                    request.ShippingAddress);
            }

            _uow.SalesOrders.Add(order);
            await _uow.SaveChangesAsync(cancellationToken);

            return Result<int>.Success(order.Id);
        }
        catch (DomainException dex)
        {
            return Result<int>.Failure(dex.Message, ErrorType.Conflict);
        }
        catch (Exception ex)
        {
            return Result<int>.Failure(Error.Exception(nameof(CreateSalesOrderAsync), ex));
        }
    }

    public async Task<Result> UpdateSalesOrderAsync(
        int orderId,
        UpdateSalesOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var order = await _uow.SalesOrders.FindAsync(
                e => e.Id == orderId,
                cancellationToken,
                includeProperties: "Items,Items.Inventory,Items.Inventory.Product");

            if (order is null)
            {
                return Result.Failure(Error.NotFound($"Order with Id {orderId}"));
            }

            order.UpdatePendingDetails(
                request.CustomerId,
                request.Description,
                request.ShippingAddress);

            if (request.Items is not null)
            {
                var currentItems = order.Items.ToList();

                foreach (var item in currentItems)
                {
                    if (item.Inventory is null)
                    {
                        return Result.Failure("Order items inventory data is missing.", ErrorType.Failure);
                    }

                    item.Inventory.IncreaseStock(
                        item.OrderedQuantity,
                        StockMovementTypeEnum.SalesOrder,
                        $"Sales order {order.Id} update restore");

                    _uow.SalesOrderItems.Delete(item);
                    order.RemoveItem(item);
                }

                var mergedItems = request.Items
                    .GroupBy(i => new { i.ProductId, i.LocationId })
                    .Select(g => new
                    {
                        g.Key.ProductId,
                        g.Key.LocationId,
                        Quantity = g.Sum(x => x.Quantity),
                    })
                    .ToList();

                foreach (var item in mergedItems)
                {
                    if (item.Quantity <= 0)
                    {
                        return Result.Failure("Item quantity must be greater than zero.", ErrorType.Validation);
                    }

                    var inventory = await _uow.Inventories.FindAsync(
                        e => e.LocationId == item.LocationId
                        && e.ProductId == item.ProductId,
                        cancellationToken,
                        includeProperties: nameof(Inventory.Product));

                    if (inventory is null)
                    {
                        return Result.Failure(Error.NotFound($"Inventory with id {item.LocationId}"));
                    }

                    if (inventory.ProductId != item.ProductId)
                    {
                        return Result.Failure(
                            $"Inventory {inventory.Id} does not belong to product {item.ProductId}.",
                            ErrorType.Validation);
                    }

                    if (inventory.QuantityOnHand < item.Quantity)
                    {
                        return Result.Failure(
                            $"Insufficient stock for inventory {inventory.Id}. Available {inventory.QuantityOnHand}, requested {item.Quantity}.",
                            ErrorType.Conflict);
                    }

                    inventory.DecreaseStock(
                        item.Quantity,
                        StockMovementTypeEnum.SalesOrder,
                        $"Sales order {order.Id} update reserve");

                    order.AddItem(inventory, item.Quantity);
                }
            }

            await _uow.SaveChangesAsync(cancellationToken);

            return Result.Success;
        }
        catch (DomainException dex)
        {
            return Result.Failure(dex.Message, ErrorType.Conflict);
        }
        catch (Exception ex)
        {
            return Result.Failure(Error.Exception(nameof(UpdateSalesOrderAsync), ex));
        }
    }

    public async Task<Result> ConfirmOrderAsync(
        int orderId,
        CancellationToken cancellationToken = default)
    {
        return await ExecuteTransitionAsync(
            orderId,
            order => order.Confirm(),
            cancellationToken);
    }

    public async Task<Result> MarkInTransitAsync(
        int orderId,
        CancellationToken cancellationToken = default)
    {
        return await ExecuteTransitionAsync(
            orderId,
            order => order.MarkInTransit(),
            cancellationToken);
    }

    public async Task<Result> ShipOrderAsync(
        int orderId,
        string? trackingNumber,
        CancellationToken cancellationToken = default)
    {
        return await ExecuteTransitionAsync(
            orderId,
            order => order.MarkShipped(trackingNumber),
            cancellationToken);
    }

    public async Task<Result> CompleteOrderAsync(
        int orderId,
        CancellationToken cancellationToken = default)
    {
        return await ExecuteTransitionAsync(
            orderId,
            order => order.Complete(),
            cancellationToken);
    }

    public async Task<Result> CancelOrderAsync(
        int orderId,
        CancellationToken cancellationToken = default)
    {
        return await ExecuteTransitionAsync(
            orderId,
            order => order.Cancel(),
            cancellationToken,
            includeProperties: "Items,Items.Inventory,Items.Inventory.Product");
    }

    public async Task<Result> ReturnOrderAsync(
        int orderId,
        CancellationToken cancellationToken = default)
    {
        return await ExecuteTransitionAsync(
            orderId,
            order => order.Return(),
            cancellationToken);
    }

    private async Task<Result> ExecuteTransitionAsync(
        int orderId,
        Action<SalesOrder> transition,
        CancellationToken cancellationToken,
        string? includeProperties = null)
    {
        try
        {
            var order = await _uow.SalesOrders.FindAsync(
                e => e.Id == orderId,
                cancellationToken,
                includeProperties ?? string.Empty);

            if (order is null)
            {
                return Result.Failure(Error.NotFound($"Order with Id {orderId}"));
            }

            transition(order);
            await _uow.SaveChangesAsync(cancellationToken);

            return Result.Success;
        }
        catch (DomainException dex)
        {
            return Result.Failure(dex.Message, ErrorType.Conflict);
        }
        catch (Exception ex)
        {
            return Result.Failure(Error.Exception(nameof(ExecuteTransitionAsync), ex));
        }
    }
}
