namespace Application.Sales.RequestResponse;

public sealed record SalesOrderItemRequest(
	int ProductId,
	int LocationId,
	decimal Quantity);
