//using Application.Abstractions.Repositories.Base;
//using Application.Abstractions.Repositories.Inventories;
//using Application.Abstractions.Services.User;
//using Application.Abstractions.UnitOfWork;
//using Application.DTOs.Inventories;
//using Application.DTOs.Locations.Request;
//using Application.DTOs.Locations.Response;
//using Application.Results;
//using Application.Services.Locations;
//using Domain.Entities;
//using Domain.Enums;
//using FluentValidation;
//using FluentValidation.Results;
//using Moq;
//using System.Linq.Expressions;
//using Xunit;

//namespace Application.Tests.LocationTests
//{
//    public class LocationServiceTests
//    {
//        private readonly Mock<IUnitOfWork> _uowMock = new();
//        private readonly Mock<IBaseRepository<Location>> _repositoryMock = new();
//        private readonly Mock<ICurrentUserService> _currentUserServiceMock = new();
//        private readonly Mock<IValidator<LocationCreateRequest>> _createValidatorMock = new();
//        private readonly Mock<IValidator<LocationUpdateRequest>> _updateValidatorMock = new();

//        private LocationService CreateService()
//        {
//            // Setup the UnitOfWork to return our mocked repository
//            _uowMock.SetupGet(u => u.Locations).Returns(_repositoryMock.Object);
//            _uowMock.SetupGet(u => u.Inventories).Returns(new Mock<IInventoryRepository>().Object);

//            return new LocationService(
//                _uowMock.Object,
//                _currentUserServiceMock.Object,
//                _createValidatorMock.Object,
//                _updateValidatorMock.Object
//            );
//        }

//        #region GetAllAsync Tests

//        [Fact]
//        public async Task GetAllAsync_ReturnsSuccess_WhenLocationsExist()
//        {
//            // Arrange
//            var locations = new List<Location>
//            {
//                new Location
//                {
//                    Id = 1,
//                    Name = "Main Warehouse",
//                    Address = "123 Main St",
//                    IsActive = true,
//                    LocationTypeId = 1,
//                    CreatedAt = DateTime.UtcNow,
//                    CreatedByUserId = 1,
//                    IsDeleted = false,
//                    CreatedByUser = new User { UserName = "user1" }
//                },
//                new Location
//                {
//                    Id = 2,
//                    Name = "Secondary Storage",
//                    Address = "456 Second Ave",
//                    IsActive = false,
//                    LocationTypeId = 2,
//                    CreatedAt = DateTime.UtcNow,
//                    CreatedByUserId = 2,
//                    IsDeleted = false,
//                    CreatedByUser = new User { UserName = "user2" }
//                }
//            };

//            _repositoryMock.Setup(r => r.GetAllAsync(
//                null,
//                It.IsAny<CancellationToken>(),
//                "CreatedByUser,DeletedByUser"
//            )).ReturnsAsync(locations);

//            var service = CreateService();

//            // Act
//            var result = await service.GetAllAsync(CancellationToken.None);

//            // Assert
//            Assert.True(result.IsSuccess);
//            Assert.NotNull(result.Value);
//            Assert.Equal(2, result.Value.Count);

//            var mainWarehouse = result.Value.First(x => x.Name == "Main Warehouse");
//            Assert.Equal(1, mainWarehouse.Id);
//            Assert.Equal("123 Main St", mainWarehouse.Address);
//            Assert.True(mainWarehouse.IsActive);
//            Assert.Equal(1, mainWarehouse.LocationTypeId);
//            Assert.Equal("user1", mainWarehouse.CreatedByUserName);

//            var secondaryStorage = result.Value.First(x => x.Name == "Secondary Storage");
//            Assert.Equal(2, secondaryStorage.Id);
//            Assert.Equal("456 Second Ave", secondaryStorage.Address);
//            Assert.False(secondaryStorage.IsActive);
//            Assert.Equal(2, secondaryStorage.LocationTypeId);
//            Assert.Equal("user2", secondaryStorage.CreatedByUserName);
//        }

//        [Fact]
//        public async Task GetAllAsync_ReturnsNotFound_WhenNoLocationsExist()
//        {
//            // Arrange
//            var locations = new List<Location>();

//            _repositoryMock.Setup(r => r.GetAllAsync(
//                null,
//                It.IsAny<CancellationToken>(),
//                "CreatedByUser,DeletedByUser"
//            )).ReturnsAsync(locations);

//            var service = CreateService();

//            // Act
//            var result = await service.GetAllAsync(CancellationToken.None);

//            // Assert
//            Assert.False(result.IsSuccess);
//            Assert.Equal(ErrorType.NotFound, result.ErrorType);
//            Assert.Contains("locations", result.ErrorMessage);
//        }

//        [Fact]
//        public async Task GetAllAsync_ReturnsNotFound_WhenLocationsIsNull()
//        {
//            // Arrange
//            _repositoryMock.Setup(r => r.GetAllAsync(
//                null,
//                It.IsAny<CancellationToken>(),
//                "CreatedByUser,DeletedByUser"
//            )).ReturnsAsync((List<Location>)null!);

//            var service = CreateService();

//            // Act
//            var result = await service.GetAllAsync(CancellationToken.None);

//            // Assert
//            Assert.False(result.IsSuccess);
//            Assert.Equal(ErrorType.NotFound, result.ErrorType);
//            Assert.Contains("locations", result.ErrorMessage);
//        }

//        [Fact]
//        public async Task GetAllAsync_ReturnsException_WhenRepositoryThrows()
//        {
//            // Arrange
//            _repositoryMock.Setup(r => r.GetAllAsync(
//                null,
//                It.IsAny<CancellationToken>(),
//                "CreatedByUser,DeletedByUser"
//            )).ThrowsAsync(new Exception("Database error"));

//            var service = CreateService();

//            // Act
//            var result = await service.GetAllAsync(CancellationToken.None);

//            // Assert
//            Assert.False(result.IsSuccess);
//            Assert.Equal(ErrorType.InternalServerError, result.ErrorType);
//            Assert.Contains("Exception in GetAllAsync", result.ErrorMessage);
//            Assert.Contains("Database error", result.ErrorMessage);
//        }

//        [Fact]
//        public async Task GetAllAsync_HandlesNullUserReferences()
//        {
//            // Arrange
//            var locations = new List<Location>
//            {
//                new Location
//                {
//                    Id = 1,
//                    Name = "Test Location",
//                    Address = "Test Address",
//                    IsActive = true,
//                    LocationTypeId = 1,
//                    CreatedAt = DateTime.UtcNow,
//                    CreatedByUserId = 1,
//                    IsDeleted = false,
//                    CreatedByUser = null!, // Null reference
//                    DeletedByUser = null
//                }
//            };

