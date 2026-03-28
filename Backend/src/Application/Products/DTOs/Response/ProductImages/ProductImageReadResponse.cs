using Application.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Products.DTOs.Response.ProductImages
{
    public sealed record ProductImageReadResponse
    {
        public int Id { get; init; }
        public string Url { get; init; } = null!;
        public string? Alt { get; init; }
        public bool IsPrimary { get; init; }
    }
}
