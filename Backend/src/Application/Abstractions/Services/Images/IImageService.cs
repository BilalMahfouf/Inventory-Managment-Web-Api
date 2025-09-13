using Application.DTOs.Images;
using Application.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Abstractions.Services.Images
{
    public interface IImageService
    {
        Task<Result<ImageDownloadResponse>> GetImageAsync(int id
            , CancellationToken cancellationToken = default);
    }
}
