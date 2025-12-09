#nullable enable
using Domain;
using Domain.Abstractions;
using System;
using System.Collections.Generic;

namespace Domain.Entities.Products;

public partial class ProductImage : IBaseEntity
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public int? ImageId { get; set; }
    public bool IsPrimary { get; set; }

    public DateTime CreatedAt { get; set; }
    public int CreatedByUserId { get; set; }

    public virtual Image Image { get; set; } = null!;
    public virtual User CreatedByUser { get; set; } = null!;
    public virtual Product Product { get; set; } = null!;
}