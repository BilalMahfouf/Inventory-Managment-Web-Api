using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Sales.RequestResponse;

public sealed record SalesOrderTableResponse
{
    public int Id { get; init; }
    public string CustomerName { get; init; } = null!;
    public string CustomerEmail { get; init; } = null!;
    public DateTime OrderDate { get; init; }
    public decimal TotalAmount { get; init; }
    public int Items { get; init; }
    public string Status { get; init; } = null!;
}
