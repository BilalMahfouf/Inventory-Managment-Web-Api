#nullable enable
using Domain.Abstractions;
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
}