//            _repositoryMock.Setup(r => r.GetAllAsync(
//                null,
//                It.IsAny<CancellationToken>(),
//                "CreatedByUser,DeletedByUser"
//            )).ReturnsAsync(locations);

//            var service = CreateService();

//            // Act
//            var result = await service.GetAllAsync(CancellationToken.None);

//            // Assert
//            Assert.True(result.IsSuccess);
//            Assert.NotNull(result.Value);
//            Assert.Single(result.Value);
//            Assert.Null(result.Value.First().CreatedByUserName);
//            Assert.Null(result.Value.First().DeletedByUserName);
//        }

//        #endregion

//        #region FindAsync Tests

//        [Fact]
//        public async Task FindAsync_ReturnsSuccess_WhenLocationExists()
//        {
//            // Arrange
//            var locationId = 1;
//            var location = new Location
//            {
//                Id = locationId,
//                Name = "Test Location",
//                Address = "Test Address",
//                IsActive = true,
//                LocationTypeId = 1,
//                CreatedAt = DateTime.UtcNow,
//                CreatedByUserId = 1,
//                IsDeleted = false,
//                CreatedByUser = new User { UserName = "testuser" },
//                DeletedByUser = null
//            };

//            _repositoryMock.Setup(r => r.FindAsync(
//                It.IsAny<Expression<Func<Location, bool>>>(),
//                It.IsAny<CancellationToken>(),
//                "CreatedByUser,DeletedByUser"
//            )).ReturnsAsync(location);

//            var service = CreateService();

//            // Act
//            var result = await service.FindAsync(locationId, CancellationToken.None);

//            // Assert
//            Assert.True(result.IsSuccess);
//            Assert.NotNull(result.Value);
//            Assert.Equal(locationId, result.Value.Id);
//            Assert.Equal("Test Location", result.Value.Name);
//            Assert.Equal("Test Address", result.Value.Address);
//            Assert.True(result.Value.IsActive);
//            Assert.Equal(1, result.Value.LocationTypeId);
//            Assert.Equal("testuser", result.Value.CreatedByUserName);
//            Assert.False(result.Value.IsDeleted);
//        }

//        [Fact]
//        public async Task FindAsync_ReturnsInvalidId_WhenIdIsZero()
//        {
//            // Arrange
//            var service = CreateService();

//            // Act
//            var result = await service.FindAsync(0, CancellationToken.None);

//            // Assert
//            Assert.False(result.IsSuccess);
//            Assert.Equal(ErrorType.BadRequest, result.ErrorType);
//            Assert.Equal("Invalid Id", result.ErrorMessage);

//            _repositoryMock.Verify(r => r.FindAsync(
//                It.IsAny<Expression<Func<Location, bool>>>(),
//                It.IsAny<CancellationToken>(),
//                It.IsAny<string>()
//            ), Times.Never);
//        }

//        [Fact]
//        public async Task FindAsync_ReturnsInvalidId_WhenIdIsNegative()
//        {
//            // Arrange
//            var service = CreateService();

//            // Act
//            var result = await service.FindAsync(-1, CancellationToken.None);

//            // Assert
//            Assert.False(result.IsSuccess);
//            Assert.Equal(ErrorType.BadRequest, result.ErrorType);
//            Assert.Equal("Invalid Id", result.ErrorMessage);

//            _repositoryMock.Verify(r => r.FindAsync(
//                It.IsAny<Expression<Func<Location, bool>>>(),
//                It.IsAny<CancellationToken>(),
//                It.IsAny<string>()
//            ), Times.Never);
//        }

//        [Fact]
//        public async Task FindAsync_ReturnsNotFound_WhenLocationDoesNotExist()
//        {
//            // Arrange
//            var locationId = 1;

//            _repositoryMock.Setup(r => r.FindAsync(
//                It.IsAny<Expression<Func<Location, bool>>>(),
//                It.IsAny<CancellationToken>(),
//                "CreatedByUser,DeletedByUser"
//            )).ReturnsAsync((Location)null!);

//            var service = CreateService();

//            // Act
//            var result = await service.FindAsync(locationId, CancellationToken.None);

//            // Assert
//            Assert.False(result.IsSuccess);
//            Assert.Equal(ErrorType.NotFound, result.ErrorType);
//            Assert.Contains("location", result.ErrorMessage);
//        }

//        [Fact]
//        public async Task FindAsync_ReturnsException_WhenRepositoryThrows()
//        {
//            // Arrange
//            var locationId = 1;

//            _repositoryMock.Setup(r => r.FindAsync(
//                It.IsAny<Expression<Func<Location, bool>>>(),
//                It.IsAny<CancellationToken>(),
//                "CreatedByUser,DeletedByUser"
//            )).ThrowsAsync(new Exception("Database error"));

//            var service = CreateService();

//            // Act
//            var result = await service.FindAsync(locationId, CancellationToken.None);

//            // Assert
//            Assert.False(result.IsSuccess);
//            Assert.Equal(ErrorType.InternalServerError, result.ErrorType);
//            Assert.Contains("Exception in FindAsync", result.ErrorMessage);
//            Assert.Contains("Database error", result.ErrorMessage);
//        }

//        [Fact]
//        public async Task FindAsync_OnlyReturnsNonDeletedLocations()
//        {
//            // Arrange
//            var locationId = 1;
//            var location = new Location
//            {
//                Id = locationId,
//                Name = "Test Location",
//                Address = "Test Address",
//                IsActive = true,
//                LocationTypeId = 1,
//                CreatedAt = DateTime.UtcNow,
//                CreatedByUserId = 1,
//                IsDeleted = false,
//                CreatedByUser = new User { UserName = "testuser" }
//            };

//            _repositoryMock.Setup(r => r.FindAsync(
//                It.IsAny<Expression<Func<Location, bool>>>(),
//                It.IsAny<CancellationToken>(),
//                "CreatedByUser,DeletedByUser"
//            )).ReturnsAsync(location);

//            var service = CreateService();

//            // Act
//            var result = await service.FindAsync(locationId, CancellationToken.None);

//            // Assert
//            Assert.True(result.IsSuccess);
            
//            // Verify the filter expression includes !e.IsDeleted
//            _repositoryMock.Verify(r => r.FindAsync(
//                It.Is<Expression<Func<Location, bool>>>(expr => expr != null),
//                It.IsAny<CancellationToken>(),
//                "CreatedByUser,DeletedByUser"
//            ), Times.Once);
//        }

