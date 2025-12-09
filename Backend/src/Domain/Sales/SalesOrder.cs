#nullable enable
using Domain;
using Domain.Abstractions;
using Domain.Entities;
using System;
using System.Collections.Generic;

namespace Domain.Sales;

public partial class SalesOrder : IBaseEntity
{
    public int Id { get; set; }

    public int CustomerId { get; set; }

    public DateTime OrderDate { get; set; }
    /// <summary>
    /// 1=Pending, 2=Completed, 3=Canceled
    /// </summary>
    public byte SalesStatus { get; set; }
    public DateTime? SalesStatusUpdatedAt { get; set; }

    public decimal TotalAmount { get; set; }

    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; }

    public int CreatedByUserId { get; set; }

    public virtual User CreatedByUser { get; set; } = null!;

    public virtual Customer Customer { get; set; } = null!;

}