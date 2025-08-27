#nullable enable
using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class SupplierContact
{
    public int Id { get; set; }

    public int SupplierId { get; set; }

    public string ContactName { get; set; } = null!;

    public string JobTitle { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public bool IsPrimary { get; set; }

    public string ContactType { get; set; } = null!;

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public int CreatedByUserId { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime? DeletedAt { get; set; }

    public int? DeletedByUserId { get; set; }

    public virtual User CreatedByUser { get; set; } = null!;

    public virtual User? DeletedByUser { get; set; }

    public virtual Supplier Supplier { get; set; } = null!;
}