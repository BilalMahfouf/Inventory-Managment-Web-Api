using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.StockMovements.Request;



public sealed record StockTransferRequest
{
    public int ProductId { get; init; }
    public int FromLocationId { get; init; }
    public int ToLocationId { get; init; }
    public decimal Quantity { get; init; }
}
