using Application.Abstractions.Services.Storage;
using Microsoft.AspNetCore.Hosting;

namespace Infrastructure.Services.ImageStorage
{
    internal class FileSystemImageStorageService : IImageStorageService
    {
        private readonly string _rootPath;

        public FileSystemImageStorageService(IWebHostEnvironment env)
        {
            // Store files inside AppData/images
            _rootPath = Path.Combine(env.ContentRootPath, "AppData", "images");
        }

        public async Task<ImageUploadResponse> UploadAsync(
            Stream file,
            string fileName,
            string category,
            CancellationToken cancellationToken = default)
        {
            // Ensure category folder exists
            string categoryFolder = Path.Combine(_rootPath, category);
            Directory.CreateDirectory(categoryFolder);

            // Generate unique file name
            string uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(fileName)}";

            // Absolute full path on disk
            string fullPath = Path.Combine(categoryFolder, uniqueFileName);

            // Save to disk
            using (var fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write))
            {
                await file.CopyToAsync(fileStream, cancellationToken);
            }

            // Relative path to save in DB
            string relativePath = Path.Combine("images", category, uniqueFileName)
                                     .Replace("\\", "/");

            return new ImageUploadResponse
            {
                StoragePath = relativePath,
                Uri = null // You can set later if you expose via URL
            };
        }

        public Task<Stream> GetAsync(string storagePath, CancellationToken cancellationToken = default)
        {
            string fullPath = Path.Combine(_rootPath, storagePath.Replace("images/", ""));
            Stream stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read);
            return Task.FromResult(stream);
        }

        public Task DeleteAsync(string storagePath, CancellationToken cancellationToken = default)
        {
            string fullPath = Path.Combine(_rootPath, storagePath.Replace("images/", ""));
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
            return Task.CompletedTask;
        }
    }
}
