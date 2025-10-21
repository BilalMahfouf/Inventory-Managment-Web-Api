#nullable enable
using Domain.Abstractions;
using Domain.Entities.Products;
using Domain.Enums;
using System;
using System.Collections.Generic;

namespace Domain.Entities;

public  class StockTransfer : IBaseEntity, IEntity
{
    public int Id { get; private set; }

    public int ProductId { get; private set; }

    public int FromLocationId { get; private set; }

    public int ToLocationId { get; private set; }

    public decimal Quantity { get; private set; }

    public TransferStatus TransferStatus { get;private  set; }

    public DateTime CreatedAt { get;  set; }

    public int CreatedByUserId { get; set; }

    public virtual User CreatedByUser { get; private set; } = null!;

    public virtual Location FromLocation { get; private set; } = null!;

    public virtual Product Product { get; private set; } = null!;

    public virtual Location ToLocation { get; private set; } = null!;
}