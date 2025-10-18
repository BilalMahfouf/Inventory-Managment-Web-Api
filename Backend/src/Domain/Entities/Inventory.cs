#nullable enable
using Application.Results;
using Domain.Abstractions;
using Domain.Entities.Products;
using Domain.Enums;
using Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Domain.Entities;

public partial class Inventory : IBaseEntity, IModifiableEntity
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public int LocationId { get; set; }

    public decimal QuantityOnHand { get; set; }

    public decimal ReorderLevel { get; set; }

    public decimal MaxLevel { get; set; }

    public DateTime CreatedAt { get; set; }

    public int CreatedByUserId { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? UpdatedByUserId { get; set; }

    public virtual User CreatedByUser { get; set; } = null!;

    public virtual Location Location { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;

    public virtual User? UpdatedByUser { get; set; }

    private List<StockMovement> stockMovements = new();
    public IReadOnlyCollection<StockMovement> StockMovements => stockMovements.AsReadOnly();



    public Inventory()
    {

    }
    private Inventory(
        Product product,
        int locationId,
        decimal quantityOnHand,
        decimal reorderLevel,
        decimal maxLevel
        )
    {

        Product = product;
        LocationId = locationId;
        QuantityOnHand = quantityOnHand;
        ReorderLevel = reorderLevel;
        MaxLevel = maxLevel;
    }
    private void AddStockMovement(StockMovement stockMovement
        )
    {
        stockMovements.Add(stockMovement);
    }
    public static Inventory Create(
        Product product,
        int locationId,
        decimal quantityOnHand,
        decimal reorderLevel,
        decimal maxLevel
        )
    {
        // Add any necessary validation or business logic here
        if (maxLevel < quantityOnHand)
        {
            throw new DomainException("Quantity on hand cannot exceed max level");
        }
        if (maxLevel < reorderLevel)
        {
            throw new DomainException("Max level cannot be less than reorder level");
        }
        var inventory = new Inventory(
            product,
            locationId,
            quantityOnHand,
            reorderLevel,
            maxLevel
            );
        var initialStockMovement = StockMovement.Create(
            product,
            inventory,
            StockMovementTypeEnum.InitialStock,
            quantityOnHand,
            "Initial stock entry"
            );
        initialStockMovement.MarkAsCompleted();

        inventory.AddStockMovement(initialStockMovement);
        return inventory;
    }
    // to do add StockChanged event here
    // to do add StockMovement creation here
    public void UpdateInventoryLevels(
    decimal quantityOnHand,
    decimal reorderLevel,
    decimal maxLevel
        )
    {
        if (maxLevel < quantityOnHand)
        {
            throw new DomainException("Quantity on hand cannot exceed max level");
        }
        if (maxLevel < reorderLevel)
        {
            throw new DomainException("Max level cannot be less than reorder level");
        }
        QuantityOnHand = quantityOnHand;
        ReorderLevel = reorderLevel;
        MaxLevel = maxLevel;
        
    }

}
