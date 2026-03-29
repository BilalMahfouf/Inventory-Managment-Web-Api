using Application.Shared.Contracts;
using Application.Inventories;
using Application.Products.Contracts;
using Application.Users.Contracts;
using Application.Inventories.DTOs.Request;
using Application.Inventories.DTOs;
using Domain.Shared.Results;
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

namespace Application.Tests.InventoryTests;

public class InventoryServiceTests
{
    private readonly Mock<IUnitOfWork> _uowMock = new();
    private readonly Mock<IInventoryRepository> _inventoryRepositoryMock = new();
    private readonly Mock<IValidator<InventoryCreateRequest>> _createValidatorMock = new();
    private readonly Mock<IValidator<InventoryUpdateRequest>> _updateValidatorMock = new();
    private readonly Mock<ICurrentUserService> _currentUserServiceMock = new();
    private readonly Mock<IProductRepository> _productRepositoryMock = new();

    private InventoryService CreateService()
    {
        _uowMock.SetupGet(u => u.Inventories).Returns(_inventoryRepositoryMock.Object);
        _uowMock.SetupGet(u => u.Products).Returns(_productRepositoryMock.Object);

        return new InventoryService(
            _uowMock.Object,
            _createValidatorMock.Object,
            _updateValidatorMock.Object,
            _currentUserServiceMock.Object
        );
    }

    #region GetAllAsync Tests

