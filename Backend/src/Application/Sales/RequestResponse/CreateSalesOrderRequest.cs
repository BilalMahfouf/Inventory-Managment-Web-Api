namespace Application.Sales.RequestResponse;

public sealed record CreateSalesOrderRequest(
    int? CustomerId,
    string? Description,
    bool IsWalkIn,
    string? ShippingAddress,
    PaymentStatus PaymentStatus,
    IEnumerable<SalesOrderItemRequest> Items);
