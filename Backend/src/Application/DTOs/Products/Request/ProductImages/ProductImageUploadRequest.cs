using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Products.Request.ProductImages
{
    public sealed record ProductImageUploadRequest
    {
        public Stream FileStream { get; set; } = null!;
        public string FileName { get; set; } = null!;
        public string? Alt { get; set; }
        public bool IsPrimary { get; set; }
    }
}