//        #endregion

//        #region CreateAsync Tests

//        [Fact]
//        public async Task CreateAsync_ReturnsSuccess_WhenValidRequest()
//        {
//            // Arrange
//            var request = new LocationCreateRequest
//            {
//                Name = "New Warehouse",
//                Address = "123 New St",
//                LocationTypeId = 1
//            };

//            var validationResult = new ValidationResult();
//            _createValidatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
//                .ReturnsAsync(validationResult);

//            _currentUserServiceMock.SetupGet(c => c.UserId).Returns(1);

//            // Setup to simulate the ID being set after Add
//            _uowMock.Setup(u => u.Locations.Add(It.IsAny<Location>()))
//                .Callback<Location>(l => l.Id = 1);

//            _uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

//            var service = CreateService();

//            // Act
//            var result = await service.CreateAsync(request, CancellationToken.None);

//            // Assert
//            Assert.True(result.IsSuccess);
//            Assert.NotNull(result.Value);
//            Assert.Equal(request.Name, result.Value.Name);
//            Assert.Equal(request.Address, result.Value.Address);
//            Assert.Equal(request.LocationTypeId, result.Value.LocationTypeId);
//            Assert.True(result.Value.IsActive);
//            Assert.Equal(1, result.Value.CreatedByUserId);
//            Assert.False(result.Value.IsDeleted);

//            _uowMock.Verify(u => u.Locations.Add(It.Is<Location>(l => 
//                l.Name == request.Name &&
//                l.Address == request.Address &&
//                l.LocationTypeId == request.LocationTypeId &&
//                l.IsActive == true &&
//                l.CreatedByUserId == 1 &&
//                l.IsDeleted == false
//            )), Times.Once);
//            _uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
//        }

//        [Fact]
//        public async Task CreateAsync_ReturnsFailure_WhenValidationFails()
//        {
//            // Arrange
//            var request = new LocationCreateRequest
//            {
//                Name = "", // Invalid empty name
//                Address = "Test Address",
//                LocationTypeId = 1
//            };

//            var validationResult = new ValidationResult(new[]
//            {
//                new ValidationFailure("Name", "Name is required."),
//            });

//            _createValidatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
//                .ReturnsAsync(validationResult);

//            var service = CreateService();

//            // Act
//            var result = await service.CreateAsync(request, CancellationToken.None);

//            // Assert
//            Assert.False(result.IsSuccess);
//            Assert.Equal(ErrorType.BadRequest, result.ErrorType);
//            Assert.Contains("Name is required.", result.ErrorMessage);

//            _uowMock.Verify(u => u.Locations.Add(It.IsAny<Location>()), Times.Never);
//            _uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
//        }

//        [Fact]
//        public async Task CreateAsync_ReturnsFailure_WhenMultipleValidationErrorsExist()
//        {
//            // Arrange
//            var request = new LocationCreateRequest
//            {
//                Name = "",
//                Address = "",
//                LocationTypeId = 0
//            };

//            var validationResult = new ValidationResult(new[]
//            {
//                new ValidationFailure("Name", "Name is required."),
//                new ValidationFailure("Address", "Address is required."),
//                new ValidationFailure("LocationTypeId", "LocationTypeId must be greater than 0.")
//            });

//            _createValidatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
//                .ReturnsAsync(validationResult);

//            var service = CreateService();

//            // Act
//            var result = await service.CreateAsync(request, CancellationToken.None);

//            // Assert
//            Assert.False(result.IsSuccess);
//            Assert.Equal(ErrorType.BadRequest, result.ErrorType);
//            Assert.Contains("Name is required.", result.ErrorMessage);
//            Assert.Contains("Address is required.", result.ErrorMessage);
//            Assert.Contains("LocationTypeId must be greater than 0.", result.ErrorMessage);
//            Assert.Contains(";", result.ErrorMessage); // Multiple errors separated by semicolon
//        }

//        [Fact]
//        public async Task CreateAsync_ReturnsException_WhenValidatorThrows()
//        {
//            // Arrange
//            var request = new LocationCreateRequest
//            {
//                Name = "Test",
//                Address = "Test Address",
//                LocationTypeId = 1
//            };

//            _createValidatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
//                .ThrowsAsync(new Exception("Validation error"));

//            var service = CreateService();

//            // Act
//            var result = await service.CreateAsync(request, CancellationToken.None);

//            // Assert
//            Assert.False(result.IsSuccess);
//            Assert.Equal(ErrorType.InternalServerError, result.ErrorType);
//            Assert.Contains("Exception in CreateAsync", result.ErrorMessage);
//            Assert.Contains("Validation error", result.ErrorMessage);
//        }

//        [Fact]
//        public async Task CreateAsync_ReturnsException_WhenSaveChangesThrows()
//        {
//            // Arrange
//            var request = new LocationCreateRequest
//            {
//                Name = "Test",
//                Address = "Test Address",
//                LocationTypeId = 1
//            };

//            var validationResult = new ValidationResult();
//            _createValidatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
//                .ReturnsAsync(validationResult);

//            _currentUserServiceMock.SetupGet(c => c.UserId).Returns(1);
//            _uowMock.Setup(u => u.Locations.Add(It.IsAny<Location>()));
//            _uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
//                .ThrowsAsync(new Exception("Database error"));

//            var service = CreateService();

//            // Act
//            var result = await service.CreateAsync(request, CancellationToken.None);

//            // Assert
//            Assert.False(result.IsSuccess);
//            Assert.Equal(ErrorType.InternalServerError, result.ErrorType);
//            Assert.Contains("Exception in CreateAsync", result.ErrorMessage);
//            Assert.Contains("Database error", result.ErrorMessage);
//        }

//        [Fact]
//        public async Task CreateAsync_SetsCorrectTimestamp()
//        {
//            // Arrange
//            var request = new LocationCreateRequest
//            {
//                Name = "Time Test",
//                Address = "Test Address",
//                LocationTypeId = 1
//            };

//            var validationResult = new ValidationResult();
//            _createValidatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
//                .ReturnsAsync(validationResult);

//            var beforeTime = DateTime.UtcNow;
//            _currentUserServiceMock.SetupGet(c => c.UserId).Returns(1);
            
//            Location capturedLocation = null!;
//            _uowMock.Setup(u => u.Locations.Add(It.IsAny<Location>()))
//                .Callback<Location>(l => 
//                {
//                    l.Id = 1;
//                    capturedLocation = l;
//                });
            
