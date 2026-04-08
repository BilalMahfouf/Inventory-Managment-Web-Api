#nullable enable
using Domain.Inventories.Enums;
using Domain.Shared.Abstractions;
using Domain.Shared.Entities;
using Domain.Shared.Exceptions;

namespace Domain.Sales.Entities;

public class SalesOrder : Entity
{
    public int? CustomerId { get; private set; }

    public DateTime OrderDate { get; private set; }
    public SalesOrderStatus SalesStatus { get; private set; }
    public DateTime? SalesStatusUpdatedAt { get; private set; }

    public PaymentStatus PaymentStatus { get; private set; }

    public decimal TotalAmount
    {
        get => _items.Sum(i => i.LineAmount);
        private set { }
    }


    public string? Description { get; private set; }
    public string? ShippingAddress { get; private set; }
    public string? TrackingNumber { get; private set; }
    public bool IsWalkIn { get; private set; }

    public int CreatedByUserId { get; set; }

    public User CreatedByUser { get; private set; } = null!;

    public Customer? Customer { get; private set; }

    private readonly List<SalesOrderItem> _items = new();
    public IReadOnlyCollection<SalesOrderItem> Items => _items.AsReadOnly();

    private SalesOrder()
    {
    }

    private SalesOrder(
        int? customerId,
        bool isWalkIn,
        string? description,
        string? shippingAddress)
    {
        CustomerId = customerId;
        IsWalkIn = isWalkIn;
        Description = description;
        ShippingAddress = shippingAddress;
        PaymentStatus = PaymentStatus.Unpaid;
    }

    public static SalesOrder Create(
        int? customerId,
        List<SalesOrderItemRequest> items,
        string? description = null,
        string? shippingAddress = null)
    {
        if (customerId is null)
        {
            throw new DomainException("Customer is required for non walk-in orders.");
        }

        if (items is null || items.Count == 0)
        {
            throw new DomainException("Sales order must have at least one item.");
        }

        var order = new SalesOrder(customerId, false, description, shippingAddress)
        {
            OrderDate = DateTime.UtcNow,
            SalesStatus = SalesOrderStatus.Pending,
        };

        foreach (var item in items)
        {
            order.AddItemWithStockDeduction(item);
        }

        order.RaiseDomainEvent(new SalesOrderCreatedDomainEvent(
            order.Id,
            order.CustomerId,
            order.SalesStatus,
            order.OrderDate,
            order.TotalAmount));

        return order;
    }

    public static SalesOrder CreateWalkIn(
        List<SalesOrderItemRequest> items,
        PaymentStatus paymentStatus,
        string? description = null)
    {
        if (items is null || items.Count == 0)
        {
            throw new DomainException("Sales order must have at least one item.");
        }

        var order = new SalesOrder(null, true, description, null)
        {
            OrderDate = DateTime.UtcNow,
            SalesStatus = SalesOrderStatus.Completed,
            SalesStatusUpdatedAt = DateTime.UtcNow,
            PaymentStatus=paymentStatus
        };

        foreach (var item in items)
        {
            order.AddItemWithStockDeduction(item);
        }

        order.RaiseDomainEvent(new SalesOrderCreatedDomainEvent(
            order.Id,
            order.CustomerId,
            order.SalesStatus,
            order.OrderDate,
            order.TotalAmount));

        return order;
    }

    public void UpdatePendingDetails(
        int? customerId,
        string? description,
        string? shippingAddress)
    {
        EnsurePendingForItemMutation();

        if (IsWalkIn && customerId is not null)
        {
            throw new DomainException("Walk-in orders cannot be assigned to a customer.");
        }

        CustomerId = customerId;
        Description = description;
        ShippingAddress = shippingAddress;
    }

    public void AddItem(SalesOrderItem item)
    {
        EnsurePendingForItemMutation();
        _items.Add(item);
    }

    public void AddItem(Inventory inventory, decimal quantity)
    {
        EnsurePendingForItemMutation();

        if (quantity <= 0)
        {
            throw new DomainException("Quantity must be greater than zero.");
        }

        _items.Add(new SalesOrderItem(
            inventory.ProductId,
            inventory.Id,
            inventory.LocationId,
            quantity,
            inventory.Product.UnitPrice));
    }

