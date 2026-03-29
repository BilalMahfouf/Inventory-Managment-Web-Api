using Application.Shared.Contracts;
using Application.Users.Contracts;
using Application.Locations.DTOs.Request;
using Application.Locations.DTOs.Response;
using Domain.Shared.Results;
using Application.Locations.Services;
using Domain.Shared.Entities;
using Domain.Products.Entities;
using Domain.Shared.Errors;
using Domain.Shared.Exceptions;
using Domain.Inventories;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using System.Linq.Expressions;
using Xunit;

namespace Application.Tests.LocationTests;

public class NewLocationServiceTests
{
    private readonly Mock<IUnitOfWork> _uowMock = new();
    private readonly Mock<IValidator<LocationCreateRequest>> _createValidatorMock = new();
    private readonly Mock<IValidator<LocationUpdateRequest>> _updateValidatorMock = new();
    private readonly Mock<ICurrentUserService> _currentUserServiceMock = new();
    private readonly Mock<IBaseRepository<Location>> _locationRepositoryMock = new();
    private readonly Mock<IBaseRepository<LocationType>> _locationTypeRepositoryMock = new();
    private readonly Mock<IBaseRepository<Inventory>> _inventoryRepositoryMock = new();

    private LocationService CreateService()
    {
        _uowMock.SetupGet(u => u.Locations).Returns(_locationRepositoryMock.Object);
        _uowMock.SetupGet(u => u.LocationTypes).Returns(_locationTypeRepositoryMock.Object);

        return new LocationService(
            _uowMock.Object,
            _currentUserServiceMock.Object,
            _createValidatorMock.Object,
            _updateValidatorMock.Object
        );
    }

    #region GetAllAsync Tests

