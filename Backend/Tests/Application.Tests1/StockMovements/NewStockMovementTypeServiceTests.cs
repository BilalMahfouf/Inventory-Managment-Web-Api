using Application.Abstractions.Repositories.Base;
using Application.Abstractions.Services.User;
using Application.Abstractions.UnitOfWork;
using Application.DTOs.StockMovements.Request;
using Domain.Shared.Results;
using Application.Services.StockMovements;
using Domain.Shared.Entities;
using Domain.Shared.Enums;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using System.Linq.Expressions;
using Xunit;

namespace Application.Tests.StockMovementTests;

public class StockMovementTypeServiceTests
{
    private readonly Mock<IUnitOfWork> _uowMock = new();
    private readonly Mock<IValidator<StockMovementTypeRequest>> _validatorMock = new();
    private readonly Mock<ICurrentUserService> _currentUserServiceMock = new();
    private readonly Mock<IBaseRepository<StockMovementType>> _stockMovementTypeRepositoryMock = new();

    private StockMovementTypeService CreateService()
    {
        _uowMock.SetupGet(u => u.StockMovementTypes).Returns(_stockMovementTypeRepositoryMock.Object);

        return new StockMovementTypeService(
            _uowMock.Object,
            _validatorMock.Object,
            _currentUserServiceMock.Object
        );
    }

    #region GetAllAsync Tests

