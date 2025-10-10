#nullable enable

using Domain.Abstractions;
using Domain.Entities.Products;
using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class AlertRule : IEntity
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public int LocationId { get; set; }

    public int AlertType { get; set; }

    public int Threshold { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual AlertType AlertTypeNavigation { get; set; } = null!;

    public virtual Location Location { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}