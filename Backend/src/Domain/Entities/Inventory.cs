﻿#nullable enable
using Application.Results;
using Domain.Abstractions;
using Domain.Entities.Products;
using Domain.Enums;
using Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Domain.Entities;

public partial class Inventory : IBaseEntity, IModifiableEntity ,ISoftDeletable
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
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public int? DeletedByUserId { get; set; }
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


    // to do add StockChanged event here
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

    }
    public void UpdateStock(
        decimal newQuantity,
        StockMovementTypeEnum stockMovementType)
    {
        if (newQuantity < 0)
        {
            throw new Exception("Stock quantity cannot be negative");
        }
        EnsureQuantityIsLessThanMaxLevel(newQuantity);
        QuantityOnHand = newQuantity;
        var stockMovement = StockMovement.Create(
            Product,
            this,
            stockMovementType,
            newQuantity,
            "Stock update"
            );
        stockMovement.MarkAsCompleted();
        AddStockMovement(stockMovement);
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
        EnsureQuantityIsLessThanMaxLevel(quantityOnHand);
        if (maxLevel < reorderLevel)
        {
            throw new DomainException("Max level cannot be less than reorder level");
        }
        UpdateStock(quantityOnHand);
        ReorderLevel = reorderLevel;
        MaxLevel = maxLevel;

    }
    public void Delete()
    {
        if( IsDeleted )
        {
            throw new DomainException("Inventory is already deleted");
        }
        if( QuantityOnHand > 0 )
        {
            throw new DomainException("Cannot delete inventory with stock on hand");
        }
    }
}
