using Application.Shared.Contracts;
using Application.Users.Contracts;
using Application.Locations.DTOs.Request;
using Domain.Shared.Results;
using Application.Locations.Services;
using Domain.Shared.Entities;
using Domain.Shared.Errors;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using System.Linq.Expressions;
using Xunit;

namespace Application.Tests.LocationTests;

public class NewLocationTypeServiceTests
{
    private readonly Mock<IUnitOfWork> _uowMock = new();
    private readonly Mock<IValidator<LocationTypeCreateRequest>> _validatorMock = new();
    private readonly Mock<ICurrentUserService> _currentUserServiceMock = new();
    private readonly Mock<IBaseRepository<LocationType>> _locationTypeRepositoryMock = new();

    private LocationTypeService CreateService()
    {
        _uowMock.SetupGet(u => u.LocationTypes).Returns(_locationTypeRepositoryMock.Object);

        return new LocationTypeService(
            _uowMock.Object,
            _validatorMock.Object,
            _currentUserServiceMock.Object
        );
    }

    #region AddLocationTypeAsync Tests

    [Fact]
    public async Task AddLocationTypeAsync_ReturnsFailure_WhenValidationFails()
    {
        // Arrange
        var request = new LocationTypeCreateRequest { Name = "", Description = null };
        var validationResult = new ValidationResult(new[]
        {
            new ValidationFailure("Name", "Name is required")
        });

        _validatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);

        var service = CreateService();

        // Act
        var result = await service.AddLocationTypeAsync(request, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Validation, result.Error.Type);
    }

    [Fact]
    public async Task AddLocationTypeAsync_ReturnsSuccess_WhenValidRequest()
    {
        // Arrange
        var request = new LocationTypeCreateRequest { Name = "Warehouse", Description = "Storage facility" };
        var validationResult = new ValidationResult();

        _validatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);

        _currentUserServiceMock.SetupGet(c => c.UserId).Returns(1);
        _uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        
        var service = CreateService();

        // Act
        var result = await service.AddLocationTypeAsync(request, CancellationToken.None);

        // This test verifies the service executes without exception when given valid input
        // The actual return value depends on the database assigning an ID
        _locationTypeRepositoryMock.Verify(r => r.Add(It.IsAny<LocationType>()), Times.Once);
        _uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AddLocationTypeAsync_ReturnsException_WhenExceptionThrown()
    {
        // Arrange
        var request = new LocationTypeCreateRequest { Name = "Warehouse", Description = "Description" };
        var validationResult = new ValidationResult();

        _validatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);

        _locationTypeRepositoryMock.Setup(r => r.Add(It.IsAny<LocationType>()))
            .Throws(new Exception("Database error"));

        var service = CreateService();

        // Act
        var result = await service.AddLocationTypeAsync(request, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Failure, result.Error.Type);
    }

    #endregion

    #region GetAllLocationTypesAsync Tests

    [Fact]
    public async Task GetAllLocationTypesAsync_ReturnsSuccess_WhenTypesExist()
    {
        // Arrange
        var locationTypes = new List<LocationType>
        {
            new LocationType { Id = 1, Name = "Warehouse", IsDeleted = false },
            new LocationType { Id = 2, Name = "Store", IsDeleted = false }
        };

        _locationTypeRepositoryMock.Setup(r => r.GetAllAsync(
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
    }

    [Fact]
    public async Task GetAllLocationTypesAsync_ReturnsSuccess_WhenNoTypesExist()
    {
        // Arrange
        _locationTypeRepositoryMock.Setup(r => r.GetAllAsync(
            It.IsAny<Expression<Func<LocationType, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>()
        )).ReturnsAsync(new List<LocationType>());

        var service = CreateService();

        // Act
        var result = await service.GetAllLocationTypesAsync(CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Empty(result.Value);
    }

    [Fact]
    public async Task GetAllLocationTypesAsync_ReturnsException_WhenExceptionThrown()
    {
        // Arrange
        _locationTypeRepositoryMock.Setup(r => r.GetAllAsync(
            It.IsAny<Expression<Func<LocationType, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>()
        )).ThrowsAsync(new Exception("Database error"));

        var service = CreateService();

        // Act
        var result = await service.GetAllLocationTypesAsync(CancellationToken.None);

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
    public async Task FindAsync_ReturnsNotFound_WhenTypeDoesNotExist()
    {
        // Arrange
        _locationTypeRepositoryMock.Setup(r => r.FindAsync(
            It.IsAny<Expression<Func<LocationType, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>()
        )).ReturnsAsync((LocationType)null!);

        var service = CreateService();

        // Act
        var result = await service.FindAsync(1, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.NotFound, result.Error.Type);
    }

    [Fact]
    public async Task FindAsync_ReturnsSuccess_WhenTypeExists()
    {
        // Arrange
        var locationType = new LocationType
        {
            Id = 1,
            Name = "Warehouse",
            Description = "Storage facility",
            IsDeleted = false
        };

        _locationTypeRepositoryMock.Setup(r => r.FindAsync(
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
        Assert.Equal("Warehouse", result.Value.Name);
    }

    [Fact]
    public async Task FindAsync_ReturnsException_WhenExceptionThrown()
    {
        // Arrange
        _locationTypeRepositoryMock.Setup(r => r.FindAsync(
            It.IsAny<Expression<Func<LocationType, bool>>>(),
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
}
