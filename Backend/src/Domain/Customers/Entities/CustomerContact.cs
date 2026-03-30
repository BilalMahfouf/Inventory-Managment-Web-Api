#nullable enable
using Domain.Shared.Abstractions;
using System;
using System.Collections.Generic;

namespace Domain.Customers.Entities;

public partial class CustomerContact : Entity
{
    public int CustomerId { get; set; }

    public string ContactName { get; set; } = null!;

    public string JobTitle { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public bool IsPrimary { get; set; }

    public string ContactType { get; set; } = null!;

    public bool IsActive { get; set; }
    public int CreatedByUserId { get; set; }
    public virtual User CreatedByUser { get; set; } = null!;

    public virtual Customer Customer { get; set; } = null!;

    public virtual User? DeletedByUser { get; set; }
}