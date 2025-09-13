using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Images
{
    public sealed record ImageDownloadResponse
    {
        public Stream ImageStream { get; init; } = null!;
        public string MimeType { get; init; } = string.Empty;
        public string FileName { get; init; } = string.Empty;

    }
}