    [Fact]
    public async Task GetAllAsync_ReturnsNotFound_WhenNoTypesExist()
    {
        // Arrange
        _stockMovementTypeRepositoryMock.Setup(r => r.GetAllAsync(
            null!,
            It.IsAny<CancellationToken>(),
            It.IsAny<string>()
        )).ReturnsAsync(new List<StockMovementType>());

        var service = CreateService();

        // Act
        var result = await service.GetAllAsync(CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.NotFound, result.ErrorType);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsSuccess_WhenTypesExist()
    {
        // Arrange
        var types = new List<StockMovementType>
        {
            new StockMovementType { Id = 1, Name = "Purchase", Direction = StockMovementDirection.In },
            new StockMovementType { Id = 2, Name = "Sale", Direction = StockMovementDirection.Out }
        };

        _stockMovementTypeRepositoryMock.Setup(r => r.GetAllAsync(
            null!,
            It.IsAny<CancellationToken>(),
            It.IsAny<string>()
        )).ReturnsAsync(types);

        var service = CreateService();

        // Act
        var result = await service.GetAllAsync(CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(2, result.Value.Count());
    }

    [Fact]
    public async Task GetAllAsync_ReturnsException_WhenExceptionThrown()
    {
        // Arrange
        _stockMovementTypeRepositoryMock.Setup(r => r.GetAllAsync(
            null!,
            It.IsAny<CancellationToken>(),
            It.IsAny<string>()
        )).ThrowsAsync(new Exception("Database error"));

        var service = CreateService();

        // Act
        var result = await service.GetAllAsync(CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.InternalServerError, result.ErrorType);
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
        Assert.Equal(ErrorType.BadRequest, result.ErrorType);
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
    }

    [Fact]
    public async Task FindAsync_ReturnsNotFound_WhenTypeDoesNotExist()
    {
        // Arrange
        _stockMovementTypeRepositoryMock.Setup(r => r.FindAsync(
            It.IsAny<Expression<Func<StockMovementType, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>()
        )).ReturnsAsync((StockMovementType)null!);

        var service = CreateService();

        // Act
        var result = await service.FindAsync(1, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.NotFound, result.ErrorType);
    }

    [Fact]
    public async Task FindAsync_ReturnsSuccess_WhenTypeExists()
    {
        // Arrange
        var type = new StockMovementType
        {
            Id = 1,
            Name = "Purchase",
            Description = "Purchase order receipt",
            Direction = StockMovementDirection.In
        };

        _stockMovementTypeRepositoryMock.Setup(r => r.FindAsync(
            It.IsAny<Expression<Func<StockMovementType, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>()
        )).ReturnsAsync(type);

        var service = CreateService();

        // Act
        var result = await service.FindAsync(1, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("Purchase", result.Value.Name);
    }

    [Fact]
    public async Task FindAsync_ReturnsException_WhenExceptionThrown()
    {
        // Arrange
        _stockMovementTypeRepositoryMock.Setup(r => r.FindAsync(
            It.IsAny<Expression<Func<StockMovementType, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>()
        )).ThrowsAsync(new Exception("Database error"));

        var service = CreateService();

        // Act
        var result = await service.FindAsync(1, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.InternalServerError, result.ErrorType);
    }

    #endregion

    #region AddAsync Tests

    [Fact]
    public async Task AddAsync_ReturnsFailure_WhenValidationFails()
    {
        // Arrange
        var request = new StockMovementTypeRequest { Name = "", Description = null, Direction = 1 };
        var validationResult = new ValidationResult(new[]
        {
            new ValidationFailure("Name", "Name is required")
        });

        _validatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);

        var service = CreateService();

        // Act
        var result = await service.AddAsync(request, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.BadRequest, result.ErrorType);
    }

    [Fact]
    public async Task AddAsync_ReturnsSuccess_WhenValidRequest()
    {
        // Arrange
        var request = new StockMovementTypeRequest { Name = "New Type", Description = "Description", Direction = (byte)StockMovementDirection.In };
        var validationResult = new ValidationResult();

        _validatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);

        _stockMovementTypeRepositoryMock.Setup(r => r.Add(It.IsAny<StockMovementType>()));
        _uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var service = CreateService();

        // Act
        var result = await service.AddAsync(request, CancellationToken.None);

        // Verify the expected behavior - the service should call Add and SaveChanges
        _stockMovementTypeRepositoryMock.Verify(r => r.Add(It.IsAny<StockMovementType>()), Times.Once);
        _uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AddAsync_ReturnsException_WhenExceptionThrown()
    {
        // Arrange
        var request = new StockMovementTypeRequest { Name = "New Type", Description = "Description", Direction = 1 };
        var validationResult = new ValidationResult();

        _validatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);

        _stockMovementTypeRepositoryMock.Setup(r => r.Add(It.IsAny<StockMovementType>()))
            .Throws(new Exception("Database error"));

        var service = CreateService();

        // Act
        var result = await service.AddAsync(request, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.InternalServerError, result.ErrorType);
    }

    #endregion

    #region UpdateAsync Tests

    [Fact]
    public async Task UpdateAsync_ReturnsInvalidId_WhenIdIsZero()
    {
        // Arrange
        var request = new StockMovementTypeRequest { Name = "Test", Description = null, Direction = 1 };
        var service = CreateService();

        // Act
        var result = await service.UpdateAsync(0, request, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.BadRequest, result.ErrorType);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsFailure_WhenValidationFails()
    {
        // Arrange
        var request = new StockMovementTypeRequest { Name = "", Description = null, Direction = 1 };
        var validationResult = new ValidationResult(new[]
        {
            new ValidationFailure("Name", "Name is required")
        });

        _validatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);

        var service = CreateService();

        // Act
        var result = await service.UpdateAsync(1, request, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.BadRequest, result.ErrorType);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsConflict_WhenNameAlreadyExists()
    {
        // Arrange
        var request = new StockMovementTypeRequest { Name = "Existing Name", Description = "Description", Direction = 1 };
        var validationResult = new ValidationResult();

        _validatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);

        _stockMovementTypeRepositoryMock.Setup(r => r.IsExistAsync(
            It.IsAny<Expression<Func<StockMovementType, bool>>>(),
            It.IsAny<CancellationToken>()
        )).ReturnsAsync(true);

        var service = CreateService();

        // Act
        var result = await service.UpdateAsync(1, request, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Conflict, result.ErrorType);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsNotFound_WhenTypeDoesNotExist()
    {
        // Arrange
        var request = new StockMovementTypeRequest { Name = "Updated Name", Description = "Description", Direction = 1 };
        var validationResult = new ValidationResult();

        _validatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);

        _stockMovementTypeRepositoryMock.Setup(r => r.IsExistAsync(
            It.IsAny<Expression<Func<StockMovementType, bool>>>(),
            It.IsAny<CancellationToken>()
        )).ReturnsAsync(false);

        _stockMovementTypeRepositoryMock.Setup(r => r.FindAsync(
            It.IsAny<Expression<Func<StockMovementType, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>()
        )).ReturnsAsync((StockMovementType)null!);

        var service = CreateService();

        // Act
        var result = await service.UpdateAsync(1, request, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.NotFound, result.ErrorType);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsSuccess_WhenValidRequest()
    {
        // Arrange
        var request = new StockMovementTypeRequest { Name = "Updated Type", Description = "Updated Description", Direction = (byte)StockMovementDirection.Out };
        var validationResult = new ValidationResult();

        var existingType = new StockMovementType
        {
            Id = 1,
            Name = "Original Type",
            Direction = StockMovementDirection.In
        };

        var updatedType = new StockMovementType
        {
            Id = 1,
            Name = "Updated Type",
            Description = "Updated Description",
            Direction = StockMovementDirection.Out
        };

        _validatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);

        _stockMovementTypeRepositoryMock.Setup(r => r.IsExistAsync(
            It.IsAny<Expression<Func<StockMovementType, bool>>>(),
            It.IsAny<CancellationToken>()
        )).ReturnsAsync(false);

        _stockMovementTypeRepositoryMock.SetupSequence(r => r.FindAsync(
            It.IsAny<Expression<Func<StockMovementType, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>()
        ))
        .ReturnsAsync(existingType)
        .ReturnsAsync(updatedType);

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
        var request = new StockMovementTypeRequest { Name = "Updated Type", Description = "Description", Direction = 1 };
        var validationResult = new ValidationResult();

        _validatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);

        _stockMovementTypeRepositoryMock.Setup(r => r.IsExistAsync(
            It.IsAny<Expression<Func<StockMovementType, bool>>>(),
            It.IsAny<CancellationToken>()
        )).ThrowsAsync(new Exception("Database error"));

        var service = CreateService();

        // Act
        var result = await service.UpdateAsync(1, request, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.InternalServerError, result.ErrorType);
    }

    #endregion
}