    [Fact]
    public async Task GetAllAsync_ReturnsSuccess_WhenInventoriesExist()
    {
        // Arrange
        var inventories = new List<Inventory>
        {
            new Inventory
            {
                Id = 1,
                ProductId = 1,
                LocationId = 1,
                QuantityOnHand = 100,
                ReorderLevel = 10,
                MaxLevel = 500,
                Product = new Product { Name = "Product 1" },
                Location = new Location { Name = "Location 1" }
            }
        };

        _inventoryRepositoryMock.Setup(r => r.GetAllAsync(
            null!,
            It.IsAny<CancellationToken>(),
            It.IsAny<string>()
        )).ReturnsAsync(inventories);

        var service = CreateService();

        // Act
        var result = await service.GetAllAsync(CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Single(result.Value);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsNotFound_WhenNoInventoriesExist()
    {
        // Arrange
        _inventoryRepositoryMock.Setup(r => r.GetAllAsync(
            null!,
            It.IsAny<CancellationToken>(),
            It.IsAny<string>()
        )).ReturnsAsync(new List<Inventory>());

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
        _inventoryRepositoryMock.Setup(r => r.GetAllAsync(
            null!,
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
    public async Task FindAsync_ReturnsNotFound_WhenInventoryDoesNotExist()
    {
        // Arrange
        _inventoryRepositoryMock.Setup(r => r.FindAsync(
            It.IsAny<Expression<Func<Inventory, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>()
        )).ReturnsAsync((Inventory)null!);

        var service = CreateService();

        // Act
        var result = await service.FindAsync(1, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.NotFound, result.Error.Type);
    }

    [Fact]
    public async Task FindAsync_ReturnsSuccess_WhenInventoryExists()
    {
        // Arrange
        var inventory = new Inventory
        {
            Id = 1,
            ProductId = 1,
            LocationId = 1,
            QuantityOnHand = 100,
            Product = new Product { Name = "Test Product" },
            Location = new Location { Name = "Test Location" }
        };

        _inventoryRepositoryMock.Setup(r => r.FindAsync(
            It.IsAny<Expression<Func<Inventory, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>()
        )).ReturnsAsync(inventory);

        var service = CreateService();

        // Act
        var result = await service.FindAsync(1, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
    }

    #endregion

    #region CreateAsync Tests

    [Fact]
    public async Task CreateAsync_ReturnsFailure_WhenValidationFails()
    {
        // Arrange
        var request = new InventoryCreateRequest();
        var validationResult = new ValidationResult(new[]
        {
            new ValidationFailure("ProductId", "ProductId is required")
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
    public async Task CreateAsync_ReturnsConflict_WhenInventoryAlreadyExists()
    {
        // Arrange
        var request = new InventoryCreateRequest
        {
            ProductId = 1,
            LocationId = 1,
            QuantityOnHand = 100,
            ReorderLevel = 10,
            MaxLevel = 500
        };

        var validationResult = new ValidationResult();

        _createValidatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);

        _inventoryRepositoryMock.Setup(r => r.IsExistAsync(
            It.IsAny<Expression<Func<Inventory, bool>>>(),
            It.IsAny<CancellationToken>()
        )).ReturnsAsync(true);

        var service = CreateService();

        // Act
        var result = await service.CreateAsync(request, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Conflict, result.Error.Type);
    }

    [Fact]
    public async Task CreateAsync_ReturnsNotFound_WhenProductDoesNotExist()
    {
        // Arrange
        var request = new InventoryCreateRequest
        {
            ProductId = 999,
            LocationId = 1,
            QuantityOnHand = 100,
            ReorderLevel = 10,
            MaxLevel = 500
        };

        var validationResult = new ValidationResult();

        _createValidatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);

        _inventoryRepositoryMock.Setup(r => r.IsExistAsync(
            It.IsAny<Expression<Func<Inventory, bool>>>(),
            It.IsAny<CancellationToken>()
        )).ReturnsAsync(false);

        _productRepositoryMock.Setup(r => r.FindAsync(
            It.IsAny<Expression<Func<Product, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>()
        )).ReturnsAsync((Product)null!);

        var service = CreateService();

        // Act
        var result = await service.CreateAsync(request, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.NotFound, result.Error.Type);
    }

    [Fact]
    public async Task CreateAsync_ReturnsConflict_WhenDomainExceptionThrown()
    {
        // Arrange
        var request = new InventoryCreateRequest
        {
            ProductId = 1,
            LocationId = 1,
            QuantityOnHand = 100,
            ReorderLevel = 10,
            MaxLevel = 500
        };

        var validationResult = new ValidationResult();

        _createValidatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);

        _inventoryRepositoryMock.Setup(r => r.IsExistAsync(
            It.IsAny<Expression<Func<Inventory, bool>>>(),
            It.IsAny<CancellationToken>()
        )).ThrowsAsync(new DomainException("Domain error"));

        var service = CreateService();

        // Act
        var result = await service.CreateAsync(request, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Conflict, result.Error.Type);
    }

    #endregion

    #region UpdateAsync Tests

    [Fact]
    public async Task UpdateAsync_ReturnsInvalidId_WhenIdIsZero()
    {
        // Arrange
        var request = new InventoryUpdateRequest();
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
        var request = new InventoryUpdateRequest();
        var validationResult = new ValidationResult(new[]
        {
            new ValidationFailure("QuantityOnHand", "QuantityOnHand is required")
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
    public async Task UpdateAsync_ReturnsNotFound_WhenInventoryDoesNotExist()
    {
        // Arrange
        var request = new InventoryUpdateRequest
        {
            QuantityOnHand = 150,
            ReorderLevel = 15,
            MaxLevel = 600
        };

        var validationResult = new ValidationResult();

        _updateValidatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);

        _inventoryRepositoryMock.Setup(r => r.FindAsync(
            It.IsAny<Expression<Func<Inventory, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>()
        )).ReturnsAsync((Inventory)null!);

        var service = CreateService();

        // Act
        var result = await service.UpdateAsync(1, request, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.NotFound, result.Error.Type);
    }

    #endregion

    #region DeleteAsync Tests

    [Fact]
    public async Task DeleteAsync_ReturnsInvalidId_WhenIdIsZero()
    {
        // Arrange
        var service = CreateService();

        // Act
        var result = await service.DeleteAsync(0, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Validation, result.Error.Type);
    }

    [Fact]
    public async Task DeleteAsync_ReturnsNotFound_WhenInventoryDoesNotExist()
    {
        // Arrange
        _inventoryRepositoryMock.Setup(r => r.FindAsync(
            It.IsAny<Expression<Func<Inventory, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>()
        )).ReturnsAsync((Inventory)null!);

        var service = CreateService();

        // Act
        var result = await service.DeleteAsync(1, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.NotFound, result.Error.Type);
    }

    [Fact]
    public async Task DeleteAsync_ReturnsSuccess_WhenInventoryExists()
    {
        // Arrange
        var inventory = new Inventory
        {
            Id = 1,
            ProductId = 1,
            LocationId = 1
        };

        _inventoryRepositoryMock.Setup(r => r.FindAsync(
            It.IsAny<Expression<Func<Inventory, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>()
        )).ReturnsAsync(inventory);

        _uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var service = CreateService();

        // Act
        var result = await service.DeleteAsync(1, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
    }

    #endregion

    #region GetInventoryLowStockAsync Tests

    [Fact]
    public async Task GetInventoryLowStockAsync_ReturnsNotFound_WhenNoLowStockInventories()
    {
        // Arrange
        _inventoryRepositoryMock.Setup(r => r.GetAllAsync(
            It.IsAny<Expression<Func<Inventory, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>()
        )).ReturnsAsync(new List<Inventory>());

        var service = CreateService();

        // Act
        var result = await service.GetInventoryLowStockAsync(CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.NotFound, result.Error.Type);
    }

    [Fact]
    public async Task GetInventoryLowStockAsync_ReturnsSuccess_WhenLowStockInventoriesExist()
    {
        // Arrange
        var inventories = new List<Inventory>
        {
            new Inventory
            {
                Id = 1,
                ProductId = 1,
                LocationId = 1,
                QuantityOnHand = 5,
                ReorderLevel = 10,
                Product = new Product { Name = "Low Stock Product" },
                Location = new Location { Name = "Warehouse" }
            }
        };

        _inventoryRepositoryMock.Setup(r => r.GetAllAsync(
            It.IsAny<Expression<Func<Inventory, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>()
        )).ReturnsAsync(inventories);

        var service = CreateService();

        // Act
        var result = await service.GetInventoryLowStockAsync(CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
    }

    #endregion

    #region GetInventoryValuationAsync Tests

    [Fact]
    public async Task GetInventoryValuationAsync_ReturnsSuccess()
    {
        // Arrange
        _inventoryRepositoryMock.Setup(r => r.GetInventoryValuationAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(25000.50m);

        var service = CreateService();

        // Act
        var result = await service.GetInventoryValuationAsync(CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(25000.50m, result.Value);
    }

    [Fact]
    public async Task GetInventoryValuationAsync_ReturnsException_WhenExceptionThrown()
    {
        // Arrange
        _inventoryRepositoryMock.Setup(r => r.GetInventoryValuationAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        var service = CreateService();

        // Act
        var result = await service.GetInventoryValuationAsync(CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Failure, result.Error.Type);
    }

    #endregion

    #region GetInventoryCostAsync Tests

    [Fact]
    public async Task GetInventoryCostAsync_ReturnsSuccess()
    {
        // Arrange
        _inventoryRepositoryMock.Setup(r => r.GetInventoryCostAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(15000.75m);

        var service = CreateService();

        // Act
        var result = await service.GetInventoryCostAsync(CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(15000.75m, result.Value);
    }

    [Fact]
    public async Task GetInventoryCostAsync_ReturnsException_WhenExceptionThrown()
    {
        // Arrange
        _inventoryRepositoryMock.Setup(r => r.GetInventoryCostAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        var service = CreateService();

        // Act
        var result = await service.GetInventoryCostAsync(CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Failure, result.Error.Type);
    }

    #endregion

    #region DeleteByIdAsync Tests

    [Fact]
    public async Task DeleteByIdAsync_ReturnsNotFound_WhenInventoryDoesNotExist()
    {
        // Arrange
        _inventoryRepositoryMock.Setup(r => r.FindAsync(
            It.IsAny<Expression<Func<Inventory, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>()
        )).ReturnsAsync((Inventory)null!);

        var service = CreateService();

        // Act
        var result = await service.DeleteByIdAsync(1, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.NotFound, result.Error.Type);
    }

    [Fact]
    public async Task DeleteByIdAsync_ReturnsConflict_WhenDomainExceptionThrown()
    {
        // Arrange
        _inventoryRepositoryMock.Setup(r => r.FindAsync(
            It.IsAny<Expression<Func<Inventory, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>()
        )).ThrowsAsync(new DomainException("Cannot delete inventory with pending reservations"));

        var service = CreateService();

        // Act
        var result = await service.DeleteByIdAsync(1, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Conflict, result.Error.Type);
    }

    #endregion
}
