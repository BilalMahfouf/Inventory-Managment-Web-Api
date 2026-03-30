#nullable enable
using Domain.Shared.Abstractions;
using System;
using System.Collections.Generic;

namespace Domain.Inventories.Entities;

public partial class AlertType : Entity
{
    public string Name { get; set; } = null!;

    public string Category { get; set; } = null!;

    public string? Description { get; set; }
    public int CreatedByUserId { get; set; }

    public virtual User CreatedByUser { get; set; } = null!;
}