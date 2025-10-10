using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Products.Request.Products
{
    public sealed record ProductCreateRequest
    {
        public string SKU { get; init; } = string.Empty;
        public string Name { get; init; } = string.Empty;
        public string? Description { get; init; } = string.Empty;
        public int CategoryId { get; init; }
        public int UnitOfMeasureId { get; init; }
        public decimal UnitPrice { get; init; }
        public decimal CostPrice { get; init; }

        // Inventory Details
        public int LocationId { get; init; }
        public decimal QuantityOnHand { get; init; }
        public decimal ReorderLevel { get; init; }
        public decimal MaxLevel { get; init; }
    }
}
