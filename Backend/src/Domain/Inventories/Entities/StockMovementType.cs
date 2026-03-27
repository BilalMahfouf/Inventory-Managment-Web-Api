#nullable enable
using Domain.Shared.Abstractions;
using Domain.Shared.Enums;
using System;
using System.Collections.Generic;

namespace Domain.Inventories.Entities;

public partial class StockMovementType : IBaseEntity, ISoftDeletable
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    /// <summary>
    ///   1 is IN ,2 is OUT, 3 is ADJUST
    /// </summary>
    public StockMovementDirection Direction { get; set; }

    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; }

    public int CreatedByUserId { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime? DeletedAt { get; set; }

    public int? DeletedByUserId { get; set; }

    public virtual User CreatedByUser { get; set; } = null!;

    public virtual User? DeletedByUser { get; set; }

}