using Application.DTOs.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Products.Response.Products
{
    public sealed record ProductReadResponse : BaseFullReadResponse
    {
        public string SKU { get; init ; }=string.Empty;
        public string Name { get; init; }=string.Empty;
        public string? Description { get; init; }=string.Empty;
        public int CategoryId { get; init; }
        public string CategoryName { get; init; }=string.Empty;
        public int UnitOfMeasureId { get; init; }
        public string UnitOfMeasureName { get; init; }=string.Empty;
        public decimal UnitPrice { get; init; } 
        public decimal CostPrice { get; init; }
        public bool IsActive { get; init; }
        public IEnumerable<object>? Inventories { get; init; } = null;
    }
}
