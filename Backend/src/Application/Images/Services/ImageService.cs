using Application.Shared.Contracts;
using Application.Images.Contracts;
using Application.Images.Contracts;
using Application.Users.Contracts;
using Application.Shared.Contracts;
using Application.Images.DTOs;
using Domain.Shared.Results;
using Domain.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Application.Images.Services
{
    // this class use IImageStorage to store images and store image metadata in database
    public class ImageService : IImageService
    {
        private readonly IImageStorageService _imageStorageService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _uow;


        public ImageService(
              IImageStorageService imageStorageService
            , ICurrentUserService currentUserService
            , IUnitOfWork uow)
        {
            _imageStorageService = imageStorageService;
            _currentUserService = currentUserService;
            _uow = uow;
        }

        public async Task<Result<Image>> AddImageAsync(Application.Images.DTOs.ImageUploadRequest request
            , CancellationToken cancellationToken = default)
        {
            try
            {

                // to do validation for the request 

                // upload the image 
                var storageResponse = await _imageStorageService
                       .UploadAsync(request.FileStream, request.FileName, "products"
                       , cancellationToken);

                var image = new Image()
                {
                    FileName = request.FileName,
                    StoragePath = storageResponse.StoragePath,
                    MimeType = request.MimeType,
                    SizeInBytes = request.SizeInBytes,
                    CreatedAt = DateTime.UtcNow,
                    CreatedByUserId = _currentUserService.UserId
                };
                _uow.Images.Add(image);
                await _uow.SaveChangesAsync(cancellationToken);
                return Result<Image>.Success(image);
            }
            catch(Exception ex)
            {
                return Result<Image>.Failure(Error.Exception(nameof(AddImageAsync), ex));
            }
        }

        public async Task<Result<ImageDownloadResponse>> GetImageAsync(
              int id
            , CancellationToken cancellationToken = default)
        {
            try
            {
                var image = await _uow.Images.FindAsync(e => e.Id == id
                , cancellationToken);
                if(image is null)
                {
                    return Result<ImageDownloadResponse>.Failure(Error.NotFound(nameof(image)));
                }
                var stream = await _imageStorageService.GetAsync(
                    image.StoragePath, cancellationToken);
                if (stream is null)
                {
                    return Result<ImageDownloadResponse>.Failure(Error.NotFound(nameof(stream)));
                }
                var response = new ImageDownloadResponse()
                {
                    ImageStream = stream,
                    MimeType = image.MimeType,
                    FileName = image.FileName
                };
                return Result<ImageDownloadResponse>.Success(response); 
            }
            catch(Exception ex)
            {
                return Result<ImageDownloadResponse>.Failure(Error.Exception(nameof(GetImageAsync), ex));
            }
        }

        public async Task<Result> DeleteImageAsync(int id
            ,CancellationToken cancellationToken)
        {
            if(id <= 0)
            {
                return Result.Failure(Error.InvalidId());
            }
            try
            {
                var image = await _uow.Images.FindAsync(e => e.Id == id
                , cancellationToken);
                if(image is null)
                {
                    return Result.Failure(Error.NotFound(nameof(image)));
                }
                await _imageStorageService.DeleteAsync(image.StoragePath
                    , cancellationToken);
                _uow.Images.Delete(image);
                // here don't save changes until delete the entity that has the ImageId as fk
                return Result.Success;
            }
            catch(Exception ex)
            {
                return Result.Failure(Error.Exception(nameof(DeleteImageAsync), ex));
            }
        }

    }
}
