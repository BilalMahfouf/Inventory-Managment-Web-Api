#nullable enable
using Domain.Shared.Abstractions;
using System;
using System.Collections.Generic;

namespace Domain.Products.Entities;

public partial class UnitOfMeasure : Entity
{
    public string Name { get; set; } = null!;

    public string? Description { get; set; }
    public int CreatedByUserId { get; set; }

    public DateTime? UpdatedAt { get; set; }
    public int? UpdatedByUserId { get; set; }
    public virtual User CreatedByUser { get; set; } = null!;
    public virtual User? UpdatedByUser { get; set; }
    public virtual User? DeletedByUser { get; set; }

}