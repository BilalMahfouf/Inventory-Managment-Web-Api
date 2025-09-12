#nullable enable
using Domain.Abstractions;
using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class ProductImage : IEntity
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public int? ImageId { get; set; }
    public bool IsPrimary { get; set; }

    public DateTime CreatedAt { get; set; }
    public int CreatedByUserId { get; set; }

   
   
    public virtual User CreatedByUser { get; set; } = null!;
   

    public virtual Image Image { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}