#nullable enable
using Domain.Abstractions;
using System;
using System.Collections.Generic;

namespace Domain.Entities;

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
}