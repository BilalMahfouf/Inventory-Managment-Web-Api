using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Sales.RequestResponse;

public sealed record SalesOrderItemResponse
{
    public int Id { get; init; }
    public int ProductId { get; init; }
    public string ProductName { get; init; } = string.Empty;
    public decimal Quantity { get; init; }
    public decimal UnitPrice { get; init; }
    public decimal TotalPrice { get; init;}
}

