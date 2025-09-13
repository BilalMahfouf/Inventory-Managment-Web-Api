using Application.DTOs.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Products.Response.ProductImages
{
    public sealed record ProductImageReadResponse
    {
        public int Id { get; init; }
        public string Url { get; init; } = null!;
        public string? Alt { get; init; }
        public bool IsPrimary { get; init; }
    }
}
