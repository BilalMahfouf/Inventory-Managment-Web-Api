#nullable enable
using Domain.Abstractions;
using Domain.Entities.Products;
using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class StockMovement : IBaseEntity
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public int LocationId { get; set; }

    public int MovementTypeId { get; set; }

    public decimal Quantity { get; set; }

    public byte StockMovmentStatus { get; set; }

    public DateTime CreatedAt { get; set; }

    public int CreatedByUserId { get; set; }

    public string? Notes { get; set; }

    public virtual User CreatedByUser { get; set; } = null!;

    public virtual Location Location { get; set; } = null!;

    public virtual StockMovementType MovementType { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}