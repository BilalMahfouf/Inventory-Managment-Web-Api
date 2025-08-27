#nullable enable
using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class ProductSupplier
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public int SupplierId { get; set; }

    public string SupplierProductCode { get; set; } = null!;

    public int LeadTimeDays { get; set; }

    public int MinOrderQuantity { get; set; }

    public DateTime CreatedAt { get; set; }

    public int CreatedByUserId { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime? DeletedAt { get; set; }

    public int? DeletedByUserId { get; set; }

    public virtual User CreatedByUser { get; set; } = null!;

    public virtual User? DeletedByUser { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual Supplier Supplier { get; set; } = null!;
}