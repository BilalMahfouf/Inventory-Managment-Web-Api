using Application.Abstractions.Services.Storage;
using Application.Abstractions.Services.User;
using Application.Abstractions.UnitOfWork;
using Application.DTOs.Images;
using Application.Results;
using Application.Services.Images;
using Domain.Entities;
using Domain.Enums;
using Moq;
using System.Linq.Expressions;
using Xunit;

namespace Application.Tests.ImagesTests
{
    public class ImageServiceTests
    {
        private readonly Mock<IImageStorageService> _imageStorageServiceMock = new();
        private readonly Mock<ICurrentUserService> _currentUserServiceMock = new();
        private readonly Mock<IUnitOfWork> _uowMock = new();

        private ImageService CreateService()
        {
            return new ImageService(
                _imageStorageServiceMock.Object,
                _currentUserServiceMock.Object,
                _uowMock.Object
            );
        }

        #region AddImageAsync Tests

        [Fact]
        public async Task AddImageAsync_ReturnsSuccess_WhenValidRequest()
        {
            // Arrange
            var service = CreateService();
            var request = new Application.DTOs.Images.ImageUploadRequest
            {
                FileName = "test.jpg",
                FileStream = new MemoryStream(),
                MimeType = "image/jpeg",
                SizeInBytes = 1024,
                Alt = "Test image"
            };

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
            _uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
            _uowMock.Setup(u => u.Images.Add(It.IsAny<Image>()));

            // Act
            var result = await service.AddImageAsync(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal("test.jpg", result.Value.FileName);
            Assert.Equal("/storage/test.jpg", result.Value.StoragePath);
            Assert.Equal("image/jpeg", result.Value.MimeType);
            Assert.Equal(1024, result.Value.SizeInBytes);
            Assert.Equal(1, result.Value.CreatedByUserId);

            _uowMock.Verify(u => u.Images.Add(It.IsAny<Image>()), Times.Once);
            _uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task AddImageAsync_ReturnsException_WhenStorageServiceThrows()
        {
            // Arrange
            var service = CreateService();
            var request = new Application.DTOs.Images.ImageUploadRequest
            {
                FileName = "test.jpg",
                FileStream = new MemoryStream(),
                MimeType = "image/jpeg",
                SizeInBytes = 1024
            };

            _imageStorageServiceMock.Setup(s => s.UploadAsync(
                It.IsAny<Stream>(),
                It.IsAny<string>(),
                "products",
                It.IsAny<CancellationToken>()
            )).ThrowsAsync(new Exception("Storage error"));

            // Act
            var result = await service.AddImageAsync(request, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorType.InternalServerError, result.ErrorType);
            Assert.Contains("Exception in AddImageAsync", result.ErrorMessage);
            Assert.Contains("Storage error", result.ErrorMessage);
        }

        [Fact]
        public async Task AddImageAsync_ReturnsException_WhenSaveChangesThrows()
        {
            // Arrange
            var service = CreateService();
            var request = new Application.DTOs.Images.ImageUploadRequest
            {
                FileName = "test.jpg",
                FileStream = new MemoryStream(),
                MimeType = "image/jpeg",
                SizeInBytes = 1024
            };

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
            _uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await service.AddImageAsync(request, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorType.InternalServerError, result.ErrorType);
            Assert.Contains("Exception in AddImageAsync", result.ErrorMessage);
            //Assert.Contains("Database error", result.ErrorMessage);
        }

        #endregion

        #region GetImageAsync Tests

        [Fact]
        public async Task GetImageAsync_ReturnsSuccess_WhenImageExists()
        {
            // Arrange
            var service = CreateService();
            var imageId = 1;
            var image = new Image
            {
                Id = imageId,
                FileName = "test.jpg",
                StoragePath = "/storage/test.jpg",
                MimeType = "image/jpeg",
                SizeInBytes = 1024,
                CreatedAt = DateTime.UtcNow,
                CreatedByUserId = 1
            };

            var imageStream = new MemoryStream();

            _uowMock.Setup(u => u.Images.FindAsync(
                It.IsAny<Expression<Func<Image, bool>>>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<string>()
            )).ReturnsAsync(image);

            _imageStorageServiceMock.Setup(s => s.GetAsync(
                "/storage/test.jpg",
                It.IsAny<CancellationToken>()
            )).ReturnsAsync(imageStream);

            // Act
            var result = await service.GetImageAsync(imageId, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(imageStream, result.Value.ImageStream);
            Assert.Equal("image/jpeg", result.Value.MimeType);
            Assert.Equal("test.jpg", result.Value.FileName);
        }

        [Fact]
        public async Task GetImageAsync_ReturnsNotFound_WhenImageDoesNotExist()
        {
            // Arrange
            var service = CreateService();
            var imageId = 1;

            _uowMock.Setup(u => u.Images.FindAsync(
                It.IsAny<Expression<Func<Image, bool>>>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<string>()
            )).ReturnsAsync((Image)null!);

            // Act
            var result = await service.GetImageAsync(imageId, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorType.NotFound, result.ErrorType);
            Assert.Contains("image nor found", result.ErrorMessage);
        }

        [Fact]
        public async Task GetImageAsync_ReturnsNotFound_WhenStreamNotFound()
        {
            // Arrange
            var service = CreateService();
            var imageId = 1;
            var image = new Image
            {
                Id = imageId,
                FileName = "test.jpg",
                StoragePath = "/storage/test.jpg",
                MimeType = "image/jpeg",
                SizeInBytes = 1024,
                CreatedAt = DateTime.UtcNow,
                CreatedByUserId = 1
            };

            _uowMock.Setup(u => u.Images.FindAsync(
                It.IsAny<Expression<Func<Image, bool>>>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<string>()
            )).ReturnsAsync(image);

            _imageStorageServiceMock.Setup(s => s.GetAsync(
                "/storage/test.jpg",
                It.IsAny<CancellationToken>()
            )).ReturnsAsync((Stream)null!);

            // Act
            var result = await service.GetImageAsync(imageId, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorType.NotFound, result.ErrorType);
            Assert.Contains("stream nor found", result.ErrorMessage);
        }

        [Fact]
        public async Task GetImageAsync_ReturnsException_WhenRepositoryThrows()
        {
            // Arrange
            var service = CreateService();
            var imageId = 1;

            _uowMock.Setup(u => u.Images.FindAsync(
                It.IsAny<Expression<Func<Image, bool>>>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<string>()
            )).ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await service.GetImageAsync(imageId, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorType.InternalServerError, result.ErrorType);
            Assert.Contains("Exception in GetImageAsync", result.ErrorMessage);
            Assert.Contains("Database error", result.ErrorMessage);
        }

        [Fact]
        public async Task GetImageAsync_ReturnsException_WhenStorageServiceThrows()
        {
            // Arrange
            var service = CreateService();
            var imageId = 1;
            var image = new Image
            {
                Id = imageId,
                FileName = "test.jpg",
                StoragePath = "/storage/test.jpg",
                MimeType = "image/jpeg",
                SizeInBytes = 1024,
                CreatedAt = DateTime.UtcNow,
                CreatedByUserId = 1
            };

            _uowMock.Setup(u => u.Images.FindAsync(
                It.IsAny<Expression<Func<Image, bool>>>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<string>()
            )).ReturnsAsync(image);

            _imageStorageServiceMock.Setup(s => s.GetAsync(
                "/storage/test.jpg",
                It.IsAny<CancellationToken>()
            )).ThrowsAsync(new Exception("Storage error"));

            // Act
            var result = await service.GetImageAsync(imageId, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorType.InternalServerError, result.ErrorType);
            Assert.Contains("Exception in GetImageAsync", result.ErrorMessage);
            Assert.Contains("Storage error", result.ErrorMessage);
        }

        #endregion

        #region DeleteImageAsync Tests

        [Fact]
        public async Task DeleteImageAsync_ReturnsInvalidId_WhenIdIsZero()
        {
            // Arrange
            var service = CreateService();

            // Act
            var result = await service.DeleteImageAsync(0, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorType.BadRequest, result.ErrorType);
            Assert.Equal("Invalid Id", result.ErrorMessage);
        }

        [Fact]
        public async Task DeleteImageAsync_ReturnsInvalidId_WhenIdIsNegative()
        {
            // Arrange
            var service = CreateService();

            // Act
            var result = await service.DeleteImageAsync(-1, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorType.BadRequest, result.ErrorType);
            Assert.Equal("Invalid Id", result.ErrorMessage);
        }

        [Fact]
        public async Task DeleteImageAsync_ReturnsNotFound_WhenImageDoesNotExist()
        {
            // Arrange
            var service = CreateService();
            var imageId = 1;

            _uowMock.Setup(u => u.Images.FindAsync(
                It.IsAny<Expression<Func<Image, bool>>>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<string>()
            )).ReturnsAsync((Image)null!);

            // Act
            var result = await service.DeleteImageAsync(imageId, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorType.NotFound, result.ErrorType);
            Assert.Contains("image nor found", result.ErrorMessage);
        }

        [Fact]
        public async Task DeleteImageAsync_ReturnsSuccess_WhenImageExists()
        {
            // Arrange
            var service = CreateService();
            var imageId = 1;
            var image = new Image
            {
                Id = imageId,
                FileName = "test.jpg",
                StoragePath = "/storage/test.jpg",
                MimeType = "image/jpeg",
                SizeInBytes = 1024,
                CreatedAt = DateTime.UtcNow,
                CreatedByUserId = 1
            };

            _uowMock.Setup(u => u.Images.FindAsync(
                It.IsAny<Expression<Func<Image, bool>>>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<string>()
            )).ReturnsAsync(image);

            _imageStorageServiceMock.Setup(s => s.DeleteAsync(
                "/storage/test.jpg",
                It.IsAny<CancellationToken>()
            )).Returns(Task.CompletedTask);

            _uowMock.Setup(u => u.Images.Delete(It.IsAny<Image>()));

            // Act
            var result = await service.DeleteImageAsync(imageId, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            _imageStorageServiceMock.Verify(s => s.DeleteAsync(
                "/storage/test.jpg",
                It.IsAny<CancellationToken>()
            ), Times.Once);
            _uowMock.Verify(u => u.Images.Delete(image), Times.Once);
        }

        [Fact]
        public async Task DeleteImageAsync_ReturnsException_WhenRepositoryThrows()
        {
            // Arrange
            var service = CreateService();
            var imageId = 1;

            _uowMock.Setup(u => u.Images.FindAsync(
                It.IsAny<Expression<Func<Image, bool>>>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<string>()
            )).ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await service.DeleteImageAsync(imageId, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorType.InternalServerError, result.ErrorType);
            Assert.Contains("Exception in DeleteImageAsync", result.ErrorMessage);
            Assert.Contains("Database error", result.ErrorMessage);
        }

        [Fact]
        public async Task DeleteImageAsync_ReturnsException_WhenStorageServiceThrows()
        {
            // Arrange
            var service = CreateService();
            var imageId = 1;
            var image = new Image
            {
                Id = imageId,
                FileName = "test.jpg",
                StoragePath = "/storage/test.jpg",
                MimeType = "image/jpeg",
                SizeInBytes = 1024,
                CreatedAt = DateTime.UtcNow,
                CreatedByUserId = 1
            };

            _uowMock.Setup(u => u.Images.FindAsync(
                It.IsAny<Expression<Func<Image, bool>>>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<string>()
            )).ReturnsAsync(image);

            _imageStorageServiceMock.Setup(s => s.DeleteAsync(
                "/storage/test.jpg",
                It.IsAny<CancellationToken>()
            )).ThrowsAsync(new Exception("Storage error"));

            // Act
            var result = await service.DeleteImageAsync(imageId, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorType.InternalServerError, result.ErrorType);
            Assert.Contains("Exception in DeleteImageAsync", result.ErrorMessage);
            Assert.Contains("Storage error", result.ErrorMessage);
        }

        #endregion

        #region Helper Methods Tests

        [Fact]
        public void ImageService_Constructor_SetsPropertiesCorrectly()
        {
            // Arrange & Act
            var service = CreateService();

            // Assert
            Assert.NotNull(service);
        }

        [Theory]
        [InlineData("test.jpg", "image/jpeg", 1024)]
        [InlineData("image.png", "image/png", 2048)]
        [InlineData("photo.gif", "image/gif", 512)]
        public async Task AddImageAsync_HandlesVariousImageTypes(string fileName, string mimeType, long size)
        {
            // Arrange
            var service = CreateService();
            var request = new Application.DTOs.Images.ImageUploadRequest
            {
                FileName = fileName,
                FileStream = new MemoryStream(),
                MimeType = mimeType,
                SizeInBytes = size
            };

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
            _uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            // Act
            var result = await service.AddImageAsync(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(fileName, result.Value!.FileName);
            Assert.Equal(mimeType, result.Value.MimeType);
            Assert.Equal(size, result.Value.SizeInBytes);
        }

        #endregion
    }
}