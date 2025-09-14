#nullable enable
using Domain.Abstractions;
using System;
using System.Collections.Generic;

namespace Domain.Entities;

public partial class SalesOrderItem : IBaseEntity
{
    public int Id { get; set; }

    public int SalesOrderId { get; set; }

    public int ProductId { get; set; }

    public decimal OrderedQuantity { get; set; }

    public decimal ReceivedQuantity { get; set; }

    public decimal UnitCost { get; set; }

    public decimal LineAmount { get; set; }

    public DateTime CreatedAt { get; set; }

    public int CreatedByUserId { get; set; }

    public virtual User CreatedByUser { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;

    public virtual SalesOrder SalesOrder { get; set; } = null!;
}