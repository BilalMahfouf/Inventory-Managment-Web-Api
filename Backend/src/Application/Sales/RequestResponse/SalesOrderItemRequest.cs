namespace Application.Sales.RequestResponse;

public sealed record SalesOrderItemRequest(
	int ProductId,
	int InventoryId,
	decimal Quantity);
