#nullable enable
using Domain.Abstractions;
using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class ProductCategory : IBaseEntity, ISoftDeletable
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public int? ParentId { get; set; }

    public DateTime CreatedAt { get; set; }

    public int CreatedByUserId { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime? UpdateAt { get; set; }
    public int? UpdatedByUserId { get; set; }
    public DateTime? DeletedAt { get; set; }

    public int? DeletedByUserId { get; set; }

    public virtual User CreatedByUser { get; set; } = null!;

    public virtual User? UpdatedByUser { get; set; }

    public virtual User? DeletedByUser { get; set; }
    public virtual ProductCategory? Parent { get; set; }

}