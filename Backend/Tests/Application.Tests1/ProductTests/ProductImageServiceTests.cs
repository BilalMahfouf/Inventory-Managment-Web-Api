using Application.Abstractions.Services.Product;
using Application.Abstractions.Services.Storage;
using Application.Abstractions.Services.User;
using Application.Abstractions.UnitOfWork;
using Application.DTOs.Images;
using Application.DTOs.Products.Request.ProductImages;
using Application.DTOs.Products.Response.ProductImages;
using Application.Results;
using Application.Services.Images;
using Application.Services.Products;
using Domain.Entities;
using Domain.Enums;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using System.Linq.Expressions;
using Xunit;

namespace Application.Tests.ProductTests
{
    public class ProductImageServiceTests
    {
        private readonly Mock<IUnitOfWork> _uowMock = new();
        private readonly Mock<IValidator<ProductImageUploadRequest>> _validatorMock = new();
        private readonly Mock<IImageStorageService> _imageStorageServiceMock = new();
        private readonly Mock<ICurrentUserService> _currentUserServiceMock = new();
        private ImageService _imageService = null!;

        private ProductImageService CreateService()
        {
            _imageService = new ImageService(
                _imageStorageServiceMock.Object,
                _currentUserServiceMock.Object,
                _uowMock.Object
            );

            return new ProductImageService(
                _uowMock.Object,
                _validatorMock.Object,
                _imageService,
                _currentUserServiceMock.Object
            );
        }

        #region AddProductImageAsync Tests

        [Fact]
        public async Task AddProductImageAsync_ReturnsSuccess_WhenValidRequest()
        {
            // Arrange
            var request = new ProductImageUploadRequest
            {
                ProductId = 1,
                FileStream = new MemoryStream(),
                FileName = "test.jpg",
                MimeType = "image/jpeg",
                FileSize = 1024,
                Alt = "Test image",
                IsPrimary = true
            };

            var validationResult = new ValidationResult();
            _validatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(validationResult);

            // Setup for ImageService
            var storageResponse = new ImageUploadResponse
            {
                StoragePath = "/storage/test.jpg",
                Uri = new Uri("https://example.com/test.jpg")
            };

            _imageStorageServiceMock.Setup(s => s.UploadAsync(
                It.IsAny<Stream>(),
                It.IsAny<string>(),
                "products",
                It.IsAny<CancellationToken>()
            )).ReturnsAsync(storageResponse);

            _currentUserServiceMock.SetupGet(c => c.UserId).Returns(1);
            _uowMock.Setup(u => u.Images.Add(It.IsAny<Image>()));
            _uowMock.Setup(u => u.ProductImages.Add(It.IsAny<ProductImage>()));
            _uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var service = CreateService();

            // Act
            var result = await service.AddProductImageAsync(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.True(result.Value.IsPrimary);
            Assert.Null(result.Value.Alt);

            _uowMock.Verify(u => u.ProductImages.Add(It.IsAny<ProductImage>()), Times.Once);
            _uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Exactly(2)); // Once for image, once for product image
        }

        [Fact]
        public async Task AddProductImageAsync_ReturnsFailure_WhenValidationFails()
        {
            // Arrange
            var request = new ProductImageUploadRequest
            {
                ProductId = 0, // Invalid product ID
                FileStream = new MemoryStream(),
                FileName = "",
                MimeType = "",
                FileSize = 0
            };

            var validationResult = new ValidationResult(new[]
            {
                new ValidationFailure("ProductId", "Product ID is required"),
                new ValidationFailure("FileName", "File name is required")
            });

            _validatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(validationResult);

            var service = CreateService();

            // Act
            var result = await service.AddProductImageAsync(request, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorType.BadRequest, result.ErrorType);
            Assert.Contains("Product ID is required", result.ErrorMessage);
            Assert.Contains("File name is required", result.ErrorMessage);

            _imageStorageServiceMock.Verify(s => s.UploadAsync(
                It.IsAny<Stream>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()
            ), Times.Never);
        }

