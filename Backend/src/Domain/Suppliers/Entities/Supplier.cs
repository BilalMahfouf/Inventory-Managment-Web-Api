#nullable enable
using Domain.Shared.Abstractions;
using System;
using System.Collections.Generic;

namespace Domain.Suppliers.Entities;

public partial class Supplier : Entity, IModifiableEntity
{
    public string Name { get; set; } = null!;

    public int SupplierTypeId { get; set; }

    public string Email { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string Terms { get; set; } = null!;

    public bool IsActive { get; set; }
    public int CreatedByUserId { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public int? UpdatedByUserId { get; set; }
    public virtual User CreatedByUser { get; set; } = null!;

    public virtual User? DeletedByUser { get; set; }
    public virtual SupplierType SupplierType { get; set; } = null!;

    public virtual User? UpdatedByUser { get; set; }
}