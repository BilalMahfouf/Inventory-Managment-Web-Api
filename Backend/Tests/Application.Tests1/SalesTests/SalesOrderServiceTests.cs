using Application.Abstractions.Repositories.Base;
using Application.Abstractions.Repositories.Inventories;
using Application.Abstractions.Repositories.Products;
using Application.Abstractions.UnitOfWork;
using Domain.Shared.Results;
using Application.Sales.RequestResponse;
using Application.Sales.Services1;
using Domain.Shared.Entities;
using Domain.Products.Entities;
using Domain.Shared.Enums;
using Domain.Shared.Exceptions;
using Domain.Inventories;
using Domain.Sales;
using Moq;
using System.Linq.Expressions;
using Xunit;

using AppSalesOrderItemRequest = Application.Sales.RequestResponse.SalesOrderItemRequest;

namespace Application.Tests.SalesTests;

public class SalesOrderServiceTests
{
    private readonly Mock<IUnitOfWork> _uowMock = new();
    private readonly Mock<IProductRepository> _productRepositoryMock = new();
    private readonly Mock<IBaseRepository<SalesOrder>> _salesOrderRepositoryMock = new();
    private readonly Mock<IBaseRepository<StockMovement>> _stockMovementRepositoryMock = new();
    private readonly Mock<IInventoryRepository> _inventoryRepositoryMock = new();

    private SalesOrderService CreateService()
    {
        _uowMock.SetupGet(u => u.Products).Returns(_productRepositoryMock.Object);
        _uowMock.SetupGet(u => u.SalesOrders).Returns(_salesOrderRepositoryMock.Object);
        _uowMock.SetupGet(u => u.StockMovements).Returns(_stockMovementRepositoryMock.Object);
        _uowMock.SetupGet(u => u.Inventories).Returns(_inventoryRepositoryMock.Object);

        return new SalesOrderService(_uowMock.Object);
    }

    #region CreateSalesOrderAsync Tests

    [Fact]
    public async Task CreateSalesOrderAsync_ReturnsNotFound_WhenProductDoesNotExist()
    {
        // Arrange
        var request = new CreateSalesOrderRequest(
            CustomerId: 1,
            Items: new List<AppSalesOrderItemRequest>
            {
                new AppSalesOrderItemRequest(ProductId: 1, Quantity: 10)
            },
            SalesStatus: SalesOrderStatus.Pending,
            Description: "Test Order"
        );

        _productRepositoryMock.Setup(r => r.FindAsync(
            It.IsAny<Expression<Func<Product, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>()
        )).ReturnsAsync((Product)null!);

        var service = CreateService();

        // Act
        var result = await service.CreateSalesOrderAsync(request, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.NotFound, result.ErrorType);
    }

    [Fact]
    public async Task CreateSalesOrderAsync_ReturnsFailure_WhenNoItemsProvided()
    {
        // Arrange
        var request = new CreateSalesOrderRequest(
            CustomerId: 1,
            Items: new List<AppSalesOrderItemRequest>(),
            SalesStatus: SalesOrderStatus.Pending,
            Description: "Test Order"
        );

        var service = CreateService();

        // Act
        var result = await service.CreateSalesOrderAsync(request, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.NotFound, result.ErrorType);
        Assert.Contains("at least one item", result.ErrorMessage);
    }

    [Fact]
    public async Task CreateSalesOrderAsync_ReturnsConflict_WhenDomainExceptionThrown()
    {
        // Arrange
        var request = new CreateSalesOrderRequest(
            CustomerId: 1,
            Items: new List<AppSalesOrderItemRequest>
            {
                new AppSalesOrderItemRequest(ProductId: 1, Quantity: 10)
            },
            SalesStatus: SalesOrderStatus.Pending,
            Description: "Test Order"
        );

        _productRepositoryMock.Setup(r => r.FindAsync(
            It.IsAny<Expression<Func<Product, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>()
        )).ThrowsAsync(new DomainException("Domain error"));

        var service = CreateService();

        // Act
        var result = await service.CreateSalesOrderAsync(request, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Conflict, result.ErrorType);
    }

    [Fact]
    public async Task CreateSalesOrderAsync_ReturnsException_WhenUnexpectedExceptionThrown()
    {
        // Arrange
        var request = new CreateSalesOrderRequest(
            CustomerId: 1,
            Items: new List<AppSalesOrderItemRequest>
            {
                new AppSalesOrderItemRequest(ProductId: 1, Quantity: 10)
            },
            SalesStatus: SalesOrderStatus.Pending,
            Description: "Test Order"
        );

        _productRepositoryMock.Setup(r => r.FindAsync(
            It.IsAny<Expression<Func<Product, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>()
        )).ThrowsAsync(new Exception("Database error"));

        var service = CreateService();

        // Act
        var result = await service.CreateSalesOrderAsync(request, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.InternalServerError, result.ErrorType);
    }

    #endregion

    #region CompleteOrderAsync Tests

    [Fact]
    public async Task CompleteOrderAsync_ReturnsNotFound_WhenOrderDoesNotExist()
    {
        // Arrange
        _salesOrderRepositoryMock.Setup(r => r.FindAsync(
            It.IsAny<Expression<Func<SalesOrder, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>()
        )).ReturnsAsync((SalesOrder)null!);

        var service = CreateService();

        // Act
        var result = await service.CompleteOrderAsync(1, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.NotFound, result.ErrorType);
    }

    #endregion
}
