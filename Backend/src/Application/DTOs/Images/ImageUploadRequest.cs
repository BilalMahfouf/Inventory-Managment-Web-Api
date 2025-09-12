using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Images
{
    public sealed record ImageUploadRequest
    {
        public Stream FileStream { get; init; } = null!;
        public string FileName { get; init; } = string.Empty;
        public string MimeType { get; init; } = string.Empty;
        public long SizeInBytes { get; init; }
        public string? Alt { get; init; }
    }
}