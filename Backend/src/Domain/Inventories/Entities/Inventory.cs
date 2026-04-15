#nullable enable
using Domain.Shared.Results;
using Domain.Shared.Abstractions;
using Domain.Shared.Entities;
using Domain.Products.Entities;
using Domain.Shared.Errors;
using Domain.Shared.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Domain.Inventories.Entities;

public class Inventory : Entity, IModifiableEntity
{
    public int ProductId { get; set; }

    public int LocationId { get; set; }

    public decimal QuantityOnHand { get; set; }
    public decimal QuantityReserved { get; private set; }

    public decimal ReorderLevel { get; set; }

    public decimal MaxLevel { get; set; }
    public int CreatedByUserId { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? UpdatedByUserId { get; set; }
    public User? DeletedByUser { get; set; }

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
        product.EnsureProductIsActive();
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

    public void UpdateStock(decimal newQuantity)
    {
        if (newQuantity < 0)
        {
            throw new DomainException("Stock quantity cannot be negative");
        }
        EnsureQuantityIsLessThanMaxLevel(newQuantity);
        if (QuantityOnHand > newQuantity)
        {
            var stockMovement = StockMovement.Create(
                   Product,
                this,
                StockMovementTypeEnum.StockDecreaseAdjustment,
                QuantityOnHand - newQuantity,
                "Stock decrease"
                );
            stockMovement.MarkAsCompleted();
            AddStockMovement(stockMovement);
        }
        if (QuantityOnHand < newQuantity)
        {
            var stockMovement = StockMovement.Create(
                  Product,
               this,
               StockMovementTypeEnum.StockIncreaseAdjustment,
               newQuantity - QuantityOnHand,
               "Stock increase"
               );
            stockMovement.MarkAsCompleted();
            AddStockMovement(stockMovement);
        }
        QuantityOnHand = newQuantity;
        if (QuantityOnHand < ReorderLevel)
        {
            RaiseDomainEvent(new LowStockDomainEvent(
                ProductId,
                LocationId,
                QuantityOnHand));
        }

    }

    /// <summary>
    /// This method updates the stock quantity and creates a stock movement record.
    /// If you want to decrease or increase stock
    /// ,use <see cref="UpdateStock(decimal)"/>  instead.
    /// For negative <paramref name="quantity"/> this will be decreased
    /// ,for positive it will be increased.
    /// Note: to use this you need to include <see cref="Product"/> navigation
    /// property when retrieving Inventory entity.
    /// </summary>
    /// <param name="quantity"></param>
    /// <param name="stockMovementType"></param>

    public void UpdateStock(
        decimal quantity,
        StockMovementTypeEnum stockMovementType)
    {
        // this method expect negetive quantity if the user want to decrease stock

        EnsureQuantityIsLessThanMaxLevel(
            quantity < 0 ? QuantityOnHand - quantity : QuantityOnHand + quantity);

        QuantityOnHand += quantity;

        QuantityReserved += quantity < 0 ? -quantity : quantity;

        var stockMovement = StockMovement.Create(
            Product,
            this,
            stockMovementType,
            quantity < 0 ? -quantity : quantity,
            "Stock update"
            );

        AddStockMovement(stockMovement);
        if (QuantityOnHand < ReorderLevel)
        {
            RaiseDomainEvent(new LowStockDomainEvent(
                ProductId,
                LocationId,
                QuantityOnHand));
        }

    }


    private void EnsureQuantityIsLessThanMaxLevel(decimal quantity)
    {
        if (quantity > MaxLevel)
        {
            throw new DomainException("Quantity exceeds maximum level");
        }
    }

    // to do add StockMovement creation here 10/18/2025
    public void UpdateInventoryLevels(
    decimal quantityOnHand,
    decimal reorderLevel,
    decimal maxLevel
        )
    {
        if(quantityOnHand > maxLevel)
        {
            throw new DomainException("Quantity on hand cannot exceed maximum level");
        }
        if (maxLevel < reorderLevel)
        {
            throw new DomainException("Max level cannot be less than reorder level");
        }
        MaxLevel = maxLevel;
        UpdateStock(quantityOnHand);
        ReorderLevel = reorderLevel;

    }
    public void Delete()
    {
        if (IsDeleted)
        {
            throw new DomainException("Inventory is already deleted");
        }
        if (QuantityOnHand > 0)
        {
            throw new DomainException("Cannot delete inventory with stock on hand");
        }
        IsDeleted = true;
    }

    public void CompleteReservation(
        decimal quantity)
    {
        if (quantity > QuantityReserved)
        {
            throw new DomainException(
                "Cannot complete reservation for more than reserved quantity");
        }
        QuantityReserved -= quantity;
    }

    /// <summary>
    /// To use this method you need to include the 
    /// <see cref="Product"/> navigation property when retrieving the Inventory entity.
    /// </summary>
    /// <param name="quantity"></param>
    /// <param name="movementType"></param>
    /// <param name="notes"></param>
    public void IncreaseStock(
        decimal quantity,
        StockMovementTypeEnum movementType = StockMovementTypeEnum.StockIncreaseAdjustment,
        string? notes = null)
    {
        var newQuantityOnHand = QuantityOnHand + quantity;
        EnsureQuantityIsLessThanMaxLevel(newQuantityOnHand);
        this.Product.EnsureProductIsActive();

        QuantityOnHand = newQuantityOnHand;
        var stockMovement = StockMovement.Create(
            Product,
            this,
            movementType,
            quantity,
            notes
            );
        if (movementType is StockMovementTypeEnum.StockIncreaseAdjustment)
        {
            stockMovement.MarkAsCompleted();
        }

        this.AddStockMovement(stockMovement);
    }
    /// <summary>
    /// To use this method you need to include the 
    /// <see cref="Product"/> navigation property when retrieving the Inventory entity.
    /// </summary>
    /// <param name="quantity"></param>
    /// <param name="movementType"></param>
    /// <param name="notes"></param>
    public void DecreaseStock(
        decimal quantity,
        StockMovementTypeEnum movementType = StockMovementTypeEnum.StockDecreaseAdjustment,
        string? notes = null)
    {
        this.Product.EnsureProductIsActive();

        var newQuantityOnHand = QuantityOnHand - quantity;
        if (newQuantityOnHand < 0)
        {
            throw new DomainException("Stock quantity cannot be negative");
        }
        QuantityOnHand = newQuantityOnHand;
        var stockMovement = StockMovement.Create(
            Product,
            this,
            movementType,
            quantity,
            notes
            );
        if (movementType is StockMovementTypeEnum.StockDecreaseAdjustment)
        {
            stockMovement.MarkAsCompleted();
        }
        this.AddStockMovement(stockMovement);
    }
}
