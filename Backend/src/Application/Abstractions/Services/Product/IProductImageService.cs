using Application.DTOs.Products.Request.ProductImages;
using Application.DTOs.Products.Response.ProductImages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Abstractions.Services.Product
{
    public interface IProductImageService
    {
        Task<IReadOnlyCollection<ProductImageReadResponse>> GetProductImages(
            int productId,
            CancellationToken cancellationToken = default);
        Task<ProductImageReadResponse> AddProductImageAsync(int productId,
            ProductImageUploadRequest request,
            CancellationToken cancellationToken = default);
        Task DeleteProductImageAsync(int productId,int productImageId,
            CancellationToken cancellationToken = default);
        Task<ProductImageReadResponse> UpdateProductImageAsync(int productId
            , int ProductImageId, CancellationToken cancellationToken = default);
        Task SetProductImagePrimaryAsync(int productId
            , int productImageId, CancellationToken cancellationToken = default);
    }
}
