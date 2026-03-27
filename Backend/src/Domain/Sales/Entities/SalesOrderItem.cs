#nullable enable
using Domain;
using Domain.Shared.Abstractions;
using Domain.Products.Entities;
using System;
using System.Collections.Generic;

namespace Domain.Sales.Entities;

public class SalesOrderItem : IBaseEntity
{
    public int Id { get; private set; }

    public int SalesOrderId { get; private set; }

    public int ProductId { get; private set; }

    public decimal OrderedQuantity { get; private set; }

    public decimal? ReceivedQuantity { get; private set; }

    public decimal UnitCost { get; private set; }

    public decimal LineAmount
    {
        get => OrderedQuantity * UnitCost;
        private set { }
    }


    public DateTime CreatedAt { get; set; }

    public int CreatedByUserId { get; set; }

    public User CreatedByUser { get;private set; } = null!;

    public  Product Product { get; private set; } = null!;

    public  SalesOrder SalesOrder { get; private set; } = null!;

    private SalesOrderItem()
    {
    }
    internal SalesOrderItem(
        int productId,
        decimal orderedQuantity,
        decimal unitCost
        )
    {
        ProductId = productId;
        OrderedQuantity = orderedQuantity;
        UnitCost = unitCost;
    }


}