//            _uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

//            var service = CreateService();

//            // Act
//            var result = await service.CreateAsync(request, CancellationToken.None);
//            var afterTime = DateTime.UtcNow;

//            // Assert
//            Assert.True(result.IsSuccess);
//            Assert.NotNull(result.Value);
//            Assert.True(result.Value.CreatedAt >= beforeTime);
//            Assert.True(result.Value.CreatedAt <= afterTime);
//            Assert.NotNull(capturedLocation);
//            Assert.True(capturedLocation.CreatedAt >= beforeTime);
//            Assert.True(capturedLocation.CreatedAt <= afterTime);
//        }

//        #endregion

//        #region UpdateLocationStatus Tests

//        [Theory]
//        [InlineData(true, "activate")]
//        [InlineData(false, "deactivate")]
//        public async Task UpdateLocationStatus_ReturnsSuccess_WhenLocationExistsAndStatusDifferent(bool newStatus, string action)
//        {
//            // Arrange
//            var locationId = 1;
//            var location = new Location
//            {
//                Id = locationId,
//                Name = "Test Location",
//                Address = "Test Address",
//                IsActive = !newStatus, // Opposite of new status
//                LocationTypeId = 1,
//                CreatedAt = DateTime.UtcNow,
//                CreatedByUserId = 1,
//                IsDeleted = false
//            };

//            _repositoryMock.Setup(r => r.FindAsync(
//                It.IsAny<Expression<Func<Location, bool>>>(),
//                It.IsAny<CancellationToken>(),
//                It.IsAny<string>()
//            )).ReturnsAsync(location);

//            _repositoryMock.Setup(r => r.Update(It.IsAny<Location>()));
//            _uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

//            var service = CreateService();

//            // Act
//            Result result;
//            if (newStatus)
//                result = await service.ActivateAsync(locationId, CancellationToken.None);
//            else
//                result = await service.DeactivateAsync(locationId, CancellationToken.None);

//            // Assert
//            Assert.True(result.IsSuccess);
//            Assert.Equal(newStatus, location.IsActive);

//            _repositoryMock.Verify(r => r.Update(It.Is<Location>(l => l.IsActive == newStatus)), Times.Once);
//            _uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
//        }

//        [Theory]
//        [InlineData(0)]
//        [InlineData(-1)]
//        public async Task UpdateLocationStatus_ReturnsInvalidId_WhenIdIsInvalid(int invalidId)
//        {
//            // Arrange
//            var service = CreateService();

//            // Act
//            var activateResult = await service.ActivateAsync(invalidId, CancellationToken.None);
//            var deactivateResult = await service.DeactivateAsync(invalidId, CancellationToken.None);

//            // Assert
//            Assert.False(activateResult.IsSuccess);
//            Assert.Equal(ErrorType.BadRequest, activateResult.ErrorType);
//            Assert.Equal("Invalid Id", activateResult.ErrorMessage);

//            Assert.False(deactivateResult.IsSuccess);
//            Assert.Equal(ErrorType.BadRequest, deactivateResult.ErrorType);
//            Assert.Equal("Invalid Id", deactivateResult.ErrorMessage);
//        }

//        [Fact]
//        public async Task UpdateLocationStatus_ReturnsNotFound_WhenLocationDoesNotExist()
//        {
//            // Arrange
//            var locationId = 1;

//            _repositoryMock.Setup(r => r.FindAsync(
//                It.IsAny<Expression<Func<Location, bool>>>(),
//                It.IsAny<CancellationToken>(),
//                It.IsAny<string>()
//            )).ReturnsAsync((Location)null!);

//            var service = CreateService();

//            // Act
//            var result = await service.ActivateAsync(locationId, CancellationToken.None);

//            // Assert
//            Assert.False(result.IsSuccess);
//            Assert.Equal(ErrorType.NotFound, result.ErrorType);
//            Assert.Contains("location", result.ErrorMessage);
//        }

//        [Theory]
//        [InlineData(true, "active")]
//        [InlineData(false, "inactive")]
//        public async Task UpdateLocationStatus_ReturnsConflict_WhenLocationAlreadyInDesiredState(bool currentStatus, string statusText)
//        {
//            // Arrange
//            var locationId = 1;
//            var location = new Location
//            {
//                Id = locationId,
//                Name = "Test Location",
//                Address = "Test Address",
//                IsActive = currentStatus,
//                LocationTypeId = 1,
//                CreatedAt = DateTime.UtcNow,
//                CreatedByUserId = 1,
//                IsDeleted = false
//            };

//            _repositoryMock.Setup(r => r.FindAsync(
//                It.IsAny<Expression<Func<Location, bool>>>(),
//                It.IsAny<CancellationToken>(),
//                It.IsAny<string>()
//            )).ReturnsAsync(location);

//            var service = CreateService();

//            // Act
//            Result result;
//            if (currentStatus)
//                result = await service.ActivateAsync(locationId, CancellationToken.None);
//            else
//                result = await service.DeactivateAsync(locationId, CancellationToken.None);

//            // Assert
//            Assert.False(result.IsSuccess);
//            Assert.Equal(ErrorType.Conflict, result.ErrorType);
//            Assert.Contains($"Location is already {statusText}", result.ErrorMessage);

//            _repositoryMock.Verify(r => r.Update(It.IsAny<Location>()), Times.Never);
//            _uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
//        }

//        [Fact]
//        public async Task UpdateLocationStatus_ReturnsException_WhenRepositoryThrows()
//        {
//            // Arrange
//            var locationId = 1;

//            _repositoryMock.Setup(r => r.FindAsync(
//                It.IsAny<Expression<Func<Location, bool>>>(),
//                It.IsAny<CancellationToken>(),
//                It.IsAny<string>()
//            )).ThrowsAsync(new Exception("Database error"));

//            var service = CreateService();

//            // Act
//            var result = await service.ActivateAsync(locationId, CancellationToken.None);

//            // Assert
//            Assert.False(result.IsSuccess);
//            Assert.Equal(ErrorType.InternalServerError, result.ErrorType);
//            Assert.Contains("Exception in _UpdateLocationStatus", result.ErrorMessage);
//            Assert.Contains("Database error", result.ErrorMessage);
//        }

