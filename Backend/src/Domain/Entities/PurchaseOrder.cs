#nullable enable
using Domain.Abstractions;
using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class PurchaseOrder : IBaseEntity
{
    public int Id { get; set; }

    public int SupplierId { get; set; }

    public DateTime OrderDate { get; set; }

    public byte PurchaseStatus { get; set; }

    public decimal TotalAmount { get; set; }

    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; }

    public int CreatedByUserId { get; set; }

    public virtual User CreatedByUser { get; set; } = null!;
    public virtual Supplier Supplier { get; set; } = null!;
}