using Application.Inventories;
using Application.Sales.RequestResponse;
using Application.Sales.Services;
using Application.Shared.Contracts;
using Domain.Inventories.Entities;
using Domain.Products.Entities;
using Domain.Sales;
using Domain.Shared.Entities;
using Domain.Shared.Errors;
using Domain.Shared.Exceptions;
using Moq;
using System.Linq.Expressions;
using Xunit;

using AppSalesOrderItemRequest = Application.Sales.RequestResponse.SalesOrderItemRequest;

namespace Application.Tests.SalesTests;

public class SalesOrderServiceTests
{
    private readonly Mock<IUnitOfWork> _uowMock = new();
    private readonly Mock<IBaseRepository<SalesOrder>> _salesOrderRepositoryMock = new();
    private readonly Mock<IInventoryRepository> _inventoryRepositoryMock = new();

    private SalesOrderService CreateService()
    {
        _uowMock.SetupGet(u => u.SalesOrders).Returns(_salesOrderRepositoryMock.Object);
        _uowMock.SetupGet(u => u.Inventories).Returns(_inventoryRepositoryMock.Object);

        return new SalesOrderService(_uowMock.Object);
    }

    [Fact]
    public async Task CreateSalesOrderAsync_ReturnsNotFound_WhenInventoryDoesNotExist()
    {
        var request = new CreateSalesOrderRequest(
            CustomerId: 1,
            Description: "Test Order",
            IsWalkIn: false,
            ShippingAddress: "Main Street",
            Items: new List<AppSalesOrderItemRequest>
            {
                new(ProductId: 1, InventoryId: 99, Quantity: 10),
            });

        _inventoryRepositoryMock.Setup(r => r.FindAsync(
            It.IsAny<Expression<Func<Inventory, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>())).ReturnsAsync((Inventory)null!);

        var service = CreateService();

        var result = await service.CreateSalesOrderAsync(request, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.NotFound, result.Error.Type);
    }

    [Fact]
    public async Task CreateSalesOrderAsync_ReturnsValidation_WhenNoItemsProvided()
    {
        var request = new CreateSalesOrderRequest(
            CustomerId: 1,
            Description: "Test Order",
            IsWalkIn: false,
            ShippingAddress: "Main Street",
            Items: new List<AppSalesOrderItemRequest>());

        var service = CreateService();

        var result = await service.CreateSalesOrderAsync(request, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Validation, result.Error.Type);
        Assert.Contains("at least one item", result.Error.Description);
    }

    [Fact]
    public async Task CreateSalesOrderAsync_ReturnsConflict_WhenDomainExceptionThrown()
    {
        var request = new CreateSalesOrderRequest(
            CustomerId: 1,
            Description: "Test Order",
            IsWalkIn: false,
            ShippingAddress: "Main Street",
            Items: new List<AppSalesOrderItemRequest>
            {
                new(ProductId: 1, InventoryId: 1, Quantity: 10),
            });

        _inventoryRepositoryMock.Setup(r => r.FindAsync(
            It.IsAny<Expression<Func<Inventory, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>())).ThrowsAsync(new DomainException("Domain error"));

        var service = CreateService();

        var result = await service.CreateSalesOrderAsync(request, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Conflict, result.Error.Type);
    }

    [Fact]
    public async Task CreateSalesOrderAsync_ReturnsFailure_WhenUnexpectedExceptionThrown()
    {
        var request = new CreateSalesOrderRequest(
            CustomerId: 1,
            Description: "Test Order",
            IsWalkIn: false,
            ShippingAddress: "Main Street",
            Items: new List<AppSalesOrderItemRequest>
            {
                new(ProductId: 1, InventoryId: 1, Quantity: 10),
            });

        _inventoryRepositoryMock.Setup(r => r.FindAsync(
            It.IsAny<Expression<Func<Inventory, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>())).ThrowsAsync(new Exception("Database error"));

        var service = CreateService();

        var result = await service.CreateSalesOrderAsync(request, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Failure, result.Error.Type);
    }

    [Fact]
    public async Task CompleteOrderAsync_ReturnsNotFound_WhenOrderDoesNotExist()
    {
        _salesOrderRepositoryMock.Setup(r => r.FindAsync(
            It.IsAny<Expression<Func<SalesOrder, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>())).ReturnsAsync((SalesOrder)null!);

        var service = CreateService();

        var result = await service.CompleteOrderAsync(1, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.NotFound, result.Error.Type);
    }
}