//        [Fact]
//        public async Task UpdateLocationStatus_ReturnsException_WhenSaveChangesThrows()
//        {
//            // Arrange
//            var locationId = 1;
//            var location = new Location
//            {
//                Id = locationId,
//                Name = "Test Location",
//                Address = "Test Address",
//                IsActive = false,
//                LocationTypeId = 1,
//                CreatedAt = DateTime.UtcNow,
//                CreatedByUserId = 1,
//                IsDeleted = false
//            };

//            _repositoryMock.Setup(r => r.FindAsync(
//                It.IsAny<Expression<Func<Location, bool>>>(),
//                It.IsAny<CancellationToken>(),
//                It.IsAny<string>()
//            )).ReturnsAsync(location);

//            _repositoryMock.Setup(r => r.Update(It.IsAny<Location>()));
//            _uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
//                .ThrowsAsync(new Exception("Save error"));

//            var service = CreateService();

//            // Act
//            var result = await service.ActivateAsync(locationId, CancellationToken.None);

//            // Assert
//            Assert.False(result.IsSuccess);
//            Assert.Equal(ErrorType.InternalServerError, result.ErrorType);
//            Assert.Contains("Exception in _UpdateLocationStatus", result.ErrorMessage);
//            Assert.Contains("Save error", result.ErrorMessage);
//        }

//        #endregion

//        #region UpdateAsync Tests

//        [Fact]
//        public async Task UpdateAsync_ReturnsSuccess_WhenValidRequest()
//        {
//            // Arrange
//            var locationId = 1;
//            var request = new LocationUpdateRequest
//            {
//                Id = locationId,
//                Name = "Updated Warehouse",
//                Address = "456 Updated St",
//                LocationTypeId = 2,
//                IsActive = true
//            };

//            var location = new Location
//            {
//                Id = locationId,
//                Name = "Old Warehouse",
//                Address = "123 Old St",
//                LocationTypeId = 1,
//                IsActive = false,
//                CreatedAt = DateTime.UtcNow,
//                CreatedByUserId = 1,
//                IsDeleted = false
//            };

//            var validationResult = new ValidationResult();
//            _updateValidatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
//                .ReturnsAsync(validationResult);

//            _repositoryMock.Setup(r => r.FindAsync(
//                It.IsAny<Expression<Func<Location, bool>>>(),
//                It.IsAny<CancellationToken>(),
//                It.IsAny<string>()
//            )).ReturnsAsync(location);

//            _repositoryMock.Setup(r => r.Update(It.IsAny<Location>()));
//            _uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

//            var service = CreateService();

//            // Act
//            var result = await service.UpdateAsync(locationId, request, CancellationToken.None);

//            // Assert
//            Assert.True(result.IsSuccess);
//            Assert.NotNull(result.Value);
//            Assert.Equal(request.Name, result.Value.Name);
//            Assert.Equal(request.Address, result.Value.Address);
//            Assert.Equal(request.LocationTypeId, result.Value.LocationTypeId);

//            // Verify the location was updated with new values
//            Assert.Equal(request.Name, location.Name);
//            Assert.Equal(request.Address, location.Address);
//            Assert.Equal(request.LocationTypeId, location.LocationTypeId);

//            _repositoryMock.Verify(r => r.Update(It.IsAny<Location>()), Times.Once);
//            _uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
//        }

//        [Fact]
//        public async Task UpdateAsync_ReturnsInvalidId_WhenIdIsZero()
//        {
//            // Arrange
//            var request = new LocationUpdateRequest
//            {
//                Id = 0,
//                Name = "Test",
//                Address = "Test Address",
//                LocationTypeId = 1,
//                IsActive = true
//            };

//            var service = CreateService();

//            // Act
//            var result = await service.UpdateAsync(0, request, CancellationToken.None);

//            // Assert
//            Assert.False(result.IsSuccess);
//            Assert.Equal(ErrorType.BadRequest, result.ErrorType);
//            Assert.Equal("Invalid Id", result.ErrorMessage);

//            _updateValidatorMock.Verify(v => v.ValidateAsync(It.IsAny<LocationUpdateRequest>(), It.IsAny<CancellationToken>()), Times.Never);
//        }

//        [Fact]
//        public async Task UpdateAsync_ReturnsInvalidId_WhenIdIsNegative()
//        {
//            // Arrange
//            var request = new LocationUpdateRequest
//            {
//                Id = -1,
//                Name = "Test",
//                Address = "Test Address",
//                LocationTypeId = 1,
//                IsActive = true
//            };

//            var service = CreateService();

//            // Act
//            var result = await service.UpdateAsync(-1, request, CancellationToken.None);

//            // Assert
//            Assert.False(result.IsSuccess);
//            Assert.Equal(ErrorType.BadRequest, result.ErrorType);
//            Assert.Equal("Invalid Id", result.ErrorMessage);

//            _updateValidatorMock.Verify(v => v.ValidateAsync(It.IsAny<LocationUpdateRequest>(), It.IsAny<CancellationToken>()), Times.Never);
//        }

//        [Fact]
//        public async Task UpdateAsync_ReturnsFailure_WhenValidationFails()
//        {
//            // Arrange
//            var locationId = 1;
//            var request = new LocationUpdateRequest
//            {
//                Id = locationId,
//                Name = "", // Invalid empty name
//                Address = "Test Address",
//                LocationTypeId = 1,
//                IsActive = true
//            };

//            var validationResult = new ValidationResult(new[]
//            {
//                new ValidationFailure("Name", "Name is required."),
//            });

//            _updateValidatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
//                .ReturnsAsync(validationResult);

//            var service = CreateService();

//            // Act
//            var result = await service.UpdateAsync(locationId, request, CancellationToken.None);

//            // Assert
//            Assert.False(result.IsSuccess);
//            Assert.Equal(ErrorType.BadRequest, result.ErrorType);
//            Assert.Contains("Name is required.", result.ErrorMessage);

//            _repositoryMock.Verify(r => r.FindAsync(It.IsAny<Expression<Func<Location, bool>>>(), It.IsAny<CancellationToken>(), It.IsAny<string>()), Times.Never);
//        }

//        [Fact]
//        public async Task UpdateAsync_ReturnsNotFound_WhenLocationDoesNotExist()
//        {
//            // Arrange
//            var locationId = 1;
//            var request = new LocationUpdateRequest
//            {
//                Id = locationId,
//                Name = "Test",
//                Address = "Test Address",
//                LocationTypeId = 1,
//                IsActive = true
//            };

//            var validationResult = new ValidationResult();
//            _updateValidatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
//                .ReturnsAsync(validationResult);

