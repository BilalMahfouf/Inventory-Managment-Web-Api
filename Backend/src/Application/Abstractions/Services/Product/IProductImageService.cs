using Application.DTOs.Products.Request.ProductImages;
using Application.DTOs.Products.Response.ProductImages;
using Application.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Abstractions.Services.Product
{
    public interface IProductImageService
    {
        Task<Result<IReadOnlyCollection<ProductImageReadResponse>>> GetProductImages(
            int productId,
            CancellationToken cancellationToken = default);
        Task<Result<ProductImageReadResponse>> AddProductImageAsync(
            ProductImageUploadRequest request,
            CancellationToken cancellationToken = default);
        Task<Result> DeleteProductImageAsync(int id,
            CancellationToken cancellationToken = default);
       
        Task<Result> SetProductImagePrimaryAsync(int id,
            CancellationToken cancellationToken = default);
    }
}
