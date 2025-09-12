using Application.Abstractions.Repositories.Base;
using Application.Abstractions.Services.Product;
using Application.Abstractions.Services.User;
using Application.Abstractions.UnitOfWork;
using Application.DTOs.Products.Request.ProductImages;
using Application.DTOs.Products.Response.ProductImages;
using Application.DTOs.Images;
using Application.Results;
using Application.Services.Images;
using Domain.Entities;
using Domain.Enums;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Products
{
    public class ProductImageService : IProductImageService
    {
        private readonly IUnitOfWork _uow;
        private readonly IValidator<ProductImageUploadRequest> _validator;
        private readonly ImageService _imageService;
        private readonly ICurrentUserService _currentUserService;
        public ProductImageService(IUnitOfWork uow
            , IValidator<ProductImageUploadRequest> validator
            , ImageService imageService
            , ICurrentUserService currentUserService)
        {
            _uow = uow;
            _validator = validator;
            _imageService = imageService;
            _currentUserService = currentUserService;
        }

        public async Task<Result<ProductImageReadResponse>> AddProductImageAsync(
            ProductImageUploadRequest request, 
            CancellationToken cancellationToken = default)
        {
            try
            {
                var result = await _validator.ValidateAsync(request,cancellationToken);
                if (!result.IsValid)
                {
                    var errorMessage = string.Join(", ", result
                        .Errors.Select(e => e.ErrorMessage));
                    return Result<ProductImageReadResponse>.Failure(errorMessage
                        , ErrorType.BadRequest);
                }
                var imageRequest = new Application.DTOs.Images.ImageUploadRequest()
                {
                    FileName = request.FileName,
                    FileStream = request.FileStream,
                    MimeType = request.MimeType,
                    SizeInBytes = request.FileSize,
                    Alt = request.Alt,

                };
                // ImageService => Upload image to server and save date in db 

                var imageResult = await _imageService.AddImageAsync(imageRequest
                    , cancellationToken);
                if (!imageResult.IsSuccess || imageResult.Value is null)
                {
                    return Result<ProductImageReadResponse>.Failure(
                         imageResult.ErrorMessage!
                        , imageResult.ErrorType);
                }
                var productImage = new ProductImage()
                {
                    ProductId = request.ProductId,
                    ImageId = imageResult.Value.Id,
                    IsPrimary = request.IsPrimary,
                    CreatedAt = DateTime.UtcNow,
                    CreatedByUserId = _currentUserService.UserId,
                };
                _uow.ProductImages.Add(productImage);
                await _uow.SaveChangesAsync(cancellationToken);
                var response = new ProductImageReadResponse()
                {
                    Id = productImage.Id,
                    IsPrimary = productImage.IsPrimary,
                    Url = $"api/images/{imageResult.Value.Id}",
                    Alt = null,
                };
                return Result<ProductImageReadResponse>.Success(response);
            }
            catch(Exception ex)
            {
                return Result<ProductImageReadResponse>.Exception(
                    nameof(AddProductImageAsync)
                    , ex);
            }


        }

        public async Task<Result> DeleteProductImageAsync(
            int id
            , CancellationToken cancellationToken = default)
        {
            if(id <= 0)
            {
                return Result.InvalidId();
            }
            try
            {
                var ProductImage = await _uow.ProductImages.FindAsync(e => e.Id == id
            , cancellationToken);
                if (ProductImage is null)
                {
                    return Result.NotFound(nameof(ProductImage));
                }
                var imageResult = await _imageService.DeleteImageAsync(id, cancellationToken);
                if (!imageResult.IsSuccess)
                {
                    return Result.Failure(imageResult.ErrorMessage!
                        , imageResult.ErrorType);
                }
                
                _uow.ProductImages.Delete(ProductImage);
                await _uow.SaveChangesAsync(cancellationToken);
                return Result.Success;

            }
            catch(Exception ex)
            {
                return Result.Exception(nameof(DeleteProductImageAsync), ex);
            }
        }

        public async Task<Result<IReadOnlyCollection<ProductImageReadResponse>>>
            GetProductImages(int productId
            , CancellationToken cancellationToken = default)
        {
            if (productId <= 0)
            {
                return Result<IReadOnlyCollection<ProductImageReadResponse>>.InvalidId();
            }
            try
            {
                var productImages = await _uow.ProductImages
                    .GetAllAsync(e => e.ProductId == productId
                    , cancellationToken);
                var response = productImages.Select(pi => new ProductImageReadResponse()
                {
                    Id = pi.Id,
                    IsPrimary = pi.IsPrimary,
                    Url = $"api/images/{pi.ImageId}",
                    Alt = null,
                }).ToList().AsReadOnly();
                return Result<IReadOnlyCollection<ProductImageReadResponse>>
                    .Success(response);
            }
            catch (Exception ex)
            {
                return Result<IReadOnlyCollection<ProductImageReadResponse>>.Exception(
                    nameof(GetProductImages), ex);
            } 
        }

        

        public async Task<Result> SetProductImagePrimaryAsync( int id
            , CancellationToken cancellationToken = default)
        {
            if (id <= 0)
            {
                return Result.InvalidId();
            }
            try
            {

                var productImage = await _uow.ProductImages.FindAsync(
                    e => e.Id == id, cancellationToken);
                if (productImage is null)
                {
                    return Result.NotFound(nameof(productImage));
                }

                var PrimaryProductImage = await _uow.ProductImages.FindAsync(
                    e => e.ProductId == productImage.ProductId && e.IsPrimary);
                if (PrimaryProductImage != null)
                {
                    PrimaryProductImage.IsPrimary = false;
                    _uow.ProductImages.Update(PrimaryProductImage);
                }
                productImage.IsPrimary = true;
                _uow.ProductImages.Update(productImage);
                await _uow.SaveChangesAsync(cancellationToken);
                return Result.Success;
            }
            catch(Exception ex)
            {
                return Result.Exception(nameof(SetProductImagePrimaryAsync),ex);
            }

        }

       
    }
}
