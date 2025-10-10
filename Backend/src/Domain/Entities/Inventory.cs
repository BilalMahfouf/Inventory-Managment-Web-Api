#nullable enable
using Application.Results;
using Domain.Abstractions;
using Domain.Entities.Products;
using Domain.Enums;
using System;
using System.Collections.Generic;

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
    public static Result<Inventory> Create(
        Product product, 
        int locationId,
        decimal quantityOnHand,
        decimal reorderLevel,
        decimal maxLevel
        )
    {
        // Add any necessary validation or business logic here
        if ( maxLevel <quantityOnHand) 
        {
            return Result<Inventory>.Failure("Quantity on hand cannot exceed max level"
                , ErrorType.BadRequest);
        }
        if(maxLevel < reorderLevel)
        {
            return Result<Inventory>.Failure("Max level cannot be less than reorder level"
                , ErrorType.BadRequest);
        }
        var inventory = new Inventory(
            product,
            locationId,
            quantityOnHand,
            reorderLevel,
            maxLevel
            );
        return Result<Inventory>.Success(inventory);
    }
    // to do add StockChanged event here
    public Result UpdateInventoryLevels(
    decimal quantityOnHand,
    decimal reorderLevel,
    decimal maxLevel
        )
    {
        if(maxLevel < quantityOnHand)
        {
            return Result.Failure("Quantity on hand cannot exceed max level"
                , ErrorType.Conflict);
        }
        if (maxLevel < reorderLevel)
        {
            return Result.Failure("Max level cannot be less than reorder level"
                , ErrorType.Conflict);
        }
        QuantityOnHand = quantityOnHand;
        ReorderLevel = reorderLevel;
        MaxLevel = maxLevel;
        return Result.Success;
    }
}
