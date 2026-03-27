using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Products.Response.Products;

public sealed record StockMovementsHistoryTableResponse()
{
    public int Id { get;init; }
    public string Product { get;init; } = string.Empty;
    public string Sku { get;init; } = string.Empty;
    public string Type { get;init; } = string.Empty;
    public decimal Quantity { get;init; }
    public string Reason { get;init; } = string.Empty;
    public DateTime CreatedAt { get;init; }
    public string CreatedByUser { get;init; } = string.Empty;
}
