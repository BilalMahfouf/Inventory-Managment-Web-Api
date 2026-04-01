namespace Application.Sales.RequestResponse;

public sealed record SalesOrderReadResponse
{
    public int Id { get; init; }
    public int? CustomerId { get; init; }
    public string? CustomerName { get; init; }
    public string? CustomerEmail { get; init; }
    public bool IsWalkIn { get; init; }
    public DateTime OrderDate { get; init; }
    public decimal TotalAmount { get; init; }
    public string SalesStatus { get; init; } = string.Empty;
    public string PaymentStatus { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string? ShippingAddress { get; init; }
    public string? TrackingNumber { get; init; }
    public IEnumerable<SalesOrderItemResponse> Items { get; init; } = null!;

}
