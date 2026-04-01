namespace Application.Sales.RequestResponse;

public sealed record SalesOrderItemResponse
{
    public int Id { get; init; }
    public int ProductId { get; init; }
    public int InventoryId { get; init; }
    public int LocationId { get; init; }
    public string? LocationName { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public decimal Quantity { get; init; }
    public decimal UnitPrice { get; init; }
    public decimal TotalPrice { get; init;}
}

