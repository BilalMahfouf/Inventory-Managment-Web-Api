#nullable enable
using Domain.Shared.Abstractions;
using System;
using System.Collections.Generic;

namespace Domain.Customers.Entities;

public partial class CustomerCategory : IBaseEntity, ISoftDeletable
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public bool IsIndividual { get; set; }

    public DateTime CreatedAt { get; set; }

    public int CreatedByUserId { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime? DeletedAt { get; set; }

    public int? DeletedByUserId { get; set; }

    public virtual User CreatedByUser { get; set; } = null!;

    public virtual User? DeletedByUser { get; set; }

    public static CustomerCategory Create(
        string name,
        bool isIndividual,
        string? description)
    {
        CustomerCategory category = new CustomerCategory()
        {
            Name = name,
            IsIndividual = isIndividual,
            Description = description
        };
        return category;
    }
    public void Update(
        string name,
        bool isIndividual,
        string? description)
    {
        Name = name;
        IsIndividual = isIndividual;
        Description = description;
    }
}