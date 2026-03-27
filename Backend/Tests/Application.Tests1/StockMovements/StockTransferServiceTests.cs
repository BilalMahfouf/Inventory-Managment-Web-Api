using Application.Abstractions.Repositories.Base;
using Application.Abstractions.Repositories.Inventories;
using Application.Abstractions.UnitOfWork;
using Application.DTOs.StockMovements.Request;
using Domain.Shared.Results;
using Application.Services.StockMovements;
using Domain.Shared.Entities;
using Domain.Products.Entities;
using Domain.Shared.Enums;
using Domain.Shared.Exceptions;
using Domain.Inventories;
using Moq;
using System.Linq.Expressions;
using Xunit;

namespace Application.Tests.StockMovementTests;

public class StockTransferServiceTests
{
    private readonly Mock<IUnitOfWork> _uowMock = new();
    private readonly Mock<IInventoryRepository> _inventoryRepositoryMock = new();
    private readonly Mock<IBaseRepository<StockMovement>> _stockMovementRepositoryMock = new();

    private StockTransferService CreateService()
    {
        _uowMock.SetupGet(u => u.Inventories).Returns(_inventoryRepositoryMock.Object);
        _uowMock.SetupGet(u => u.StockMovements).Returns(_stockMovementRepositoryMock.Object);

        return new StockTransferService(_uowMock.Object);
    }

    #region TransferStockAsync Tests

    [Fact]
    public async Task TransferStockAsync_ReturnsNotFound_WhenSourceInventoryDoesNotExist()
    {
        // Arrange
        var request = new StockTransferRequest
        {
            ProductId = 1,
            FromLocationId = 1,
            ToLocationId = 2,
            Quantity = 10
        };

        _inventoryRepositoryMock.Setup(r => r.FindAsync(
            It.IsAny<Expression<Func<Inventory, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>()
        )).ReturnsAsync((Inventory)null!);

        var service = CreateService();

        // Act
        var result = await service.TransferStockAsync(request, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.NotFound, result.ErrorType);
    }

    [Fact]
    public async Task TransferStockAsync_ReturnsNotFound_WhenDestinationInventoryDoesNotExist()
    {
        // Arrange
        var request = new StockTransferRequest
        {
            ProductId = 1,
            FromLocationId = 1,
            ToLocationId = 2,
            Quantity = 10
        };

        var fromInventory = new Inventory
        {
            Id = 1,
            ProductId = 1,
            LocationId = 1,
            QuantityOnHand = 100
        };

        _inventoryRepositoryMock.SetupSequence(r => r.FindAsync(
            It.IsAny<Expression<Func<Inventory, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>()
        ))
        .ReturnsAsync(fromInventory)
        .ReturnsAsync((Inventory)null!);

        var service = CreateService();

        // Act
        var result = await service.TransferStockAsync(request, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.NotFound, result.ErrorType);
    }

    [Fact]
    public async Task TransferStockAsync_ReturnsConflict_WhenDomainExceptionThrown()
    {
        // Arrange
        var request = new StockTransferRequest
        {
            ProductId = 1,
            FromLocationId = 1,
            ToLocationId = 2,
            Quantity = 10
        };

        _inventoryRepositoryMock.Setup(r => r.FindAsync(
            It.IsAny<Expression<Func<Inventory, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>()
        )).ThrowsAsync(new DomainException("Insufficient stock"));

        var service = CreateService();

        // Act
        var result = await service.TransferStockAsync(request, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Conflict, result.ErrorType);
    }

    [Fact]
    public async Task TransferStockAsync_ReturnsException_WhenUnexpectedExceptionThrown()
    {
        // Arrange
        var request = new StockTransferRequest
        {
            ProductId = 1,
            FromLocationId = 1,
            ToLocationId = 2,
            Quantity = 10
        };

        _inventoryRepositoryMock.Setup(r => r.FindAsync(
            It.IsAny<Expression<Func<Inventory, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>()
        )).ThrowsAsync(new Exception("Database error"));

        var service = CreateService();

        // Act
        var result = await service.TransferStockAsync(request, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.InternalServerError, result.ErrorType);
    }

    #endregion
}