    [Fact]
    public async Task GetAllAsync_ReturnsSuccess_WhenLocationsExist()
    {
        // Arrange
        var locations = new List<Location>
        {
            new Location { Id = 1, Name = "Location 1", Address = "Address 1", IsDeleted = false },
            new Location { Id = 2, Name = "Location 2", Address = "Address 2", IsDeleted = false }
        };

        _locationRepositoryMock.Setup(r => r.GetAllAsync(
            It.IsAny<Expression<Func<Location, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>()
        )).ReturnsAsync(locations);

        var service = CreateService();

        // Act
        var result = await service.GetAllAsync(CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(2, result.Value.Count());
    }

    [Fact]
    public async Task GetAllAsync_ReturnsNotFound_WhenNoLocationsExist()
    {
        // Arrange
        _locationRepositoryMock.Setup(r => r.GetAllAsync(
            It.IsAny<Expression<Func<Location, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>()
        )).ReturnsAsync(new List<Location>());

        var service = CreateService();

        // Act
        var result = await service.GetAllAsync(CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.NotFound, result.Error.Type);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsException_WhenExceptionThrown()
    {
        // Arrange
        _locationRepositoryMock.Setup(r => r.GetAllAsync(
            It.IsAny<Expression<Func<Location, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>()
        )).ThrowsAsync(new Exception("Database error"));

        var service = CreateService();

        // Act
        var result = await service.GetAllAsync(CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Failure, result.Error.Type);
    }

    #endregion

    #region FindAsync Tests

    [Fact]
    public async Task FindAsync_ReturnsInvalidId_WhenIdIsZero()
    {
        // Arrange
        var service = CreateService();

        // Act
        var result = await service.FindAsync(0, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Validation, result.Error.Type);
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
        Assert.Equal(ErrorType.Validation, result.Error.Type);
    }

    [Fact]
    public async Task FindAsync_ReturnsNotFound_WhenLocationDoesNotExist()
    {
        // Arrange
        _locationRepositoryMock.Setup(r => r.FindAsync(
            It.IsAny<Expression<Func<Location, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>()
        )).ReturnsAsync((Location)null!);

        var service = CreateService();

        // Act
        var result = await service.FindAsync(1, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.NotFound, result.Error.Type);
    }

    [Fact]
    public async Task FindAsync_ReturnsSuccess_WhenLocationExists()
    {
        // Arrange
        var location = new Location
        {
            Id = 1,
            Name = "Test Location",
            Address = "Test Address",
            IsDeleted = false
        };

        _locationRepositoryMock.Setup(r => r.FindAsync(
            It.IsAny<Expression<Func<Location, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>()
        )).ReturnsAsync(location);

        var service = CreateService();

        // Act
        var result = await service.FindAsync(1, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("Test Location", result.Value.Name);
    }

    [Fact]
    public async Task FindAsync_ReturnsException_WhenExceptionThrown()
    {
        // Arrange
        _locationRepositoryMock.Setup(r => r.FindAsync(
            It.IsAny<Expression<Func<Location, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>()
        )).ThrowsAsync(new Exception("Database error"));

        var service = CreateService();

        // Act
        var result = await service.FindAsync(1, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Failure, result.Error.Type);
    }

    #endregion

    #region CreateAsync Tests

    [Fact]
    public async Task CreateAsync_ReturnsFailure_WhenValidationFails()
    {
        // Arrange
        var request = new LocationCreateRequest { Name = "", Address = "", LocationTypeId = 1 };
        var validationResult = new ValidationResult(new[]
        {
            new ValidationFailure("Name", "Name is required")
        });

        _createValidatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);

        var service = CreateService();

        // Act
        var result = await service.CreateAsync(request, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Validation, result.Error.Type);
    }

    [Fact]
    public async Task CreateAsync_ReturnsSuccess_WhenValidRequest()
    {
        // Arrange
        var request = new LocationCreateRequest { Name = "New Location", Address = "456 New St", LocationTypeId = 1 };
        var validationResult = new ValidationResult();

        var location = new Location
        {
            Id = 1,
            Name = "New Location",
            Address = "456 New St",
            LocationTypeId = 1
        };

        _createValidatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);

        _currentUserServiceMock.SetupGet(c => c.UserId).Returns(1);
        _uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        _locationRepositoryMock.Setup(r => r.FindAsync(
            It.IsAny<Expression<Func<Location, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>()
        )).ReturnsAsync(location);

        var service = CreateService();

        // Act
        var result = await service.CreateAsync(request, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task CreateAsync_ReturnsException_WhenExceptionThrown()
    {
        // Arrange
        var request = new LocationCreateRequest { Name = "New Location", Address = "456 New St", LocationTypeId = 1 };
        var validationResult = new ValidationResult();

        _createValidatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);

        _locationRepositoryMock.Setup(r => r.Add(It.IsAny<Location>()))
            .Throws(new Exception("Database error"));

        var service = CreateService();

        // Act
        var result = await service.CreateAsync(request, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Failure, result.Error.Type);
    }

    #endregion

    #region UpdateAsync Tests

    [Fact]
    public async Task UpdateAsync_ReturnsInvalidId_WhenIdIsZero()
    {
        // Arrange
        var request = new LocationUpdateRequest { Id = 0, Name = "Test", Address = "Address", LocationTypeId = 1 };
        var service = CreateService();

        // Act
        var result = await service.UpdateAsync(0, request, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Validation, result.Error.Type);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsFailure_WhenValidationFails()
    {
        // Arrange
        var request = new LocationUpdateRequest { Id = 1, Name = "", Address = "", LocationTypeId = 1 };
        var validationResult = new ValidationResult(new[]
        {
            new ValidationFailure("Name", "Name is required")
        });

        _updateValidatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);

        var service = CreateService();

        // Act
        var result = await service.UpdateAsync(1, request, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Validation, result.Error.Type);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsNotFound_WhenLocationDoesNotExist()
    {
        // Arrange
        var request = new LocationUpdateRequest { Id = 1, Name = "Updated", Address = "New Address", LocationTypeId = 1 };
        var validationResult = new ValidationResult();

        _updateValidatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);

        _locationRepositoryMock.Setup(r => r.FindAsync(
            It.IsAny<Expression<Func<Location, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>()
        )).ReturnsAsync((Location)null!);

        var service = CreateService();

        // Act
        var result = await service.UpdateAsync(1, request, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.NotFound, result.Error.Type);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsSuccess_WhenValidRequest()
    {
        // Arrange
        var request = new LocationUpdateRequest { Id = 1, Name = "Updated Location", Address = "Updated Address", LocationTypeId = 1, IsActive = true };
        var validationResult = new ValidationResult();

        var existingLocation = new Location
        {
            Id = 1,
            Name = "Original Location",
            Address = "Original Address",
            LocationTypeId = 1,
            IsDeleted = false
        };

        _updateValidatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);

        _locationRepositoryMock.Setup(r => r.FindAsync(
            It.IsAny<Expression<Func<Location, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>()
        )).ReturnsAsync(existingLocation);

        _uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var service = CreateService();

        // Act
        var result = await service.UpdateAsync(1, request, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsException_WhenExceptionThrown()
    {
        // Arrange
        var request = new LocationUpdateRequest { Id = 1, Name = "Updated Location", Address = "Updated Address", LocationTypeId = 1 };
        var validationResult = new ValidationResult();

        _updateValidatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);

        _locationRepositoryMock.Setup(r => r.FindAsync(
            It.IsAny<Expression<Func<Location, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>()
        )).ThrowsAsync(new Exception("Database error"));

        var service = CreateService();

        // Act
        var result = await service.UpdateAsync(1, request, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Failure, result.Error.Type);
    }

    #endregion

    #region ActivateAsync Tests

    [Fact]
    public async Task ActivateAsync_ReturnsInvalidId_WhenIdIsZero()
    {
        // Arrange
        var service = CreateService();

        // Act
        var result = await service.ActivateAsync(0, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Validation, result.Error.Type);
    }

    [Fact]
    public async Task ActivateAsync_ReturnsNotFound_WhenLocationDoesNotExist()
    {
        // Arrange
        _locationRepositoryMock.Setup(r => r.FindAsync(
            It.IsAny<Expression<Func<Location, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>()
        )).ReturnsAsync((Location)null!);

        var service = CreateService();

        // Act
        var result = await service.ActivateAsync(1, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.NotFound, result.Error.Type);
    }

    [Fact]
    public async Task ActivateAsync_ReturnsConflict_WhenLocationAlreadyActive()
    {
        // Arrange
        var location = new Location
        {
            Id = 1,
            Name = "Test Location",
            IsActive = true,
            IsDeleted = false
        };

        _locationRepositoryMock.Setup(r => r.FindAsync(
            It.IsAny<Expression<Func<Location, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>()
        )).ReturnsAsync(location);

        var service = CreateService();

        // Act
        var result = await service.ActivateAsync(1, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Conflict, result.Error.Type);
    }

    [Fact]
    public async Task ActivateAsync_ReturnsSuccess_WhenLocationCanBeActivated()
    {
        // Arrange
        var location = new Location
        {
            Id = 1,
            Name = "Test Location",
            IsActive = false,
            IsDeleted = false
        };

        _locationRepositoryMock.Setup(r => r.FindAsync(
            It.IsAny<Expression<Func<Location, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>()
        )).ReturnsAsync(location);

        _uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var service = CreateService();

        // Act
        var result = await service.ActivateAsync(1, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
    }

    #endregion
}