//            _repositoryMock.Setup(r => r.FindAsync(
//                It.IsAny<Expression<Func<Location, bool>>>(),
//                It.IsAny<CancellationToken>(),
//                It.IsAny<string>()
//            )).ReturnsAsync((Location)null!);

//            var service = CreateService();

//            // Act
//            var result = await service.UpdateAsync(locationId, request, CancellationToken.None);

//            // Assert
//            Assert.False(result.IsSuccess);
//            Assert.Equal(ErrorType.NotFound, result.ErrorType);
//            Assert.Contains("location", result.ErrorMessage);
//        }

//        [Fact]
//        public async Task UpdateAsync_ReturnsException_WhenRepositoryThrows()
//        {
//            // Arrange
//            var locationId = 1;
//            var request = new LocationUpdateRequest
//            {
//                Id = locationId,
//                Name = "Test",
//                Address = "Test Address",
//                LocationTypeId = 1,
//                IsActive = true
//            };

//            var validationResult = new ValidationResult();
//            _updateValidatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
//                .ReturnsAsync(validationResult);

//            _repositoryMock.Setup(r => r.FindAsync(
//                It.IsAny<Expression<Func<Location, bool>>>(),
//                It.IsAny<CancellationToken>(),
//                It.IsAny<string>()
//            )).ThrowsAsync(new Exception("Database error"));

//            var service = CreateService();

//            // Act
//            var result = await service.UpdateAsync(locationId, request, CancellationToken.None);

//            // Assert
//            Assert.False(result.IsSuccess);
//            Assert.Equal(ErrorType.InternalServerError, result.ErrorType);
//            Assert.Contains("Exception in UpdateAsync", result.ErrorMessage);
//            Assert.Contains("Database error", result.ErrorMessage);
//        }

//        #endregion

//        #region GetLocationInventoriesAsync Tests

//        [Fact]
//        public async Task GetLocationInventoriesAsync_ReturnsSuccess_WhenInventoriesExist()
//        {
//            // Arrange
//            var locationId = 1;
//            var inventories = new List<Inventory>
//            {
//                new Inventory
//                {
//                    Id = 1,
//                    ProductId = 1,
//                    LocationId = locationId,
//                    QuantityOnHand = 100,
//                    ReorderLevel = 10,
//                    MaxLevel = 500,
//                    Product = new Product { Name = "Product 1" },
//                    Location = new Location { Name = "Test Location" }
//                },
//                new Inventory
//                {
//                    Id = 2,
//                    ProductId = 2,
//                    LocationId = locationId,
//                    QuantityOnHand = 50,
//                    ReorderLevel = 5,
//                    MaxLevel = 200,
//                    Product = new Product { Name = "Product 2" },
//                    Location = new Location { Name = "Test Location" }
//                }
//            };

//            var inventoryRepositoryMock = new Mock<IInventoryRepository>();
//            inventoryRepositoryMock.Setup(r => r.GetAllAsync(
//                It.IsAny<Expression<Func<Inventory, bool>>>(),
//                It.IsAny<CancellationToken>(),
//                "Product,Location"
//            )).ReturnsAsync(inventories);

//            _uowMock.SetupGet(u => u.Inventories).Returns(inventoryRepositoryMock.Object);

//            var service = CreateService();

//            // Act
//            var result = await service.GetLocationInventoriesAsync(locationId, CancellationToken.None);

//            // Assert
//            Assert.True(result.IsSuccess);
//            Assert.NotNull(result.Value);
//            Assert.Equal(2, result.Value.Count);

//            var first = result.Value.First();
//            Assert.Equal(1, first.Id);
//            Assert.Equal(1, first.ProductId);
//            Assert.Equal("Product 1", first.ProductName);
//            Assert.Equal(locationId, first.LocationId);
//            Assert.Equal("Test Location", first.LocationName);
//            Assert.Equal(100, first.QuantityOnHand);
//            Assert.Equal(10, first.ReorderLevel);
//            Assert.Equal(500, first.MaxLevel);
//        }

//        [Fact]
//        public async Task GetLocationInventoriesAsync_ReturnsInvalidId_WhenIdIsZero()
//        {
//            // Arrange
//            var service = CreateService();

//            // Act
//            var result = await service.GetLocationInventoriesAsync(0, CancellationToken.None);

//            // Assert
//            Assert.False(result.IsSuccess);
//            Assert.Equal(ErrorType.BadRequest, result.ErrorType);
//            Assert.Equal("Invalid Id", result.ErrorMessage);
//        }

//        [Fact]
//        public async Task GetLocationInventoriesAsync_ReturnsInvalidId_WhenIdIsNegative()
//        {
//            // Arrange
//            var service = CreateService();

//            // Act
//            var result = await service.GetLocationInventoriesAsync(-1, CancellationToken.None);

//            // Assert
//            Assert.False(result.IsSuccess);
//            Assert.Equal(ErrorType.BadRequest, result.ErrorType);
//            Assert.Equal("Invalid Id", result.ErrorMessage);
//        }

//        [Fact]
//        public async Task GetLocationInventoriesAsync_ReturnsNotFound_WhenNoInventoriesExist()
//        {
//            // Arrange
//            var locationId = 1;
//            var inventories = new List<Inventory>();

//            var inventoryRepositoryMock = new Mock<IInventoryRepository>();
//            inventoryRepositoryMock.Setup(r => r.GetAllAsync(
//                It.IsAny<Expression<Func<Inventory, bool>>>(),
//                It.IsAny<CancellationToken>(),
//                "Product,Location"
//            )).ReturnsAsync(inventories);

//            _uowMock.SetupGet(u => u.Inventories).Returns(inventoryRepositoryMock.Object);

//            var service = CreateService();

//            // Act
//            var result = await service.GetLocationInventoriesAsync(locationId, CancellationToken.None);

//            // Assert
//            Assert.False(result.IsSuccess);
//            Assert.Equal(ErrorType.NotFound, result.ErrorType);
//            Assert.Contains("inventories", result.ErrorMessage);
//        }

//        [Fact]
//        public async Task GetLocationInventoriesAsync_ReturnsNotFound_WhenInventoriesIsNull()
//        {
//            // Arrange
//            var locationId = 1;

//            var inventoryRepositoryMock = new Mock<IInventoryRepository>();
//            inventoryRepositoryMock.Setup(r => r.GetAllAsync(
//                It.IsAny<Expression<Func<Inventory, bool>>>(),
//                It.IsAny<CancellationToken>(),
//                "Product,Location"
//            )).ReturnsAsync((List<Inventory>)null!);

