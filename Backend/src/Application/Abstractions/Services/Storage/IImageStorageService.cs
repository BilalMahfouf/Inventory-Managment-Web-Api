using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Abstractions.Services.Storage
{
    public sealed record ImageUploadResponse()
    {
        public string StoragePath { get; init; } = string.Empty;
        public Uri? Uri { get; init; }
    }
    public interface IImageStorageService
    {
        Task<ImageUploadResponse> UploadAsync(Stream File, string fileName
           ,string category ,CancellationToken cancellationToken=default);
        Task<Stream> GetAsync(string storagePath
            , CancellationToken cancellationToken = default);
        Task DeleteAsync(string storagePath
            , CancellationToken cancellationToken = default);
    }
}