        [Fact]
        public async Task AddProductImageAsync_ReturnsFailure_WhenImageServiceFails()
        {
            // Arrange
            var request = new ProductImageUploadRequest
            {
                ProductId = 1,
                FileStream = new MemoryStream(),
                FileName = "test.jpg",
                MimeType = "image/jpeg",
                FileSize = 1024,
                IsPrimary = false
            };

            var validationResult = new ValidationResult();
            _validatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(validationResult);

            _imageStorageServiceMock.Setup(s => s.UploadAsync(
                It.IsAny<Stream>(),
                It.IsAny<string>(),
                "products",
                It.IsAny<CancellationToken>()
            )).ThrowsAsync(new Exception("Storage error"));

            var service = CreateService();

            // Act
            var result = await service.AddProductImageAsync(request, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorType.InternalServerError, result.ErrorType);
            Assert.Contains("Exception in AddImageAsync", result.ErrorMessage);

            _uowMock.Verify(u => u.ProductImages.Add(It.IsAny<ProductImage>()), Times.Never);
        }

        [Fact]
        public async Task AddProductImageAsync_ReturnsException_WhenExceptionThrown()
        {
            // Arrange
            var request = new ProductImageUploadRequest
            {
                ProductId = 1,
                FileStream = new MemoryStream(),
                FileName = "test.jpg",
                MimeType = "image/jpeg",
                FileSize = 1024
            };

            _validatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Validation error"));

            var service = CreateService();

            // Act
            var result = await service.AddProductImageAsync(request, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorType.InternalServerError, result.ErrorType);
            Assert.Contains("Exception in AddProductImageAsync", result.ErrorMessage);
            Assert.Contains("Validation error", result.ErrorMessage);
        }

        #endregion

        #region DeleteProductImageAsync Tests

        [Fact]
        public async Task DeleteProductImageAsync_ReturnsInvalidId_WhenIdIsZero()
        {
            // Arrange
            var service = CreateService();

            // Act
            var result = await service.DeleteProductImageAsync(0, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorType.BadRequest, result.ErrorType);
            Assert.Equal("Invalid Id", result.ErrorMessage);
        }

        [Fact]
        public async Task DeleteProductImageAsync_ReturnsInvalidId_WhenIdIsNegative()
        {
            // Arrange
            var service = CreateService();

            // Act
            var result = await service.DeleteProductImageAsync(-1, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorType.BadRequest, result.ErrorType);
            Assert.Equal("Invalid Id", result.ErrorMessage);
        }

        [Fact]
        public async Task DeleteProductImageAsync_ReturnsNotFound_WhenProductImageDoesNotExist()
        {
            // Arrange
            var productImageId = 1;

            _uowMock.Setup(u => u.ProductImages.FindAsync(
                It.IsAny<Expression<Func<ProductImage, bool>>>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<string>()
            )).ReturnsAsync((ProductImage)null!);

            var service = CreateService();

            // Act
            var result = await service.DeleteProductImageAsync(productImageId, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorType.NotFound, result.ErrorType);
            Assert.Contains("ProductImage", result.ErrorMessage);
        }

