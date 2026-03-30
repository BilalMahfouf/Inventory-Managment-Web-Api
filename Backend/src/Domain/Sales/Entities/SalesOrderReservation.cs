using Domain.Shared.Abstractions;
using Domain.Shared.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Sales.Entities;

public sealed class SalesOrderReservation : Entity
{
    public int OrderId { get; private set; }
    public int StockMovemntId { get; private set; }
    public int ProductId { get; private set; }
    public int InventoryId { get; private set; }
    public decimal Quantity { get; private set; }
    public SalesOrderResevationStatus Status { get; private set; }
    public DateTime? StatusUpdateAt { get; private set; }
    public int CreatedByUserId { get; set; }

    private SalesOrderReservation()
    {
    }
    internal SalesOrderReservation(
        int orderId,
        int productId,
        int inventoryId,
        SalesOrderResevationStatus status,
        decimal quantity)
    {
        OrderId = orderId;
        ProductId = productId;
        InventoryId = inventoryId;
        Quantity = quantity;
        Status = status;
    }

    public void CompleteReservation()
    {
        if(Status is not SalesOrderResevationStatus.Pending)
        {
            throw new DomainException(
                "Only pending reservations can be completed.");
        }
        Status = SalesOrderResevationStatus.Completed;
        StatusUpdateAt = DateTime.UtcNow;
    }

}

public enum SalesOrderResevationStatus
{
    Pending = 1,
    Completed = 2,
    Cancelled = 3

}
