namespace Application.Sales.RequestResponse;

public sealed record CreateSalesOrderRequest(
    int? CustomerId,
    string? Description,
    bool IsWalkIn,
    string? ShippingAddress,
    IEnumerable<SalesOrderItemRequest> Items);
