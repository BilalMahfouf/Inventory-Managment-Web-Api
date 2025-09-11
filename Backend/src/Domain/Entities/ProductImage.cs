#nullable enable
using Domain.Abstractions;
using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class ProductImage : ISoftDeletable
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public int ImageId { get; set; }
    public bool IsPrimary { get; set; }

    public DateTime CreatedAt { get; set; }
    public int CreatedByUserId { get; set; }

    public DateTime? UpdatedAt { get; set; }
    public int? UpdatedByUserId { get; set; }

    public DateTime? DeletedAt { get; set; }
    public int? DeletedByUserId { get; set; }
    public bool IsDeleted { get; set; }



    public virtual User CreatedByUser { get; set; } = null!;
    public virtual User? DeletedByUser { get; set; }
    public virtual User? UpdatedByUser { get; set; }

    public virtual Image Image { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}