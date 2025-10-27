#nullable enable
using Domain.Abstractions;
using Domain.Entities.Products;
using Domain.Enums;
using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class StockMovement : IBaseEntity
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public int InventoryId { get; set; }

    public int MovementTypeId { get; set; }

    public decimal Quantity { get; set; }

    public StockMovementStatus StockMovmentStatus { get; set; }

    public DateTime CreatedAt { get; set; }

    public int CreatedByUserId { get; set; }

    public string? Notes { get; set; }

    public virtual User CreatedByUser { get; set; } = null!;

    public virtual Inventory Inventory { get; set; } = null!;

    public virtual StockMovementType MovementType { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;


    public StockMovement()
    {
        
    }
    private StockMovement(
        Product product,
        Inventory inventory,
        StockMovementTypeEnum movementTypeId,
        decimal quantity,
        string? notes
        )
    {
        Product = product;
        Inventory= inventory;
        MovementTypeId = (int)movementTypeId;
        Quantity = quantity;
        Notes = notes;
        StockMovmentStatus= StockMovementStatus.Pending;
    }
    internal static StockMovement Create(
        Product product,
        Inventory inventory,
        StockMovementTypeEnum movementType,
        decimal quantity,
        string? notes
        )
    {
        return new StockMovement(
            product,
            inventory,
            movementType,
            quantity,
            notes
            );
    }

    internal void MarkAsCompleted()
    {
        StockMovmentStatus = StockMovementStatus.Completed;
    }

}