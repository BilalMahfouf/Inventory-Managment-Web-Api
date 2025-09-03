#nullable enable
using Domain.Abstractions;
using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class Customer : ISoftDeletable
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string Address { get; set; } = null!;

    public bool IsActive { get; set; }

    public decimal CreditLimit { get; set; }

    public string? PaymentTerms { get; set; }

    public string CreditStatus { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public int CreatedByUserId { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime? DeletedAt { get; set; }

    public int? DeletedByUserId { get; set; }

    public virtual User CreatedByUser { get; set; } = null!;


    public virtual User? DeletedByUser { get; set; }

}