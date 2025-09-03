using Application.DTOs.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Products.Response.Categories
{
    public sealed record ProductCategoryTreeReadResponse : BaseFullReadResponse
    {
        public string Name { get; init; } = string.Empty;
        public string? Description {  get; init; } = string.Empty;
        public ProductCategoryTreeReadResponse? Children { get; init; } = null;
    }
}
