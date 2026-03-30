#nullable enable
using Domain.Shared.Abstractions;
using Domain.Products.Entities;
using System;
using System.Collections.Generic;

namespace Domain.Products.Entities;

public partial class ProductSupplier : Entity
{
    public int ProductId { get; set; }

    public int SupplierId { get; set; }

    public string SupplierProductCode { get; set; } = null!;

    public int LeadTimeDays { get; set; }

    public int MinOrderQuantity { get; set; }
    public int CreatedByUserId { get; set; }
    public virtual User CreatedByUser { get; set; } = null!;

    public virtual User? DeletedByUser { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual Supplier Supplier { get; set; } = null!;
}