    public void RemoveItem(SalesOrderItem item)
    {
        EnsurePendingForItemMutation();

        if (!_items.Any(i => i == item))
        {
            throw new DomainException("Item not found in the order.");
        }

        if (!_items.Remove(item))
        {
            throw new DomainException("Failed to remove item from the order.");
        }
    }

    public void Confirm()
    {
        if (SalesStatus != SalesOrderStatus.Pending)
        {
            throw new DomainException("Only pending orders can be confirmed.");
        }

        SalesStatus = SalesOrderStatus.Confirmed;
        SalesStatusUpdatedAt = DateTime.UtcNow;

        RaiseDomainEvent(new SalesOrderConfirmedDomainEvent(Id));
    }

    public void MarkInTransit()
    {
        if (SalesStatus != SalesOrderStatus.Confirmed)
        {
            throw new DomainException("Only confirmed orders can be moved to in transit.");
        }

        SalesStatus = SalesOrderStatus.InTransit;
        SalesStatusUpdatedAt = DateTime.UtcNow;
    }

    public void MarkShipped(string? trackingNumber)
    {
        if (SalesStatus != SalesOrderStatus.InTransit)
        {
            throw new DomainException("Only in-transit orders can be marked as shipped.");
        }

        TrackingNumber = trackingNumber;
        SalesStatus = SalesOrderStatus.Shipped;
        SalesStatusUpdatedAt = DateTime.UtcNow;

        RaiseDomainEvent(new SalesOrderShippedDomainEvent(Id, trackingNumber));
    }

    public void Complete()
    {
        if (SalesStatus != SalesOrderStatus.Shipped)
        {
            throw new DomainException("Only shipped orders can be completed.");
        }

        SalesStatus = SalesOrderStatus.Completed;
        SalesStatusUpdatedAt = DateTime.UtcNow;

        RaiseDomainEvent(new SalesOrderCompletedDomainEvent(Id));
    }

    public void Cancel()
    {
        if (SalesStatus is not (SalesOrderStatus.Pending or SalesOrderStatus.Confirmed or SalesOrderStatus.InTransit))
        {
            throw new DomainException("Only pending, confirmed, or in-transit orders can be cancelled.");
        }

        foreach (var item in _items)
        {
            if (item.Inventory is null)
            {
                throw new DomainException("Cannot restore stock without loaded inventory.");
            }

            item.Inventory.IncreaseStock(
                item.OrderedQuantity,
                StockMovementTypeEnum.SalesOrder,
                $"Sales order {Id} cancellation");
        }

        var previousStatus = SalesStatus;
        SalesStatus = SalesOrderStatus.Cancelled;
        SalesStatusUpdatedAt = DateTime.UtcNow;

        RaiseDomainEvent(new SalesOrderCancelledDomainEvent(Id, previousStatus));
    }

    public void Return()
    {
        if (SalesStatus != SalesOrderStatus.Completed)
        {
            throw new DomainException("Only completed orders can be returned.");
        }

        SalesStatus = SalesOrderStatus.Returned;
        SalesStatusUpdatedAt = DateTime.UtcNow;

        RaiseDomainEvent(new SalesOrderReturnedDomainEvent(Id));
    }

    private void AddItemWithStockDeduction(SalesOrderItemRequest request)
    {
        if (request.Quantity <= 0)
        {
            throw new DomainException("Quantity must be greater than zero.");
        }

        var inventory = request.Inventory;
        if (inventory is null)
        {
            throw new DomainException("Inventory must be provided for each order item.");
        }

        if (inventory.Id != request.InventoryId)
        {
            throw new DomainException("Inventory mismatch for order item.");
        }

        inventory.DecreaseStock(
            request.Quantity,
            StockMovementTypeEnum.SalesOrder,
            $"Sales order {Id}");

        var item = new SalesOrderItem(
            inventory.ProductId,
            inventory.Id,
            inventory.LocationId,
            request.Quantity,
            inventory.Product.UnitPrice);

        _items.Add(item);
    }

    private void EnsurePendingForItemMutation()
    {
        if (SalesStatus != SalesOrderStatus.Pending)
        {
            throw new DomainException("Only pending orders can modify items.");
        }
    }
}
