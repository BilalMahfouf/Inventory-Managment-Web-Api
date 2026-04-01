namespace Application.Sales.RequestResponse;

public sealed record UpdateSalesOrderRequest(
    int? CustomerId,
    string? Description,
    string? ShippingAddress,
    IEnumerable<SalesOrderItemRequest>? Items);