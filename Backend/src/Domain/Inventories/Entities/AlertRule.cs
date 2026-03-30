#nullable enable

using Domain.Shared.Abstractions;
using Domain.Products.Entities;
using System;
using System.Collections.Generic;

namespace Domain.Inventories.Entities;

public partial class AlertRule : Entity
{
    public int ProductId { get; set; }

    public int LocationId { get; set; }

    public int AlertType { get; set; }

    public int Threshold { get; set; }

    public bool IsActive { get; set; }
    public virtual AlertType AlertTypeNavigation { get; set; } = null!;

    public virtual Location Location { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}