        [Fact]
        public async Task DeleteProductImageAsync_ReturnsSuccess_WhenProductImageExists()
        {
            // Arrange
            var productImageId = 1;
            var productImage = new ProductImage
            {
                Id = productImageId,
                ProductId = 1,
                ImageId = 1,
                IsPrimary = false,
                CreatedAt = DateTime.UtcNow,
                CreatedByUserId = 1
            };

            var image = new Image
            {
                Id = 1,
                FileName = "test.jpg",
                StoragePath = "/storage/test.jpg",
                MimeType = "image/jpeg",
                SizeInBytes = 1024,
                CreatedAt = DateTime.UtcNow,
                CreatedByUserId = 1
            };

            _uowMock.Setup(u => u.ProductImages.FindAsync(
                It.IsAny<Expression<Func<ProductImage, bool>>>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<string>()
            )).ReturnsAsync(productImage);

            _uowMock.Setup(u => u.Images.FindAsync(
                It.IsAny<Expression<Func<Image, bool>>>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<string>()
            )).ReturnsAsync(image);

            _imageStorageServiceMock.Setup(s => s.DeleteAsync(
                "/storage/test.jpg",
                It.IsAny<CancellationToken>()
            )).Returns(Task.CompletedTask);

            _uowMock.Setup(u => u.ProductImages.Delete(It.IsAny<ProductImage>()));
            _uowMock.Setup(u => u.Images.Delete(It.IsAny<Image>()));
            _uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var service = CreateService();

            // Act
            var result = await service.DeleteProductImageAsync(productImageId, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);

            _uowMock.Verify(u => u.ProductImages.Delete(productImage), Times.Once);
            _uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task DeleteProductImageAsync_ReturnsException_WhenExceptionThrown()
        {
            // Arrange
            var productImageId = 1;

            _uowMock.Setup(u => u.ProductImages.FindAsync(
                It.IsAny<Expression<Func<ProductImage, bool>>>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<string>()
            )).ThrowsAsync(new Exception("Database error"));

            var service = CreateService();

            // Act
            var result = await service.DeleteProductImageAsync(productImageId, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorType.InternalServerError, result.ErrorType);
            Assert.Contains("Exception in DeleteProductImageAsync", result.ErrorMessage);
            Assert.Contains("Database error", result.ErrorMessage);
        }

        #endregion

        #region GetProductImages Tests

        [Fact]
        public async Task GetProductImages_ReturnsInvalidId_WhenProductIdIsZero()
        {
            // Arrange
            var service = CreateService();

            // Act
            var result = await service.GetProductImages(0, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorType.BadRequest, result.ErrorType);
            Assert.Equal("Invalid Id", result.ErrorMessage);
        }

        [Fact]
        public async Task GetProductImages_ReturnsInvalidId_WhenProductIdIsNegative()
        {
            // Arrange
            var service = CreateService();

            // Act
            var result = await service.GetProductImages(-1, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorType.BadRequest, result.ErrorType);
            Assert.Equal("Invalid Id", result.ErrorMessage);
        }

        [Fact]
        public async Task GetProductImages_ReturnsSuccess_WhenProductImagesExist()
        {
            // Arrange
            var productId = 1;
            var productImages = new List<ProductImage>
            {
                new ProductImage
                {
                    Id = 1,
                    ProductId = productId,
                    ImageId = 1,
                    IsPrimary = true,
                    CreatedAt = DateTime.UtcNow,
                    CreatedByUserId = 1
                },
                new ProductImage
                {
                    Id = 2,
                    ProductId = productId,
                    ImageId = 2,
                    IsPrimary = false,
                    CreatedAt = DateTime.UtcNow,
                    CreatedByUserId = 1
                }
            };

            _uowMock.Setup(u => u.ProductImages.GetAllAsync(
                It.IsAny<Expression<Func<ProductImage, bool>>>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<string>()
            )).ReturnsAsync(productImages);

            var service = CreateService();

            // Act
            var result = await service.GetProductImages(productId, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(2, result.Value.Count);

            var primaryImage = result.Value.First(x => x.IsPrimary);
            Assert.Equal(1, primaryImage.Id);
            Assert.Equal("api/images/1", primaryImage.Url);
            Assert.True(primaryImage.IsPrimary);

            var secondaryImage = result.Value.First(x => !x.IsPrimary);
            Assert.Equal(2, secondaryImage.Id);
            Assert.Equal("api/images/2", secondaryImage.Url);
            Assert.False(secondaryImage.IsPrimary);
        }

        [Fact]
        public async Task GetProductImages_ReturnsEmptyCollection_WhenNoProductImagesExist()
        {
            // Arrange
            var productId = 1;
            var productImages = new List<ProductImage>();

            _uowMock.Setup(u => u.ProductImages.GetAllAsync(
                It.IsAny<Expression<Func<ProductImage, bool>>>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<string>()
            )).ReturnsAsync(productImages);

            var service = CreateService();

            // Act
            var result = await service.GetProductImages(productId, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Empty(result.Value);
        }

        [Fact]
        public async Task GetProductImages_ReturnsException_WhenExceptionThrown()
        {
            // Arrange
            var productId = 1;

            _uowMock.Setup(u => u.ProductImages.GetAllAsync(
                It.IsAny<Expression<Func<ProductImage, bool>>>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<string>()
            )).ThrowsAsync(new Exception("Database error"));

            var service = CreateService();

            // Act
            var result = await service.GetProductImages(productId, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorType.InternalServerError, result.ErrorType);
            Assert.Contains("Exception in GetProductImages", result.ErrorMessage);
            Assert.Contains("Database error", result.ErrorMessage);
        }

        #endregion

        #region SetProductImagePrimaryAsync Tests

        [Fact]
        public async Task SetProductImagePrimaryAsync_ReturnsInvalidId_WhenIdIsZero()
        {
            // Arrange
            var service = CreateService();

            // Act
            var result = await service.SetProductImagePrimaryAsync(0, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorType.BadRequest, result.ErrorType);
            Assert.Equal("Invalid Id", result.ErrorMessage);
        }

        [Fact]
        public async Task SetProductImagePrimaryAsync_ReturnsInvalidId_WhenIdIsNegative()
        {
            // Arrange
            var service = CreateService();

            // Act
            var result = await service.SetProductImagePrimaryAsync(-1, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorType.BadRequest, result.ErrorType);
            Assert.Equal("Invalid Id", result.ErrorMessage);
        }

        [Fact]
        public async Task SetProductImagePrimaryAsync_ReturnsNotFound_WhenProductImageDoesNotExist()
        {
            // Arrange
            var productImageId = 1;

            _uowMock.Setup(u => u.ProductImages.FindAsync(
                It.IsAny<Expression<Func<ProductImage, bool>>>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<string>()
            )).ReturnsAsync((ProductImage)null!);

            var service = CreateService();

            // Act
            var result = await service.SetProductImagePrimaryAsync(productImageId, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorType.NotFound, result.ErrorType);
            Assert.Contains("productImage", result.ErrorMessage);
        }

        [Fact]
        public async Task SetProductImagePrimaryAsync_ReturnsSuccess_WhenNoPrimaryImageExists()
        {
            // Arrange
            var productImageId = 1;
            var productImage = new ProductImage
            {
                Id = productImageId,
                ProductId = 1,
                ImageId = 1,
                IsPrimary = false,
                CreatedAt = DateTime.UtcNow,
                CreatedByUserId = 1
            };

            _uowMock.SetupSequence(u => u.ProductImages.FindAsync(
                It.IsAny<Expression<Func<ProductImage, bool>>>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<string>()
            ))
                .ReturnsAsync(productImage) // First call to find the target image
                .ReturnsAsync((ProductImage)null!); // Second call to find existing primary image

            _uowMock.Setup(u => u.ProductImages.Update(It.IsAny<ProductImage>()));
            _uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var service = CreateService();

            // Act
            var result = await service.SetProductImagePrimaryAsync(productImageId, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);

            _uowMock.Verify(u => u.ProductImages.Update(It.Is<ProductImage>(pi => pi.IsPrimary)), Times.Once);
            _uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task SetProductImagePrimaryAsync_ReturnsSuccess_WhenPrimaryImageExists()
        {
            // Arrange
            var productImageId = 1;
            var productImage = new ProductImage
            {
                Id = productImageId,
                ProductId = 1,
                ImageId = 1,
                IsPrimary = false,
                CreatedAt = DateTime.UtcNow,
                CreatedByUserId = 1
            };

            var currentPrimaryImage = new ProductImage
            {
                Id = 2,
                ProductId = 1,
                ImageId = 2,
                IsPrimary = true,
                CreatedAt = DateTime.UtcNow,
                CreatedByUserId = 1
            };

            _uowMock.SetupSequence(u => u.ProductImages.FindAsync(
                It.IsAny<Expression<Func<ProductImage, bool>>>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<string>()
            ))
                .ReturnsAsync(productImage) // First call to find the target image
                .ReturnsAsync(currentPrimaryImage); // Second call to find existing primary image

            _uowMock.Setup(u => u.ProductImages.Update(It.IsAny<ProductImage>()));
            _uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var service = CreateService();

            // Act
            var result = await service.SetProductImagePrimaryAsync(productImageId, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);

            // Verify that the current primary image is set to false
            _uowMock.Verify(u => u.ProductImages.Update(It.Is<ProductImage>(pi => pi.Id == 2 && !pi.IsPrimary)), Times.Once);
            // Verify that the target image is set to true
            _uowMock.Verify(u => u.ProductImages.Update(It.Is<ProductImage>(pi => pi.Id == 1 && pi.IsPrimary)), Times.Once);
            _uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task SetProductImagePrimaryAsync_ReturnsException_WhenExceptionThrown()
        {
            // Arrange
            var productImageId = 1;

            _uowMock.Setup(u => u.ProductImages.FindAsync(
                It.IsAny<Expression<Func<ProductImage, bool>>>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<string>()
            )).ThrowsAsync(new Exception("Database error"));

            var service = CreateService();

            // Act
            var result = await service.SetProductImagePrimaryAsync(productImageId, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorType.InternalServerError, result.ErrorType);
            Assert.Contains("Exception in SetProductImagePrimaryAsync", result.ErrorMessage);
            Assert.Contains("Database error", result.ErrorMessage);
        }

        #endregion

        #region Helper Methods Tests

        [Fact]
        public void ProductImageService_Constructor_SetsPropertiesCorrectly()
        {
            // Arrange & Act
            var service = CreateService();

            // Assert
            Assert.NotNull(service);
        }

        [Theory]
        [InlineData(1, "test.jpg", "image/jpeg", 1024, true)]
        [InlineData(2, "image.png", "image/png", 2048, false)]
        [InlineData(3, "photo.gif", "image/gif", 512, true)]
        public async Task AddProductImageAsync_HandlesVariousImageTypes(int productId, string fileName, string mimeType, long fileSize, bool isPrimary)
        {
            // Arrange
            var request = new ProductImageUploadRequest
            {
                ProductId = productId,
                FileStream = new MemoryStream(),
                FileName = fileName,
                MimeType = mimeType,
                FileSize = fileSize,
                IsPrimary = isPrimary
            };

            var validationResult = new ValidationResult();
            _validatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(validationResult);

            var storageResponse = new ImageUploadResponse
            {
                StoragePath = $"/storage/{fileName}"
            };

            _imageStorageServiceMock.Setup(s => s.UploadAsync(
                It.IsAny<Stream>(),
                It.IsAny<string>(),
                "products",
                It.IsAny<CancellationToken>()
            )).ReturnsAsync(storageResponse);

            _currentUserServiceMock.SetupGet(c => c.UserId).Returns(1);
            _uowMock.Setup(u => u.Images.Add(It.IsAny<Image>()));
            _uowMock.Setup(u => u.ProductImages.Add(It.IsAny<ProductImage>()));
            _uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var service = CreateService();

            // Act
            var result = await service.AddProductImageAsync(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(isPrimary, result.Value.IsPrimary);
        }

        #endregion

        #region Edge Cases

        [Fact]
        public async Task AddProductImageAsync_HandlesNullAltText()
        {
            // Arrange
            var request = new ProductImageUploadRequest
            {
                ProductId = 1,
                FileStream = new MemoryStream(),
                FileName = "test.jpg",
                MimeType = "image/jpeg",
                FileSize = 1024,
                Alt = null,
                IsPrimary = false
            };

            var validationResult = new ValidationResult();
            _validatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(validationResult);

            var storageResponse = new ImageUploadResponse
            {
                StoragePath = "/storage/test.jpg"
            };

            _imageStorageServiceMock.Setup(s => s.UploadAsync(
                It.IsAny<Stream>(),
                It.IsAny<string>(),
                "products",
                It.IsAny<CancellationToken>()
            )).ReturnsAsync(storageResponse);

            _currentUserServiceMock.SetupGet(c => c.UserId).Returns(1);
            _uowMock.Setup(u => u.Images.Add(It.IsAny<Image>()));
            _uowMock.Setup(u => u.ProductImages.Add(It.IsAny<ProductImage>()));
            _uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var service = CreateService();

            // Act
            var result = await service.AddProductImageAsync(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Null(result.Value.Alt);
        }

        [Fact]
        public async Task SetProductImagePrimaryAsync_HandlesAlreadyPrimaryImage()
        {
            // Arrange
            var productImageId = 1;
            var productImage = new ProductImage
            {
                Id = productImageId,
                ProductId = 1,
                ImageId = 1,
                IsPrimary = true, // Already primary
                CreatedAt = DateTime.UtcNow,
                CreatedByUserId = 1
            };

            _uowMock.SetupSequence(u => u.ProductImages.FindAsync(
                It.IsAny<Expression<Func<ProductImage, bool>>>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<string>()
            ))
                .ReturnsAsync(productImage) // First call to find the target image
                .ReturnsAsync(productImage); // Second call finds the same image as primary

            _uowMock.Setup(u => u.ProductImages.Update(It.IsAny<ProductImage>()));
            _uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var service = CreateService();

            // Act
            var result = await service.SetProductImagePrimaryAsync(productImageId, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);

            // Verify that the same image gets updated (set to false first, then true)
            _uowMock.Verify(u => u.ProductImages.Update(It.IsAny<ProductImage>()), Times.Exactly(2));
            _uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion
    }
}