#nullable enable
using Domain.Abstractions;
using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class StockTransfer : IBaseEntity, IEntity
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public int FromLocationId { get; set; }

    public int ToLocationId { get; set; }

    public decimal Quantity { get; set; }

    public byte TransferStatus { get; set; }

    public DateTime CreatedAt { get; set; }

    public int CreatedByUserId { get; set; }

    public virtual User CreatedByUser { get; set; } = null!;

    public virtual Location FromLocation { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;

    public virtual Location ToLocation { get; set; } = null!;
}