using Application.Shared.Contracts;
using Application.Users.Contracts;
using Application.UnitOfMeasures.DTOs;
using Domain.Shared.Results;
using Application.UnitOfMeasures.Services;
using Domain.Shared.Entities;
using Domain.Shared.Errors;
using Domain.Shared.Exceptions;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using System.Linq.Expressions;
using Xunit;

namespace Application.Tests.UnitOfMeasureTests;

public class UnitOfMeasureServiceTests
{
    private readonly Mock<IUnitOfWork> _uowMock = new();
    private readonly Mock<IValidator<UnitOfMeasureRequest>> _validatorMock = new();
    private readonly Mock<ICurrentUserService> _currentUserServiceMock = new();
    private readonly Mock<IBaseRepository<UnitOfMeasure>> _unitOfMeasureRepositoryMock = new();

    private UnitOfMeasureService CreateService()
    {
        _uowMock.SetupGet(u => u.UnitOfMeasures).Returns(_unitOfMeasureRepositoryMock.Object);

        return new UnitOfMeasureService(
            _unitOfMeasureRepositoryMock.Object,
            _currentUserServiceMock.Object,
            _uowMock.Object,
            _validatorMock.Object
        );
    }

    #region GetAllAsync Tests

    [Fact]
    public async Task GetAllAsync_ReturnsSuccess_WhenUnitsExist()
    {
        // Arrange
        var units = new List<UnitOfMeasure>
        {
            new UnitOfMeasure { Id = 1, Name = "Kilogram", Description = "Weight unit", IsDeleted = false },
            new UnitOfMeasure { Id = 2, Name = "Meter", Description = "Length unit", IsDeleted = false }
        };

        _unitOfMeasureRepositoryMock.Setup(r => r.GetAllAsync(
            It.IsAny<Expression<Func<UnitOfMeasure, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>()
        )).ReturnsAsync(units);

        var service = CreateService();

        // Act
        var result = await service.GetAllAsync(CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(2, result.Value.Count());
    }

    [Fact]
    public async Task GetAllAsync_ReturnsNotFound_WhenNoUnitsExist()
    {
        // Arrange
        _unitOfMeasureRepositoryMock.Setup(r => r.GetAllAsync(
            It.IsAny<Expression<Func<UnitOfMeasure, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>()
        )).ReturnsAsync(new List<UnitOfMeasure>());

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
        _unitOfMeasureRepositoryMock.Setup(r => r.GetAllAsync(
            It.IsAny<Expression<Func<UnitOfMeasure, bool>>>(),
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
    public async Task FindAsync_ReturnsNotFound_WhenUnitDoesNotExist()
    {
        // Arrange
        _unitOfMeasureRepositoryMock.Setup(r => r.FindAsync(
            It.IsAny<Expression<Func<UnitOfMeasure, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>()
        )).ReturnsAsync((UnitOfMeasure)null!);

        var service = CreateService();

        // Act
        var result = await service.FindAsync(1, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.NotFound, result.Error.Type);
    }

    [Fact]
    public async Task FindAsync_ReturnsSuccess_WhenUnitExists()
    {
        // Arrange
        var unit = new UnitOfMeasure
        {
            Id = 1,
            Name = "Kilogram",
            Description = "Weight unit",
            IsDeleted = false
        };

        _unitOfMeasureRepositoryMock.Setup(r => r.FindAsync(
            It.IsAny<Expression<Func<UnitOfMeasure, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>()
        )).ReturnsAsync(unit);

        var service = CreateService();

        // Act
        var result = await service.FindAsync(1, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("Kilogram", result.Value.Name);
    }

    [Fact]
    public async Task FindAsync_ReturnsException_WhenExceptionThrown()
    {
        // Arrange
        _unitOfMeasureRepositoryMock.Setup(r => r.FindAsync(
            It.IsAny<Expression<Func<UnitOfMeasure, bool>>>(),
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

    #region AddAsync Tests

    [Fact]
    public async Task AddAsync_ReturnsFailure_WhenValidationFails()
    {
        // Arrange
        var request = new UnitOfMeasureRequest { Name = "", Description = null };
        var validationResult = new ValidationResult(new[]
        {
            new ValidationFailure("Name", "Name is required")
        });

        _validatorMock.Setup(v => v.Validate(request)).Returns(validationResult);

        var service = CreateService();

        // Act
        var result = await service.AddAsync(request, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Validation, result.Error.Type);
    }

    [Fact]
    public async Task AddAsync_ReturnsConflict_WhenNameAlreadyExists()
    {
        // Arrange
        var request = new UnitOfMeasureRequest { Name = "Existing Unit", Description = "Description" };
        var validationResult = new ValidationResult();

        _validatorMock.Setup(v => v.Validate(request)).Returns(validationResult);

        _unitOfMeasureRepositoryMock.Setup(r => r.IsExistAsync(
            It.IsAny<Expression<Func<UnitOfMeasure, bool>>>(),
            It.IsAny<CancellationToken>()
        )).ReturnsAsync(true);

        var service = CreateService();

        // Act
        var result = await service.AddAsync(request, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Conflict, result.Error.Type);
    }

    [Fact]
    public async Task AddAsync_ReturnsException_WhenExceptionThrown()
    {
        // Arrange
        var request = new UnitOfMeasureRequest { Name = "New Unit", Description = "Description" };
        var validationResult = new ValidationResult();

        _validatorMock.Setup(v => v.Validate(request)).Returns(validationResult);

        _unitOfMeasureRepositoryMock.Setup(r => r.IsExistAsync(
            It.IsAny<Expression<Func<UnitOfMeasure, bool>>>(),
            It.IsAny<CancellationToken>()
        )).ThrowsAsync(new Exception("Database error"));

        var service = CreateService();

        // Act
        var result = await service.AddAsync(request, CancellationToken.None);

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
        var request = new UnitOfMeasureRequest { Name = "Test", Description = null };
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
        var request = new UnitOfMeasureRequest { Name = "", Description = null };
        var validationResult = new ValidationResult(new[]
        {
            new ValidationFailure("Name", "Name is required")
        });

        _validatorMock.Setup(v => v.Validate(request)).Returns(validationResult);

        var service = CreateService();

        // Act
        var result = await service.UpdateAsync(1, request, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Validation, result.Error.Type);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsNotFound_WhenUnitDoesNotExist()
    {
        // Arrange
        var request = new UnitOfMeasureRequest { Name = "Updated Unit", Description = "Description" };
        var validationResult = new ValidationResult();

        _validatorMock.Setup(v => v.Validate(request)).Returns(validationResult);

        _unitOfMeasureRepositoryMock.Setup(r => r.FindAsync(
            It.IsAny<Expression<Func<UnitOfMeasure, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>()
        )).ReturnsAsync((UnitOfMeasure)null!);

        var service = CreateService();

        // Act
        var result = await service.UpdateAsync(1, request, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.NotFound, result.Error.Type);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsConflict_WhenNameAlreadyExists()
    {
        // Arrange
        var request = new UnitOfMeasureRequest { Name = "Existing Name", Description = "Description" };
        var validationResult = new ValidationResult();

        var existingUnit = new UnitOfMeasure
        {
            Id = 1,
            Name = "Original Unit",
            IsDeleted = false
        };

        _validatorMock.Setup(v => v.Validate(request)).Returns(validationResult);

        _unitOfMeasureRepositoryMock.Setup(r => r.FindAsync(
            It.IsAny<Expression<Func<UnitOfMeasure, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>()
        )).ReturnsAsync(existingUnit);

        _unitOfMeasureRepositoryMock.Setup(r => r.IsExistAsync(
            It.IsAny<Expression<Func<UnitOfMeasure, bool>>>(),
            It.IsAny<CancellationToken>()
        )).ReturnsAsync(true);

        var service = CreateService();

        // Act
        var result = await service.UpdateAsync(1, request, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Conflict, result.Error.Type);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsException_WhenExceptionThrown()
    {
        // Arrange
        var request = new UnitOfMeasureRequest { Name = "Updated Unit", Description = "Description" };
        var validationResult = new ValidationResult();

        _validatorMock.Setup(v => v.Validate(request)).Returns(validationResult);

        _unitOfMeasureRepositoryMock.Setup(r => r.FindAsync(
            It.IsAny<Expression<Func<UnitOfMeasure, bool>>>(),
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
}
