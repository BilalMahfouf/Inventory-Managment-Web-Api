#nullable enable
using Domain.Shared.Abstractions;
using System;
using System.Collections.Generic;

namespace Domain.Suppliers.Entities;

public partial class SupplierType : Entity
{
    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public bool IsIndividual { get; set; }
    public int CreatedByUserId { get; set; }
    public virtual User CreatedByUser { get; set; } = null!;

    public virtual User? DeletedByUser { get; set; }

}