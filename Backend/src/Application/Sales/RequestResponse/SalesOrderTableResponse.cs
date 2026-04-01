namespace Application.Sales.RequestResponse;

public sealed record SalesOrderTableResponse
{
    public int Id { get; init; }
    public int? CustomerId { get; init; }
    public string? CustomerName { get; init; }
    public string? CustomerEmail { get; init; }
    public bool IsWalkIn { get; init; }
    public DateTime OrderDate { get; init; }
    public decimal TotalAmount { get; init; }
    public int Items { get; init; }
    public string Status { get; init; } = null!;
    public string PaymentStatus { get; init; } = null!;
}
