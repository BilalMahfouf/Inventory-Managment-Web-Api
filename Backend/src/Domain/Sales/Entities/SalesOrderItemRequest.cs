using Domain.Products.Entities;
using Domain.Inventories.Entities;

namespace Domain.Sales.Entities;

public sealed record SalesOrderItemRequest()
{
    public int InventoryId { get; set; }
    public Inventory Inventory { get; set; } = null!;
    public Product Product { get; set; } = null!;
    public decimal Quantity { get; set; }
}
