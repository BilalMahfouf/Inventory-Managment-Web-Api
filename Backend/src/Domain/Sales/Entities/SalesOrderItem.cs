#nullable enable
using Domain;
using Domain.Inventories.Entities;
using Domain.Shared.Abstractions;
using Domain.Products.Entities;

namespace Domain.Sales.Entities;

public class SalesOrderItem : Entity
{
    public int SalesOrderId { get; private set; }

    public int ProductId { get; private set; }

    public int InventoryId { get; private set; }

    public int LocationId { get; private set; }

    public decimal OrderedQuantity { get; private set; }

    public decimal? ReceivedQuantity { get; private set; }

    public decimal UnitCost { get; private set; }

    public decimal LineAmount
    {
        get => OrderedQuantity * UnitCost;
        private set { }
    }
    public int CreatedByUserId { get; set; }

    public User CreatedByUser { get;private set; } = null!;

    public  Product Product { get; private set; } = null!;

    public Inventory Inventory { get; private set; } = null!;

    public  SalesOrder SalesOrder { get; private set; } = null!;

    private SalesOrderItem()
    {
    }
    internal SalesOrderItem(
        int productId,
        int inventoryId,
        int locationId,
        decimal orderedQuantity,
        decimal unitCost
        )
    {
        ProductId = productId;
        InventoryId = inventoryId;
        LocationId = locationId;
        OrderedQuantity = orderedQuantity;
        UnitCost = unitCost;
    }

}