#nullable enable
using Domain.Shared.Abstractions;
using Domain.Shared.Errors;
using System;
using System.Collections.Generic;

namespace Domain.Inventories.Entities;

public partial class StockMovementType : Entity
{
    public string Name { get; set; } = null!;

    /// <summary>
    ///   1 is IN ,2 is OUT, 3 is ADJUST
    /// </summary>
    public StockMovementDirection Direction { get; set; }

    public string? Description { get; set; }
    public int CreatedByUserId { get; set; }
    public virtual User CreatedByUser { get; set; } = null!;

    public virtual User? DeletedByUser { get; set; }

}