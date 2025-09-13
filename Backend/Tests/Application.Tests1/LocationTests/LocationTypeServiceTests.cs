using Application.Abstractions.Services.User;
using Application.Abstractions.UnitOfWork;
using Application.DTOs.Locations.Request;
using Application.DTOs.Locations.Response;
using Application.Results;
using Application.Services.Locations;
using Domain.Entities;
using Domain.Enums;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using System.Linq.Expressions;
using Xunit;

namespace Application.Tests.LocationTests
{
    public class LocationTypeServiceTests
    {
        private readonly Mock<IUnitOfWork> _uowMock = new();
        private readonly Mock<IValidator<LocationTypeCreateRequest>> _validatorMock = new();
        private readonly Mock<ICurrentUserService> _currentUserServiceMock = new();

        private LocationTypeService CreateService()
        {
            return new LocationTypeService(
                _uowMock.Object,
                _validatorMock.Object,
                _currentUserServiceMock.Object
            );
        }

        #region AddLocationTypeAsync Tests

        [Fact]
        public async Task AddLocationTypeAsync_ReturnsSuccess_WhenValidRequest()
        {
            // Arrange
            var request = new LocationTypeCreateRequest
            {
                Name = "Warehouse",
                Description = "Storage warehouse location"
            };

            var validationResult = new ValidationResult();
            _validatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(validationResult);

            var savedLocationType = new LocationType
            {
                Id = 1,
                Name = request.Name,
                Description = request.Description,
                CreatedAt = DateTime.UtcNow,
                CreatedByUserId = 1,
                IsDeleted = false,
                CreatedByUser = new User { UserName = "testuser" }
            };

            _currentUserServiceMock.SetupGet(c => c.UserId).Returns(1);
            
            // Setup to simulate the ID being set after Add
            _uowMock.Setup(u => u.LocationTypes.Add(It.IsAny<LocationType>()))
                .Callback<LocationType>(lt => lt.Id = 1);
            
            _uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
            
            // Setup for FindAsync call at the end of AddLocationTypeAsync
            _uowMock.Setup(u => u.LocationTypes.FindAsync(
                It.IsAny<Expression<Func<LocationType, bool>>>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<string>()
            )).ReturnsAsync(savedLocationType);

            var service = CreateService();

            // Act
            var result = await service.AddLocationTypeAsync(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(request.Name, result.Value.Name);
            Assert.Equal(request.Description, result.Value.Description);
            Assert.Equal(1, result.Value.Id);
            Assert.Equal(1, result.Value.CreatedByUserId);
            Assert.False(result.Value.IsDeleted);

            _uowMock.Verify(u => u.LocationTypes.Add(It.IsAny<LocationType>()), Times.Once);
            _uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task AddLocationTypeAsync_ReturnsFailure_WhenValidationFails()
        {
            // Arrange
            var request = new LocationTypeCreateRequest
            {
                Name = "", // Invalid empty name
                Description = "Test description"
            };

            var validationResult = new ValidationResult(new[]
            {
                new ValidationFailure("Name", "Name is required."),
            });

            _validatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(validationResult);

            var service = CreateService();

            // Act
            var result = await service.AddLocationTypeAsync(request, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorType.BadRequest, result.ErrorType);
            Assert.Contains("Name is required.", result.ErrorMessage);

            _uowMock.Verify(u => u.LocationTypes.Add(It.IsAny<LocationType>()), Times.Never);
            _uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task AddLocationTypeAsync_ReturnsFailure_WhenMultipleValidationErrorsExist()
        {
            // Arrange
            var request = new LocationTypeCreateRequest
            {
                Name = "",
                Description = null
            };

            var validationResult = new ValidationResult(new[]
            {
                new ValidationFailure("Name", "Name is required."),
                new ValidationFailure("Name", "Name must be unique.")
            });

            _validatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(validationResult);

            var service = CreateService();

            // Act
            var result = await service.AddLocationTypeAsync(request, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorType.BadRequest, result.ErrorType);
            Assert.Contains("Name is required.", result.ErrorMessage);
            Assert.Contains("Name must be unique.", result.ErrorMessage);
            Assert.Contains(";", result.ErrorMessage); // Multiple errors separated by semicolon
        }

        [Fact]
        public async Task AddLocationTypeAsync_ReturnsException_WhenValidatorThrows()
        {
            // Arrange
            var request = new LocationTypeCreateRequest
            {
                Name = "Test",
                Description = "Test description"
            };

            _validatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Validation error"));

            var service = CreateService();

            // Act
            var result = await service.AddLocationTypeAsync(request, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorType.InternalServerError, result.ErrorType);
            Assert.Contains("Exception in AddLocationTypeAsync", result.ErrorMessage);
            Assert.Contains("Validation error", result.ErrorMessage);
        }

        [Fact]
        public async Task AddLocationTypeAsync_ReturnsException_WhenSaveChangesThrows()
        {
            // Arrange
            var request = new LocationTypeCreateRequest
            {
                Name = "Test",
                Description = "Test description"
            };

            var validationResult = new ValidationResult();
            _validatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(validationResult);

            _currentUserServiceMock.SetupGet(c => c.UserId).Returns(1);
            _uowMock.Setup(u => u.LocationTypes.Add(It.IsAny<LocationType>()));
            _uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Database error"));

            var service = CreateService();

            // Act
            var result = await service.AddLocationTypeAsync(request, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorType.InternalServerError, result.ErrorType);
            Assert.Contains("Exception in AddLocationTypeAsync", result.ErrorMessage);
            Assert.Contains("Database error", result.ErrorMessage);
        }

        [Fact]
        public async Task AddLocationTypeAsync_HandlesNullDescription()
        {
            // Arrange
            var request = new LocationTypeCreateRequest
            {
                Name = "Test Location",
                Description = null
            };

            var validationResult = new ValidationResult();
            _validatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(validationResult);

            var savedLocationType = new LocationType
            {
                Id = 1,
                Name = request.Name,
                Description = null,
                CreatedAt = DateTime.UtcNow,
                CreatedByUserId = 1,
                IsDeleted = false,
                CreatedByUser = new User { UserName = "testuser" }
            };

            _currentUserServiceMock.SetupGet(c => c.UserId).Returns(1);
            
            // Setup to simulate the ID being set after Add
            _uowMock.Setup(u => u.LocationTypes.Add(It.IsAny<LocationType>()))
                .Callback<LocationType>(lt => lt.Id = 1);
            
            _uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
            _uowMock.Setup(u => u.LocationTypes.FindAsync(
                It.IsAny<Expression<Func<LocationType, bool>>>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<string>()
            )).ReturnsAsync(savedLocationType);

            var service = CreateService();

            // Act
            var result = await service.AddLocationTypeAsync(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(request.Name, result.Value.Name);
            Assert.Null(result.Value.Description);
        }

        #endregion

        #region GetAllLocationTypesAsync Tests

        [Fact]
        public async Task GetAllLocationTypesAsync_ReturnsSuccess_WhenLocationTypesExist()
        {
            // Arrange
            var locationTypes = new List<LocationType>
            {
                new LocationType
                {
                    Id = 1,
                    Name = "Warehouse",
                    Description = "Storage warehouse",
                    CreatedAt = DateTime.UtcNow,
                    CreatedByUserId = 1,
                    IsDeleted = false,
                    CreatedByUser = new User { UserName = "user1" }
                },
                new LocationType
                {
                    Id = 2,
                    Name = "Office",
                    Description = "Office location",
                    CreatedAt = DateTime.UtcNow,
                    CreatedByUserId = 2,
                    IsDeleted = false,
                    CreatedByUser = new User { UserName = "user2" }
                }
            };

            _uowMock.Setup(u => u.LocationTypes.GetAllAsync(
                It.IsAny<Expression<Func<LocationType, bool>>>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<string>()
            )).ReturnsAsync(locationTypes);

            var service = CreateService();

            // Act
            var result = await service.GetAllLocationTypesAsync(CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(2, result.Value.Count);

            var warehouseType = result.Value.First(x => x.Name == "Warehouse");
            Assert.Equal(1, warehouseType.Id);
            Assert.Equal("Storage warehouse", warehouseType.Description);
            Assert.Equal("user1", warehouseType.CreatedByUserName);

            var officeType = result.Value.First(x => x.Name == "Office");
            Assert.Equal(2, officeType.Id);
            Assert.Equal("Office location", officeType.Description);
            Assert.Equal("user2", officeType.CreatedByUserName);
        }

        [Fact]
        public async Task GetAllLocationTypesAsync_ReturnsEmptyCollection_WhenNoLocationTypesExist()
        {
            // Arrange
            var locationTypes = new List<LocationType>();

            _uowMock.Setup(u => u.LocationTypes.GetAllAsync(
                It.IsAny<Expression<Func<LocationType, bool>>>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<string>()
            )).ReturnsAsync(locationTypes);

            var service = CreateService();

            // Act
            var result = await service.GetAllLocationTypesAsync(CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Empty(result.Value);
        }

        [Fact]
        public async Task GetAllLocationTypesAsync_ReturnsException_WhenRepositoryThrows()
        {
            // Arrange
            _uowMock.Setup(u => u.LocationTypes.GetAllAsync(
                It.IsAny<Expression<Func<LocationType, bool>>>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<string>()
            )).ThrowsAsync(new Exception("Database error"));

            var service = CreateService();

            // Act
            var result = await service.GetAllLocationTypesAsync(CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorType.InternalServerError, result.ErrorType);
            Assert.Contains("Exception in GetAllLocationTypesAsync", result.ErrorMessage);
            Assert.Contains("Database error", result.ErrorMessage);
        }

        [Fact]
        public async Task GetAllLocationTypesAsync_OnlyReturnsNonDeletedItems()
        {
            // Arrange
            var locationTypes = new List<LocationType>
            {
                new LocationType
                {
                    Id = 1,
                    Name = "Active Location",
                    Description = "Active",
                    CreatedAt = DateTime.UtcNow,
                    CreatedByUserId = 1,
                    IsDeleted = false,
                    CreatedByUser = new User { UserName = "user1" }
                }
                // Note: deleted items should be filtered out by repository call
            };

            _uowMock.Setup(u => u.LocationTypes.GetAllAsync(
                It.IsAny<Expression<Func<LocationType, bool>>>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<string>()
            )).ReturnsAsync(locationTypes);

            var service = CreateService();

            // Act
            var result = await service.GetAllLocationTypesAsync(CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Single(result.Value!);
            Assert.All(result.Value, lt => Assert.False(lt.IsDeleted));

            // Verify the filter is applied correctly
            _uowMock.Verify(u => u.LocationTypes.GetAllAsync(
                It.Is<Expression<Func<LocationType, bool>>>(expr => expr != null),
                It.IsAny<CancellationToken>(),
                "CreatedByUser,DeletedByUser"
            ), Times.Once);
        }

        #endregion

        #region FindAsync Tests

        [Fact]
        public async Task FindAsync_ReturnsSuccess_WhenLocationTypeExists()
        {
            // Arrange
            var locationTypeId = 1;
            var locationType = new LocationType
            {
                Id = locationTypeId,
                Name = "Test Location",
                Description = "Test description",
                CreatedAt = DateTime.UtcNow,
                CreatedByUserId = 1,
                IsDeleted = false,
                CreatedByUser = new User { UserName = "testuser" },
                DeletedByUser = null
            };

            _uowMock.Setup(u => u.LocationTypes.FindAsync(
                It.IsAny<Expression<Func<LocationType, bool>>>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<string>()
            )).ReturnsAsync(locationType);

            var service = CreateService();

            // Act
            var result = await service.FindAsync(locationTypeId, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(locationTypeId, result.Value.Id);
            Assert.Equal("Test Location", result.Value.Name);
            Assert.Equal("Test description", result.Value.Description);
            Assert.Equal("testuser", result.Value.CreatedByUserName);
            Assert.False(result.Value.IsDeleted);
        }

        [Fact]
        public async Task FindAsync_ReturnsInvalidId_WhenIdIsZero()
        {
            // Arrange
            var service = CreateService();

            // Act
            var result = await service.FindAsync(0, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorType.BadRequest, result.ErrorType);
            Assert.Equal("Invalid Id", result.ErrorMessage);

            _uowMock.Verify(u => u.LocationTypes.FindAsync(
                It.IsAny<Expression<Func<LocationType, bool>>>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<string>()
            ), Times.Never);
        }

        [Fact]
        public async Task FindAsync_ReturnsInvalidId_WhenIdIsNegative()
        {
            // Arrange
            var service = CreateService();

            // Act
            var result = await service.FindAsync(-1, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorType.BadRequest, result.ErrorType);
            Assert.Equal("Invalid Id", result.ErrorMessage);

            _uowMock.Verify(u => u.LocationTypes.FindAsync(
                It.IsAny<Expression<Func<LocationType, bool>>>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<string>()
            ), Times.Never);
        }

        [Fact]
        public async Task FindAsync_ReturnsNotFound_WhenLocationTypeDoesNotExist()
        {
            // Arrange
            var locationTypeId = 1;

            _uowMock.Setup(u => u.LocationTypes.FindAsync(
                It.IsAny<Expression<Func<LocationType, bool>>>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<string>()
            )).ReturnsAsync((LocationType)null!);

            var service = CreateService();

            // Act
            var result = await service.FindAsync(locationTypeId, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorType.NotFound, result.ErrorType);
            Assert.Contains("LocationType", result.ErrorMessage);
        }

        [Fact]
        public async Task FindAsync_ReturnsException_WhenRepositoryThrows()
        {
            // Arrange
            var locationTypeId = 1;

            _uowMock.Setup(u => u.LocationTypes.FindAsync(
                It.IsAny<Expression<Func<LocationType, bool>>>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<string>()
            )).ThrowsAsync(new Exception("Database error"));

            var service = CreateService();

            // Act
            var result = await service.FindAsync(locationTypeId, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorType.InternalServerError, result.ErrorType);
            Assert.Contains("Exception in FindAsync", result.ErrorMessage);
            Assert.Contains("Database error", result.ErrorMessage);
        }

        [Fact]
        public async Task FindAsync_IncludesDeletedUserInformation_WhenLocationTypeIsDeleted()
        {
            // Arrange
            var locationTypeId = 1;
            var locationType = new LocationType
            {
                Id = locationTypeId,
                Name = "Deleted Location",
                Description = "Deleted description",
                CreatedAt = DateTime.UtcNow.AddDays(-5),
                CreatedByUserId = 1,
                IsDeleted = false, // Will be returned if not filtered
                DeletedAt = DateTime.UtcNow,
                DeletedByUserId = 2,
                CreatedByUser = new User { UserName = "creator" },
                DeletedByUser = new User { UserName = "deleter" }
            };

            _uowMock.Setup(u => u.LocationTypes.FindAsync(
                It.IsAny<Expression<Func<LocationType, bool>>>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<string>()
            )).ReturnsAsync(locationType);

            var service = CreateService();

            // Act
            var result = await service.FindAsync(locationTypeId, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal("creator", result.Value.CreatedByUserName);
            Assert.Equal("deleter", result.Value.DeletedByUserName);
            Assert.Equal(2, result.Value.DeletedByUserId);
        }

        #endregion

        #region SoftDeleteAsync Tests (Inherited from DeleteService)

        [Fact]
        public async Task SoftDeleteAsync_ReturnsInvalidId_WhenIdIsZero()
        {
            // Arrange
            var service = CreateService();

            // Act
            var result = await service.SoftDeleteAsync(0, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorType.BadRequest, result.ErrorType);
            Assert.Equal("Invalid Id", result.ErrorMessage);
        }

        [Fact]
        public async Task SoftDeleteAsync_ReturnsInvalidId_WhenIdIsNegative()
        {
            // Arrange
            var service = CreateService();

            // Act
            var result = await service.SoftDeleteAsync(-1, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorType.BadRequest, result.ErrorType);
            Assert.Equal("Invalid Id", result.ErrorMessage);
        }

        [Fact]
        public async Task SoftDeleteAsync_ReturnsNotFound_WhenLocationTypeDoesNotExist()
        {
            // Arrange
            var locationTypeId = 1;

            _uowMock.Setup(u => u.LocationTypes.FindAsync(
                It.IsAny<Expression<Func<LocationType, bool>>>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<string>()
            )).ReturnsAsync((LocationType)null!);

            var service = CreateService();

            // Act
            var result = await service.SoftDeleteAsync(locationTypeId, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorType.NotFound, result.ErrorType);
            Assert.Contains("LocationType", result.ErrorMessage);
        }

        [Fact]
        public async Task SoftDeleteAsync_ReturnsSuccess_WhenLocationTypeExists()
        {
            // Arrange
            var locationTypeId = 1;
            var locationType = new LocationType
            {
                Id = locationTypeId,
                Name = "Test Location",
                Description = "Test description",
                CreatedAt = DateTime.UtcNow,
                CreatedByUserId = 1,
                IsDeleted = false,
                DeletedAt = null,
                DeletedByUserId = null
            };

            _uowMock.Setup(u => u.LocationTypes.FindAsync(
                It.IsAny<Expression<Func<LocationType, bool>>>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<string>()
            )).ReturnsAsync(locationType);

            _currentUserServiceMock.SetupGet(c => c.UserId).Returns(2);
            _uowMock.Setup(u => u.LocationTypes.Update(It.IsAny<LocationType>()));
            _uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var service = CreateService();

            // Act
            var result = await service.SoftDeleteAsync(locationTypeId, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);

            _uowMock.Verify(u => u.LocationTypes.Update(It.Is<LocationType>(lt => 
                lt.IsDeleted == true && 
                lt.DeletedByUserId == 2 &&
                lt.DeletedAt != null
            )), Times.Once);
            _uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task SoftDeleteAsync_ReturnsFailure_WhenLocationTypeAlreadyDeleted()
        {
            // Arrange
            var locationTypeId = 1;
            var locationType = new LocationType
            {
                Id = locationTypeId,
                Name = "Test Location",
                Description = "Test description",
                CreatedAt = DateTime.UtcNow,
                CreatedByUserId = 1,
                IsDeleted = true, // Already deleted
                DeletedAt = DateTime.UtcNow,
                DeletedByUserId = 1
            };

            _uowMock.Setup(u => u.LocationTypes.FindAsync(
                It.IsAny<Expression<Func<LocationType, bool>>>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<string>()
            )).ReturnsAsync(locationType);

            var service = CreateService();

            // Act
            var result = await service.SoftDeleteAsync(locationTypeId, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorType.BadRequest, result.ErrorType);
            Assert.Contains("LocationType is already deleted", result.ErrorMessage);

            _uowMock.Verify(u => u.LocationTypes.Update(It.IsAny<LocationType>()), Times.Never);
            _uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task SoftDeleteAsync_ReturnsException_WhenRepositoryThrows()
        {
            // Arrange
            var locationTypeId = 1;

            _uowMock.Setup(u => u.LocationTypes.FindAsync(
                It.IsAny<Expression<Func<LocationType, bool>>>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<string>()
            )).ThrowsAsync(new Exception("Database error"));

            var service = CreateService();

            // Act
            var result = await service.SoftDeleteAsync(locationTypeId, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorType.InternalServerError, result.ErrorType);
            Assert.Contains("Error:", result.ErrorMessage);
            Assert.Contains("Database error", result.ErrorMessage);
        }

        [Fact]
        public async Task SoftDeleteAsync_ReturnsException_WhenSaveChangesThrows()
        {
            // Arrange
            var locationTypeId = 1;
            var locationType = new LocationType
            {
                Id = locationTypeId,
                Name = "Test Location",
                Description = "Test description",
                CreatedAt = DateTime.UtcNow,
                CreatedByUserId = 1,
                IsDeleted = false
            };

            _uowMock.Setup(u => u.LocationTypes.FindAsync(
                It.IsAny<Expression<Func<LocationType, bool>>>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<string>()
            )).ReturnsAsync(locationType);

            _currentUserServiceMock.SetupGet(c => c.UserId).Returns(2);
            _uowMock.Setup(u => u.LocationTypes.Update(It.IsAny<LocationType>()));
            _uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Save error"));

            var service = CreateService();

            // Act
            var result = await service.SoftDeleteAsync(locationTypeId, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorType.InternalServerError, result.ErrorType);
            Assert.Contains("Error:", result.ErrorMessage);
            Assert.Contains("Save error", result.ErrorMessage);
        }

        #endregion

        #region Helper Methods Tests

        [Fact]
        public void LocationTypeService_Constructor_SetsPropertiesCorrectly()
        {
            // Arrange & Act
            var service = CreateService();

            // Assert
            Assert.NotNull(service);
        }

        [Theory]
        [InlineData("Warehouse", "Storage facility")]
        [InlineData("Office", "Work location")]
        [InlineData("Retail Store", "Customer-facing location")]
        [InlineData("Distribution Center", "")]
        public async Task AddLocationTypeAsync_HandlesVariousLocationTypes(string name, string? description)
        {
            // Arrange
            var request = new LocationTypeCreateRequest
            {
                Name = name,
                Description = description
            };

            var validationResult = new ValidationResult();
            _validatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(validationResult);

            var savedLocationType = new LocationType
            {
                Id = 1,
                Name = name,
                Description = description,
                CreatedAt = DateTime.UtcNow,
                CreatedByUserId = 1,
                IsDeleted = false,
                CreatedByUser = new User { UserName = "testuser" }
            };

            _currentUserServiceMock.SetupGet(c => c.UserId).Returns(1);
            
            // Setup to simulate the ID being set after Add
            _uowMock.Setup(u => u.LocationTypes.Add(It.IsAny<LocationType>()))
                .Callback<LocationType>(lt => lt.Id = 1);
            
            _uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
            _uowMock.Setup(u => u.LocationTypes.FindAsync(
                It.IsAny<Expression<Func<LocationType, bool>>>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<string>()
            )).ReturnsAsync(savedLocationType);

            var service = CreateService();

            // Act
            var result = await service.AddLocationTypeAsync(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(name, result.Value.Name);
            Assert.Equal(description, result.Value.Description);
        }

        #endregion

        #region Edge Cases

        [Fact]
        public async Task MapToResponse_HandlesNullUserReferences()
        {
            // Arrange
            var locationType = new LocationType
            {
                Id = 1,
                Name = "Test Location",
                Description = "Test description",
                CreatedAt = DateTime.UtcNow,
                CreatedByUserId = 1,
                IsDeleted = false,
                CreatedByUser = null!, // Null reference
                DeletedByUser = null
            };

            _uowMock.Setup(u => u.LocationTypes.FindAsync(
                It.IsAny<Expression<Func<LocationType, bool>>>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<string>()
            )).ReturnsAsync(locationType);

            var service = CreateService();

            // Act
            var result = await service.FindAsync(1, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Null(result.Value.CreatedByUserName);
            Assert.Null(result.Value.DeletedByUserName);
        }

        [Fact]
        public async Task AddLocationTypeAsync_SetsCorrectTimestamp()
        {
            // Arrange
            var request = new LocationTypeCreateRequest
            {
                Name = "Time Test",
                Description = "Testing timestamp"
            };

            var validationResult = new ValidationResult();
            _validatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(validationResult);

            var beforeTime = DateTime.UtcNow;
            
            var savedLocationType = new LocationType
            {
                Id = 1,
                Name = request.Name,
                Description = request.Description,
                CreatedAt = DateTime.UtcNow,
                CreatedByUserId = 1,
                IsDeleted = false,
                CreatedByUser = new User { UserName = "testuser" }
            };

            _currentUserServiceMock.SetupGet(c => c.UserId).Returns(1);
            
            // Setup to simulate the ID being set after Add
            _uowMock.Setup(u => u.LocationTypes.Add(It.IsAny<LocationType>()))
                .Callback<LocationType>(lt => lt.Id = 1);
            
            _uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
            _uowMock.Setup(u => u.LocationTypes.FindAsync(
                It.IsAny<Expression<Func<LocationType, bool>>>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<string>()
            )).ReturnsAsync(savedLocationType);

            var service = CreateService();

            // Act
            var result = await service.AddLocationTypeAsync(request, CancellationToken.None);
            var afterTime = DateTime.UtcNow;

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.True(result.Value.CreatedAt >= beforeTime);
            Assert.True(result.Value.CreatedAt <= afterTime);
        }

        #endregion
    }
}