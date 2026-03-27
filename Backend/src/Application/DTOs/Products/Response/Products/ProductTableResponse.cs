using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Products.Response.Products
{
    public sealed record ProductTableResponse
    {
        public int Id { get; init; }
        public string SKU { get; init; } = string.Empty;
        public string Product { get; init; } = string.Empty;
        public decimal Stock { get; init; }
        public int CategoryId { get; init; }
        public string Category { get; init; } = string.Empty;
        public decimal Price { get; init; }
        public decimal Cost { get; init; }
        public bool IsActive { get; init; }
        public DateTime CreatedAt { get; init; }
    }
}
