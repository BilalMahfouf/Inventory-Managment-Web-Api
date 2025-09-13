using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Products.Request.ProductImages
{
    public sealed record ProductImageUploadRequest
    {
        public int ProductId { get; init; }
        public Stream FileStream { get; init; } = null!;
        public string FileName { get; init; } = null!;
        public string MimeType { get; init; } = null!;
        public long FileSize { get; init; }
        public string? Alt { get; init; }
        public bool IsPrimary { get; init; }
    }
}
