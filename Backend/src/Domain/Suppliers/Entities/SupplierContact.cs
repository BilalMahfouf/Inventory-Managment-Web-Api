#nullable enable
using Domain.Shared.Abstractions;
using System;
using System.Collections.Generic;

namespace Domain.Suppliers.Entities;

public partial class SupplierContact : Entity
{
    public int SupplierId { get; set; }

    public string ContactName { get; set; } = null!;

    public string JobTitle { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public bool IsPrimary { get; set; }

    public string ContactType { get; set; } = null!;

    public bool IsActive { get; set; }
    public int CreatedByUserId { get; set; }
    public virtual User CreatedByUser { get; set; } = null!;

    public virtual User? DeletedByUser { get; set; }

    public virtual Supplier Supplier { get; set; } = null!;
}