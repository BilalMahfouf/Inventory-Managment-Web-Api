using Application.Images.DTOs;
using Domain.Shared.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Images.Contracts
{
    public interface IImageService
    {
        Task<Result<ImageDownloadResponse>> GetImageAsync(int id
            , CancellationToken cancellationToken = default);
    }
}
