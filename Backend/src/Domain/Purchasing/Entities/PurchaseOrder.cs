#nullable enable
using Domain.Shared.Abstractions;
using System;
using System.Collections.Generic;

namespace Domain.Purchasing.Entities;

public partial class PurchaseOrder : Entity
{
    public int SupplierId { get; set; }

    public DateTime OrderDate { get; set; }

    public byte PurchaseStatus { get; set; }

    public decimal TotalAmount { get; set; }

    public string? Description { get; set; }
    public int CreatedByUserId { get; set; }

    public virtual User CreatedByUser { get; set; } = null!;
    public virtual Supplier Supplier { get; set; } = null!;
}