//            _uowMock.SetupGet(u => u.Inventories).Returns(inventoryRepositoryMock.Object);

//            var service = CreateService();

//            // Act
//            var result = await service.GetLocationInventoriesAsync(locationId, CancellationToken.None);

//            // Assert
//            Assert.False(result.IsSuccess);
//            Assert.Equal(ErrorType.NotFound, result.ErrorType);
//            Assert.Contains("inventories", result.ErrorMessage);
//        }

//        [Fact]
//        public async Task GetLocationInventoriesAsync_ReturnsException_WhenRepositoryThrows()
//        {
//            // Arrange
//            var locationId = 1;

//            var inventoryRepositoryMock = new Mock<IInventoryRepository>();
//            inventoryRepositoryMock.Setup(r => r.GetAllAsync(
//                It.IsAny<Expression<Func<Inventory, bool>>>(),
//                It.IsAny<CancellationToken>(),
//                "Product,Location"
//            )).ThrowsAsync(new Exception("Database error"));

//            _uowMock.SetupGet(u => u.Inventories).Returns(inventoryRepositoryMock.Object);

//            var service = CreateService();

//            // Act
//            var result = await service.GetLocationInventoriesAsync(locationId, CancellationToken.None);

//            // Assert
//            Assert.False(result.IsSuccess);
//            Assert.Equal(ErrorType.InternalServerError, result.ErrorType);
//            Assert.Contains("Exception in GetLocationInventoriesAsync", result.ErrorMessage);
//            Assert.Contains("Database error", result.ErrorMessage);
//        }

//        [Fact]
//        public async Task GetLocationInventoriesAsync_HandlesNullProductAndLocationNames()
//        {
//            // Arrange
//            var locationId = 1;
//            var inventories = new List<Inventory>
//            {
//                new Inventory
//                {
//                    Id = 1,
//                    ProductId = 1,
//                    LocationId = locationId,
//                    QuantityOnHand = 100,
//                    ReorderLevel = 10,
//                    MaxLevel = 500,
//                    Product = null!, // Null product
//                    Location = null! // Null location
//                }
//            };

//            var inventoryRepositoryMock = new Mock<IInventoryRepository>();
//            inventoryRepositoryMock.Setup(r => r.GetAllAsync(
//                It.IsAny<Expression<Func<Inventory, bool>>>(),
//                It.IsAny<CancellationToken>(),
//                "Product,Location"
//            )).ReturnsAsync(inventories);

//            _uowMock.SetupGet(u => u.Inventories).Returns(inventoryRepositoryMock.Object);

//            var service = CreateService();

//            // Act
//            var result = await service.GetLocationInventoriesAsync(locationId, CancellationToken.None);

//            // Assert
//            Assert.True(result.IsSuccess);
//            Assert.NotNull(result.Value);
//            Assert.Single(result.Value);

//            var inventory = result.Value.First();
//            Assert.Equal(string.Empty, inventory.ProductName);
//            Assert.Equal(string.Empty, inventory.LocationName);
//        }

//        #endregion

//        #region SoftDeleteAsync Tests (Inherited from DeleteService)

//        [Fact]
//        public async Task SoftDeleteAsync_ReturnsInvalidId_WhenIdIsZero()
//        {
//            // Arrange
//            var service = CreateService();

//            // Act
//            var result = await service.SoftDeleteAsync(0, CancellationToken.None);

//            // Assert
//            Assert.False(result.IsSuccess);
//            Assert.Equal(ErrorType.BadRequest, result.ErrorType);
//            Assert.Equal("Invalid Id", result.ErrorMessage);
//        }

//        [Fact]
//        public async Task SoftDeleteAsync_ReturnsInvalidId_WhenIdIsNegative()
//        {
//            // Arrange
//            var service = CreateService();

//            // Act
//            var result = await service.SoftDeleteAsync(-1, CancellationToken.None);

//            // Assert
//            Assert.False(result.IsSuccess);
//            Assert.Equal(ErrorType.BadRequest, result.ErrorType);
//            Assert.Equal("Invalid Id", result.ErrorMessage);
//        }

//        [Fact]
//        public async Task SoftDeleteAsync_ReturnsNotFound_WhenLocationDoesNotExist()
//        {
//            // Arrange
//            var locationId = 1;

//            _repositoryMock.Setup(r => r.FindAsync(
//                It.IsAny<Expression<Func<Location, bool>>>(),
//                It.IsAny<CancellationToken>(),
//                It.IsAny<string>()
//            )).ReturnsAsync((Location)null!);

//            var service = CreateService();

//            // Act
//            var result = await service.SoftDeleteAsync(locationId, CancellationToken.None);

//            // Assert
//            Assert.False(result.IsSuccess);
//            Assert.Equal(ErrorType.NotFound, result.ErrorType);
//            Assert.Contains("Location", result.ErrorMessage);
//        }

//        [Fact]
//        public async Task SoftDeleteAsync_ReturnsSuccess_WhenLocationExists()
//        {
//            // Arrange
//            var locationId = 1;
//            var location = new Location
//            {
//                Id = locationId,
//                Name = "Test Location",
//                Address = "Test Address",
//                IsActive = true,
//                LocationTypeId = 1,
//                CreatedAt = DateTime.UtcNow,
//                CreatedByUserId = 1,
//                IsDeleted = false,
//                DeletedAt = null,
//                DeletedByUserId = null
//            };

//            _repositoryMock.Setup(r => r.FindAsync(
//                It.IsAny<Expression<Func<Location, bool>>>(),
//                It.IsAny<CancellationToken>(),
//                It.IsAny<string>()
//            )).ReturnsAsync(location);

//            _currentUserServiceMock.SetupGet(c => c.UserId).Returns(2);
//            _repositoryMock.Setup(r => r.Update(It.IsAny<Location>()));
//            _uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

//            var service = CreateService();

//            // Act
//            var result = await service.SoftDeleteAsync(locationId, CancellationToken.None);

//            // Assert
//            Assert.True(result.IsSuccess);

//            _repositoryMock.Verify(r => r.Update(It.Is<Location>(l => 
//                l.IsDeleted == true && 
//                l.DeletedByUserId == 2 &&
//                l.DeletedAt != null
//            )), Times.Once);
//            _uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
//        }

//        [Fact]
//        public async Task SoftDeleteAsync_ReturnsFailure_WhenLocationAlreadyDeleted()
//        {
//            // Arrange
//            var locationId = 1;
//            var location = new Location
//            {
//                Id = locationId,
//                Name = "Test Location",
//                Address = "Test Address",
//                IsActive = true,
//                LocationTypeId = 1,
//                CreatedAt = DateTime.UtcNow,
//                CreatedByUserId = 1,
//                IsDeleted = true, // Already deleted
//                DeletedAt = DateTime.UtcNow,
//                DeletedByUserId = 1
//            };

