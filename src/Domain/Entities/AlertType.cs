#nullable enable
using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class AlertType
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Category { get; set; } = null!;

    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; }

    public int CreatedByUserId { get; set; }


    public virtual User CreatedByUser { get; set; } = null!;
}