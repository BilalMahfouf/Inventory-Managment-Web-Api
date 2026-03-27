using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Sales.RequestResponse;

public sealed record SalesOrderReadResponse
{
    public int Id { get; init; }
    public int CustomerId { get; init; }
    public string CustomerName { get; init; } = string.Empty;
    public string CustomerEmail { get; init; } = string.Empty;
    public DateTime OrderDate { get; init; }
    public decimal TotalAmount { get; init; }
    public string SalesStatus { get; init; } = string.Empty;
    public IEnumerable<SalesOrderItemResponse> Items { get; init; } = null!;

}