//            _repositoryMock.Setup(r => r.FindAsync(
//                It.IsAny<Expression<Func<Location, bool>>>(),
//                It.IsAny<CancellationToken>(),
//                It.IsAny<string>()
//            )).ReturnsAsync(location);

//            var service = CreateService();

//            // Act
//            var result = await service.SoftDeleteAsync(locationId, CancellationToken.None);

//            // Assert
//            Assert.False(result.IsSuccess);
//            Assert.Equal(ErrorType.BadRequest, result.ErrorType);
//            Assert.Contains("Location is already deleted", result.ErrorMessage);

//            _repositoryMock.Verify(r => r.Update(It.IsAny<Location>()), Times.Never);
//            _uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
//        }

//        #endregion

//        #region Helper Methods and Edge Cases

//        [Fact]
//        public void LocationService_Constructor_SetsPropertiesCorrectly()
//        {
//            // Arrange & Act
//            var service = CreateService();

//            // Assert
//            Assert.NotNull(service);
//        }

//        [Theory]
//        [InlineData("Warehouse", "123 Industrial Blvd")]
//        [InlineData("Office", "456 Business Park")]
//        [InlineData("Retail Store", "789 Main Street")]
//        [InlineData("Distribution Center", "321 Commerce Way")]
//        public async Task CreateAsync_HandlesVariousLocationTypes(string name, string address)
//        {
//            // Arrange
//            var request = new LocationCreateRequest
//            {
//                Name = name,
//                Address = address,
//                LocationTypeId = 1
//            };

//            var validationResult = new ValidationResult();
//            _createValidatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
//                .ReturnsAsync(validationResult);

//            _currentUserServiceMock.SetupGet(c => c.UserId).Returns(1);
            
//            _uowMock.Setup(u => u.Locations.Add(It.IsAny<Location>()))
//                .Callback<Location>(l => l.Id = 1);
            
//            _uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

//            var service = CreateService();

//            // Act
//            var result = await service.CreateAsync(request, CancellationToken.None);

//            // Assert
//            Assert.True(result.IsSuccess);
//            Assert.NotNull(result.Value);
//            Assert.Equal(name, result.Value.Name);
//            Assert.Equal(address, result.Value.Address);
//        }

//        [Fact]
//        public async Task Map_HandlesAllNullUserReferences()
//        {
//            // Arrange
//            var location = new Location
//            {
//                Id = 1,
//                Name = "Test Location",
//                Address = "Test Address",
//                IsActive = true,
//                LocationTypeId = 1,
//                CreatedAt = DateTime.UtcNow,
//                CreatedByUserId = 1,
//                IsDeleted = false,
//                DeletedAt = DateTime.UtcNow,
//                DeletedByUserId = 2,
//                CreatedByUser = null!, // Null reference
//                DeletedByUser = null // Null reference
//            };

//            _repositoryMock.Setup(r => r.FindAsync(
//                It.IsAny<Expression<Func<Location, bool>>>(),
//                It.IsAny<CancellationToken>(),
//                "CreatedByUser,DeletedByUser"
//            )).ReturnsAsync(location);

//            var service = CreateService();

//            // Act
//            var result = await service.FindAsync(1, CancellationToken.None);

//            // Assert
//            Assert.True(result.IsSuccess);
//            Assert.NotNull(result.Value);
//            Assert.Null(result.Value.CreatedByUserName);
//            Assert.Null(result.Value.DeletedByUserName);
//            Assert.Equal(1, result.Value.CreatedByUserId);
//            Assert.Equal(2, result.Value.DeletedByUserId);
//        }

//        [Fact]
//        public async Task CreateAsync_SetsDefaultValuesCorrectly()
//        {
//            // Arrange
//            var request = new LocationCreateRequest
//            {
//                Name = "Test Location",
//                Address = "Test Address",
//                LocationTypeId = 1
//            };

//            var validationResult = new ValidationResult();
//            _createValidatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
//                .ReturnsAsync(validationResult);

//            _currentUserServiceMock.SetupGet(c => c.UserId).Returns(5);

//            Location capturedLocation = null!;
//            _uowMock.Setup(u => u.Locations.Add(It.IsAny<Location>()))
//                .Callback<Location>(l => 
//                {
//                    l.Id = 1;
//                    capturedLocation = l;
//                });

//            _uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

//            var service = CreateService();

//            // Act
//            var result = await service.CreateAsync(request, CancellationToken.None);

//            // Assert
//            Assert.True(result.IsSuccess);
//            Assert.NotNull(capturedLocation);
            
//            // Verify default values are set correctly
//            Assert.True(capturedLocation.IsActive); // Should default to true
//            Assert.False(capturedLocation.IsDeleted); // Should default to false
//            Assert.Equal(5, capturedLocation.CreatedByUserId); // Should use current user
//            Assert.True(capturedLocation.CreatedAt > DateTime.MinValue); // Should be set to current time
//        }

//        [Fact]
//        public async Task GetLocationInventoriesAsync_FiltersCorrectlyByLocationId()
//        {
//            // Arrange
//            var locationId = 5;

//            var inventoryRepositoryMock = new Mock<IInventoryRepository>();
//            inventoryRepositoryMock.Setup(r => r.GetAllAsync(
//                It.IsAny<Expression<Func<Inventory, bool>>>(),
//                It.IsAny<CancellationToken>(),
//                "Product,Location"
//            )).ReturnsAsync(new List<Inventory>());

//            _uowMock.SetupGet(u => u.Inventories).Returns(inventoryRepositoryMock.Object);

//            var service = CreateService();

//            // Act
//            var result = await service.GetLocationInventoriesAsync(locationId, CancellationToken.None);

//            // Assert - since empty list, it should return NotFound, but we still verify the call was made
//            Assert.False(result.IsSuccess);
//            Assert.Equal(ErrorType.NotFound, result.ErrorType);
            
//            // Verify that the expression filters by LocationId = 5
//            inventoryRepositoryMock.Verify(r => r.GetAllAsync(
//                It.Is<Expression<Func<Inventory, bool>>>(expr => expr != null),
//                It.IsAny<CancellationToken>(),
//                "Product,Location"
//            ), Times.Once);
//        }

//        #endregion
//    }
//}