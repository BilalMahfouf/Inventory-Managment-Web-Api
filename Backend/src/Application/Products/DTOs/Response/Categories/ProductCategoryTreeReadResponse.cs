using Application.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Products.DTOs.Response.Categories
{
    public sealed record ProductCategoryTreeReadResponse : BaseFullReadResponse
    {
        public string Name { get; init; } = string.Empty;
        public string? Description { get; init; } = string.Empty;
        public IReadOnlyCollection<ProductCategoryTreeReadResponse>? Children
        { get; init; } = null;
    }
}
