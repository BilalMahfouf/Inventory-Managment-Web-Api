using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Products.Request.Products
{
    public sealed record ProductUpdateRequest
    {
        public string Name { get; init; } = string.Empty;
        public string? Description { get; init; } = string.Empty;
        public int CategoryId { get; init; }
        public decimal UnitPrice { get; init; }
        public decimal CostPrice { get; init; }
        public decimal LocationId { get; init;}
        public decimal QuantityOnHand { get; init; }
        public decimal ReorderLevel { get; init; }
        public decimal MaxLevel { get; init; }
    }
}
