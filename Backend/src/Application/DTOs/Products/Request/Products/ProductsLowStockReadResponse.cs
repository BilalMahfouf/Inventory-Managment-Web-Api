using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Products.Request.Products
{
    public sealed record ProductsLowStockReadResponse
    {
        //  ProductId,ProductName,LocationId,LocationName, QuantityOnHand, ReorderLevel
        public int ProductId { get; init; }
        public string ProductName { get; init; } = string.Empty;
        public int LocationId { get; init; }
        public string LocationName { get; init; } = string.Empty;
        public decimal QuantityOnHand { get; init; }
        public decimal ReorderLevel { get; init; }
    }
}
