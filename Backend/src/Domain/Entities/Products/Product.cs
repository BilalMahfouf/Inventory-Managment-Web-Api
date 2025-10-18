#nullable enable
using Application.Results;
using Domain;
using Domain.Abstractions;
using Domain.Enums;
using Domain.Exceptions;
using System;
using System.Collections.Generic;

namespace Domain.Entities.Products;

public partial class Product : ISoftDeletable, IModifiableEntity, IBaseEntity
{
    public int Id { get; set; }

    public string Sku { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public int CategoryId { get; set; }

    public int UnitOfMeasureId { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal Cost { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public int CreatedByUserId { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? UpdatedByUserId { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime? DeletedAt { get; set; }

    public int? DeletedByUserId { get; set; }


    public virtual ProductCategory Category { get; set; } = null!;

    public virtual User CreatedByUser { get; set; } = null!;

    public virtual User? DeletedByUser { get; set; }

    public virtual UnitOfMeasure UnitOfMeasure { get; set; } = null!;

    public virtual User? UpdatedByUser { get; set; }

    private readonly List<Inventory> _inventories = new();
    public virtual IReadOnlyCollection<Inventory> Inventories => _inventories.AsReadOnly();

    public Product()
    {

    }
    private Product(
        string sku,
        string name,
        string? description,
        int categoryId,
        int unitOfMeasureId,
        decimal unitPrice,
        decimal cost
        )
    {
        Sku = sku;
        Name = name;
        Description = description;
        CategoryId = categoryId;
        UnitOfMeasureId = unitOfMeasureId;
        UnitPrice = unitPrice;
        Cost = cost;
    }

    public void AddInventory(Inventory inventory)
    {
        EnsureProductIsActive();
        _inventories.Add(inventory);
    }
    public static Result<Product> Create(
        string sku,
        string name,
        string? description,
        int categoryId,
        int unitOfMeasureId,
        decimal unitPrice,
        decimal cost
        )
    {
        // Here you can add any validation or business logic before creating the product
        if (unitPrice < cost)
        {
            return Result<Product>.Failure("Unit price cannot be less than cost."
                , ErrorType.Conflict);
        }

        var product = new Product(
            sku,
            name,
            description,
            categoryId,
            unitOfMeasureId,
            unitPrice,
            cost);
        return Result<Product>.Success(product);
    }

    public Result Update(
        string name,
        string? description,
        int categoryId,
        decimal unitPrice,
        decimal cost)
    {
        EnsureProductIsActive();
        if (unitPrice < cost)
        {
            return Result.Failure("Unit price cannot be less than cost."
                , ErrorType.Conflict);
        }
        this.Name = name;
        this.Description = description;
        this.CategoryId = categoryId;
        this.UnitPrice = unitPrice;
        this.Cost = cost;
        return Result.Success;
    }

    public void EnsureProductIsActive()
    {
        if (!IsActive)
        {
            throw new DomainException("Product is not active.");
        }
    }
}
