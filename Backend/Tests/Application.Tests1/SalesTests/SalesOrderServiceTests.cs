using Application.Inventories;
using Application.Sales.RequestResponse;
using Application.Sales.Services;
using Application.Shared.Contracts;
using Domain.Inventories.Entities;
using Domain.Products.Entities;
using Domain.Sales;
using Domain.Sales.Entities;
using Domain.Sales.Enums;
using Domain.Shared.Entities;
using Domain.Shared.Errors;
using Domain.Shared.Exceptions;
using Moq;
using System.Linq.Expressions;
using System.Reflection;
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
        _uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        return new SalesOrderService(_uowMock.Object);
    }

    private static Product CreateActiveProduct(int productId, decimal unitPrice = 10m)
    {
        return new Product
        {
            Id = productId,
            IsActive = true,
            UnitPrice = unitPrice,
            Name = $"Product-{productId}",
            Sku = $"SKU-{productId}",
        };
    }

    private static Inventory CreateInventory(
        int inventoryId,
        int productId,
        decimal quantityOnHand,
        decimal unitPrice = 10m,
        int locationId = 1,
        decimal maxLevel = 1000m)
    {
        var product = CreateActiveProduct(productId, unitPrice);

        return new Inventory
        {
            Id = inventoryId,
            ProductId = productId,
            Product = product,
            LocationId = locationId,
            QuantityOnHand = quantityOnHand,
            MaxLevel = maxLevel,
            ReorderLevel = 1m,
        };
    }

    private static Domain.Sales.Entities.SalesOrderItemRequest CreateDomainOrderItemRequest(
        Inventory inventory,
        decimal quantity)
    {
        return new Domain.Sales.Entities.SalesOrderItemRequest
        {
            InventoryId = inventory.Id,
            Inventory = inventory,
            Product = inventory.Product,
            Quantity = quantity,
        };
    }

    private static SalesOrder CreatePendingOrder(
        Inventory inventory,
        decimal quantity = 5m,
        int orderId = 0)
    {
        var order = SalesOrder.Create(
            1,
            new List<Domain.Sales.Entities.SalesOrderItemRequest>
            {
                CreateDomainOrderItemRequest(inventory, quantity),
            },
            "Order",
            "Address");

        order.Id = orderId;
        return order;
    }

    private static void AttachInventoryToAllOrderItems(SalesOrder order, Inventory inventory)
    {
        var inventoryProperty = typeof(SalesOrderItem)
            .GetProperty(nameof(SalesOrderItem.Inventory), BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)!;

        foreach (var item in order.Items)
        {
            inventoryProperty.SetValue(item, inventory);
        }
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
    public async Task CreateSalesOrderAsync_ReturnsValidation_WhenItemsIsNull()
    {
        var request = new CreateSalesOrderRequest(
            CustomerId: 1,
            Description: "Test Order",
            IsWalkIn: false,
            ShippingAddress: "Main Street",
            Items: null!);

        var service = CreateService();

        var result = await service.CreateSalesOrderAsync(request, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Validation, result.Error.Type);
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
    public async Task CreateSalesOrderAsync_ReturnsValidation_WhenCustomerMissingForNonWalkIn()
    {
        var request = new CreateSalesOrderRequest(
            CustomerId: null,
            Description: "Order",
            IsWalkIn: false,
            ShippingAddress: "Main Street",
            Items: new List<AppSalesOrderItemRequest>
            {
                new(ProductId: 1, InventoryId: 1, Quantity: 1),
            });

        var service = CreateService();

        var result = await service.CreateSalesOrderAsync(request, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Validation, result.Error.Type);
    }

    [Fact]
    public async Task CreateSalesOrderAsync_ReturnsValidation_WhenWalkInHasCustomer()
    {
        var request = new CreateSalesOrderRequest(
            CustomerId: 10,
            Description: "Order",
            IsWalkIn: true,
            ShippingAddress: null,
            Items: new List<AppSalesOrderItemRequest>
            {
                new(ProductId: 1, InventoryId: 1, Quantity: 1),
            });

        var service = CreateService();

        var result = await service.CreateSalesOrderAsync(request, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Validation, result.Error.Type);
    }

    [Fact]
    public async Task CreateSalesOrderAsync_ReturnsValidation_WhenItemQuantityIsNotPositive()
    {
        var request = new CreateSalesOrderRequest(
            CustomerId: 1,
            Description: "Order",
            IsWalkIn: false,
            ShippingAddress: "Address",
            Items: new List<AppSalesOrderItemRequest>
            {
                new(ProductId: 1, InventoryId: 1, Quantity: 0),
            });

        var service = CreateService();

        var result = await service.CreateSalesOrderAsync(request, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Validation, result.Error.Type);
    }

    [Fact]
    public async Task CreateSalesOrderAsync_ReturnsValidation_WhenInventoryProductDoesNotMatch()
    {
        var request = new CreateSalesOrderRequest(
            CustomerId: 1,
            Description: "Order",
            IsWalkIn: false,
            ShippingAddress: "Address",
            Items: new List<AppSalesOrderItemRequest>
            {
                new(ProductId: 2, InventoryId: 1, Quantity: 3),
            });

        var inventory = CreateInventory(inventoryId: 1, productId: 1, quantityOnHand: 100);
        _inventoryRepositoryMock.Setup(r => r.FindAsync(
            It.IsAny<Expression<Func<Inventory, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>())).ReturnsAsync(inventory);

        var service = CreateService();

        var result = await service.CreateSalesOrderAsync(request, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Validation, result.Error.Type);
    }

    [Fact]
    public async Task CreateSalesOrderAsync_ReturnsConflict_WhenInventoryStockIsInsufficient()
    {
        var request = new CreateSalesOrderRequest(
            CustomerId: 1,
            Description: "Order",
            IsWalkIn: false,
            ShippingAddress: "Address",
            Items: new List<AppSalesOrderItemRequest>
            {
                new(ProductId: 1, InventoryId: 1, Quantity: 50),
            });

        var inventory = CreateInventory(inventoryId: 1, productId: 1, quantityOnHand: 10);
        _inventoryRepositoryMock.Setup(r => r.FindAsync(
            It.IsAny<Expression<Func<Inventory, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>())).ReturnsAsync(inventory);

        var service = CreateService();

        var result = await service.CreateSalesOrderAsync(request, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Conflict, result.Error.Type);
    }

    [Fact]
    public async Task CreateSalesOrderAsync_ReturnsSuccess_WhenRequestIsValidNonWalkIn()
    {
        var request = new CreateSalesOrderRequest(
            CustomerId: 1,
            Description: "Order",
            IsWalkIn: false,
            ShippingAddress: "Address",
            Items: new List<AppSalesOrderItemRequest>
            {
                new(ProductId: 1, InventoryId: 11, Quantity: 4),
            });

        var inventory = CreateInventory(inventoryId: 11, productId: 1, quantityOnHand: 30);
        _inventoryRepositoryMock.Setup(r => r.FindAsync(
            It.IsAny<Expression<Func<Inventory, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>())).ReturnsAsync(inventory);

        _salesOrderRepositoryMock
            .Setup(r => r.Add(It.IsAny<SalesOrder>()))
            .Callback<SalesOrder>(o => o.Id = 123);

        var service = CreateService();

        var result = await service.CreateSalesOrderAsync(request, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(123, result.Value);
        _salesOrderRepositoryMock.Verify(r => r.Add(It.IsAny<SalesOrder>()), Times.Once);
        _uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateSalesOrderAsync_ReturnsSuccess_WhenRequestIsValidWalkIn()
    {
        var request = new CreateSalesOrderRequest(
            CustomerId: null,
            Description: "Walk in",
            IsWalkIn: true,
            ShippingAddress: null,
            Items: new List<AppSalesOrderItemRequest>
            {
                new(ProductId: 1, InventoryId: 11, Quantity: 2),
            });

        var inventory = CreateInventory(inventoryId: 11, productId: 1, quantityOnHand: 30);
        _inventoryRepositoryMock.Setup(r => r.FindAsync(
            It.IsAny<Expression<Func<Inventory, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>())).ReturnsAsync(inventory);

        _salesOrderRepositoryMock
            .Setup(r => r.Add(It.IsAny<SalesOrder>()))
            .Callback<SalesOrder>(o => o.Id = 124);

        var service = CreateService();

        var result = await service.CreateSalesOrderAsync(request, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(124, result.Value);
        _salesOrderRepositoryMock.Verify(r => r.Add(It.IsAny<SalesOrder>()), Times.Once);
        _uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
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
    public async Task UpdateSalesOrderAsync_ReturnsNotFound_WhenOrderDoesNotExist()
    {
        _salesOrderRepositoryMock.Setup(r => r.FindAsync(
            It.IsAny<Expression<Func<SalesOrder, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>())).ReturnsAsync((SalesOrder)null!);

        var request = new UpdateSalesOrderRequest(1, "Desc", "Address", null);
        var service = CreateService();

        var result = await service.UpdateSalesOrderAsync(10, request, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.NotFound, result.Error.Type);
    }

    [Fact]
    public async Task UpdateSalesOrderAsync_ReturnsConflict_WhenOrderIsNotPending()
    {
        var inventory = CreateInventory(inventoryId: 1, productId: 1, quantityOnHand: 50);
        var order = SalesOrder.CreateWalkIn(
            new List<Domain.Sales.Entities.SalesOrderItemRequest>
            {
                CreateDomainOrderItemRequest(inventory, 2),
            },
            "Walk in");

        _salesOrderRepositoryMock.Setup(r => r.FindAsync(
            It.IsAny<Expression<Func<SalesOrder, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>())).ReturnsAsync(order);

        var request = new UpdateSalesOrderRequest(1, "Desc", "Address", null);
        var service = CreateService();

        var result = await service.UpdateSalesOrderAsync(10, request, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Conflict, result.Error.Type);
    }

    [Fact]
    public async Task UpdateSalesOrderAsync_ReturnsSuccess_WhenItemsAreNull()
    {
        var inventory = CreateInventory(inventoryId: 1, productId: 1, quantityOnHand: 50);
        var order = CreatePendingOrder(inventory);

        _salesOrderRepositoryMock.Setup(r => r.FindAsync(
            It.IsAny<Expression<Func<SalesOrder, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>())).ReturnsAsync(order);

        var request = new UpdateSalesOrderRequest(2, "Updated", "Updated address", null);
        var service = CreateService();

        var result = await service.UpdateSalesOrderAsync(10, request, CancellationToken.None);

        Assert.True(result.IsSuccess);
        _uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateSalesOrderAsync_ReturnsFailure_WhenExistingItemInventoryIsMissing()
    {
        var inventory = CreateInventory(inventoryId: 1, productId: 1, quantityOnHand: 50);
        var order = CreatePendingOrder(inventory);

        _salesOrderRepositoryMock.Setup(r => r.FindAsync(
            It.IsAny<Expression<Func<SalesOrder, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>())).ReturnsAsync(order);

        var request = new UpdateSalesOrderRequest(
            1,
            "Updated",
            "Address",
            new List<AppSalesOrderItemRequest> { new(1, 1, 1) });

        var service = CreateService();

        var result = await service.UpdateSalesOrderAsync(10, request, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Failure, result.Error.Type);
    }

    [Fact]
    public async Task UpdateSalesOrderAsync_ReturnsValidation_WhenMergedItemQuantityIsNotPositive()
    {
        var oldInventory = CreateInventory(inventoryId: 1, productId: 1, quantityOnHand: 50);
        var order = CreatePendingOrder(oldInventory, quantity: 4);
        AttachInventoryToAllOrderItems(order, oldInventory);

        _salesOrderRepositoryMock.Setup(r => r.FindAsync(
            It.IsAny<Expression<Func<SalesOrder, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>())).ReturnsAsync(order);

        var request = new UpdateSalesOrderRequest(
            1,
            "Updated",
            "Address",
            new List<AppSalesOrderItemRequest>
            {
                new(ProductId: 1, InventoryId: 1, Quantity: 0),
            });

        var service = CreateService();

        var result = await service.UpdateSalesOrderAsync(10, request, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Validation, result.Error.Type);
    }

    [Fact]
    public async Task UpdateSalesOrderAsync_ReturnsNotFound_WhenMergedInventoryDoesNotExist()
    {
        var oldInventory = CreateInventory(inventoryId: 1, productId: 1, quantityOnHand: 50);
        var order = CreatePendingOrder(oldInventory, quantity: 4);
        AttachInventoryToAllOrderItems(order, oldInventory);

        _salesOrderRepositoryMock.Setup(r => r.FindAsync(
            It.IsAny<Expression<Func<SalesOrder, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>())).ReturnsAsync(order);

        _inventoryRepositoryMock.Setup(r => r.FindAsync(
            It.IsAny<Expression<Func<Inventory, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>())).ReturnsAsync((Inventory)null!);

        var request = new UpdateSalesOrderRequest(
            1,
            "Updated",
            "Address",
            new List<AppSalesOrderItemRequest>
            {
                new(ProductId: 2, InventoryId: 2, Quantity: 2),
            });

        var service = CreateService();

        var result = await service.UpdateSalesOrderAsync(10, request, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.NotFound, result.Error.Type);
    }

    [Fact]
    public async Task UpdateSalesOrderAsync_ReturnsValidation_WhenMergedInventoryProductDoesNotMatch()
    {
        var oldInventory = CreateInventory(inventoryId: 1, productId: 1, quantityOnHand: 50);
        var order = CreatePendingOrder(oldInventory, quantity: 4);
        AttachInventoryToAllOrderItems(order, oldInventory);

        var newInventory = CreateInventory(inventoryId: 2, productId: 3, quantityOnHand: 50);

        _salesOrderRepositoryMock.Setup(r => r.FindAsync(
            It.IsAny<Expression<Func<SalesOrder, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>())).ReturnsAsync(order);

        _inventoryRepositoryMock.Setup(r => r.FindAsync(
            It.IsAny<Expression<Func<Inventory, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>())).ReturnsAsync(newInventory);

        var request = new UpdateSalesOrderRequest(
            1,
            "Updated",
            "Address",
            new List<AppSalesOrderItemRequest>
            {
                new(ProductId: 2, InventoryId: 2, Quantity: 2),
            });

        var service = CreateService();

        var result = await service.UpdateSalesOrderAsync(10, request, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Validation, result.Error.Type);
    }

    [Fact]
    public async Task UpdateSalesOrderAsync_ReturnsConflict_WhenMergedInventoryStockIsInsufficient()
    {
        var oldInventory = CreateInventory(inventoryId: 1, productId: 1, quantityOnHand: 50);
        var order = CreatePendingOrder(oldInventory, quantity: 4);
        AttachInventoryToAllOrderItems(order, oldInventory);

        var newInventory = CreateInventory(inventoryId: 2, productId: 2, quantityOnHand: 1);

        _salesOrderRepositoryMock.Setup(r => r.FindAsync(
            It.IsAny<Expression<Func<SalesOrder, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>())).ReturnsAsync(order);

        _inventoryRepositoryMock.Setup(r => r.FindAsync(
            It.IsAny<Expression<Func<Inventory, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>())).ReturnsAsync(newInventory);

        var request = new UpdateSalesOrderRequest(
            1,
            "Updated",
            "Address",
            new List<AppSalesOrderItemRequest>
            {
                new(ProductId: 2, InventoryId: 2, Quantity: 2),
            });

        var service = CreateService();

        var result = await service.UpdateSalesOrderAsync(10, request, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Conflict, result.Error.Type);
    }

    [Fact]
    public async Task UpdateSalesOrderAsync_ReturnsConflict_WhenDecreaseStockThrowsDomainException()
    {
        var oldInventory = CreateInventory(inventoryId: 1, productId: 1, quantityOnHand: 50);
        var order = CreatePendingOrder(oldInventory, quantity: 4);
        AttachInventoryToAllOrderItems(order, oldInventory);

        var newInventory = CreateInventory(inventoryId: 2, productId: 2, quantityOnHand: 10);
        newInventory.Product.IsActive = false;

        _salesOrderRepositoryMock.Setup(r => r.FindAsync(
            It.IsAny<Expression<Func<SalesOrder, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>())).ReturnsAsync(order);

        _inventoryRepositoryMock.Setup(r => r.FindAsync(
            It.IsAny<Expression<Func<Inventory, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>())).ReturnsAsync(newInventory);

        var request = new UpdateSalesOrderRequest(
            1,
            "Updated",
            "Address",
            new List<AppSalesOrderItemRequest>
            {
                new(ProductId: 2, InventoryId: 2, Quantity: 2),
            });

        var service = CreateService();

        var result = await service.UpdateSalesOrderAsync(10, request, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Conflict, result.Error.Type);
    }

    [Fact]
    public async Task UpdateSalesOrderAsync_ReturnsFailure_WhenUnexpectedExceptionOccurs()
    {
        var inventory = CreateInventory(inventoryId: 1, productId: 1, quantityOnHand: 50);
        var order = CreatePendingOrder(inventory);

        _salesOrderRepositoryMock.Setup(r => r.FindAsync(
            It.IsAny<Expression<Func<SalesOrder, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>())).ReturnsAsync(order);

        var service = CreateService();

        _uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Database error"));

        var request = new UpdateSalesOrderRequest(1, "Updated", "Address", null);

        var result = await service.UpdateSalesOrderAsync(10, request, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Failure, result.Error.Type);
    }

    [Fact]
    public async Task UpdateSalesOrderAsync_ReturnsSuccess_WhenItemsAreReplacedAndMerged()
    {
        var oldInventory = CreateInventory(inventoryId: 1, productId: 1, quantityOnHand: 50);
        var order = CreatePendingOrder(oldInventory, quantity: 4);
        AttachInventoryToAllOrderItems(order, oldInventory);

        var newInventory = CreateInventory(inventoryId: 2, productId: 2, quantityOnHand: 20, unitPrice: 15m);

        _salesOrderRepositoryMock.Setup(r => r.FindAsync(
            It.IsAny<Expression<Func<SalesOrder, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>())).ReturnsAsync(order);

        _inventoryRepositoryMock.Setup(r => r.FindAsync(
            It.IsAny<Expression<Func<Inventory, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>())).ReturnsAsync(newInventory);

        var request = new UpdateSalesOrderRequest(
            1,
            "Updated",
            "Address",
            new List<AppSalesOrderItemRequest>
            {
                new(ProductId: 2, InventoryId: 2, Quantity: 2),
                new(ProductId: 2, InventoryId: 2, Quantity: 3),
            });

        var service = CreateService();

        var result = await service.UpdateSalesOrderAsync(10, request, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Single(order.Items);
        Assert.Equal(5m, order.Items.Single().OrderedQuantity);
        Assert.Equal(50m, oldInventory.QuantityOnHand);
        Assert.Equal(15m, newInventory.QuantityOnHand);
        _uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
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

    [Fact]
    public async Task ConfirmOrderAsync_ReturnsSuccess_WhenOrderIsPending()
    {
        var inventory = CreateInventory(inventoryId: 1, productId: 1, quantityOnHand: 50);
        var order = CreatePendingOrder(inventory);

        _salesOrderRepositoryMock.Setup(r => r.FindAsync(
            It.IsAny<Expression<Func<SalesOrder, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>())).ReturnsAsync(order);

        var service = CreateService();

        var result = await service.ConfirmOrderAsync(1, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(SalesOrderStatus.Confirmed, order.SalesStatus);
    }

    [Fact]
    public async Task MarkInTransitAsync_ReturnsSuccess_WhenOrderIsConfirmed()
    {
        var inventory = CreateInventory(inventoryId: 1, productId: 1, quantityOnHand: 50);
        var order = CreatePendingOrder(inventory);
        order.Confirm();

        _salesOrderRepositoryMock.Setup(r => r.FindAsync(
            It.IsAny<Expression<Func<SalesOrder, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>())).ReturnsAsync(order);

        var service = CreateService();

        var result = await service.MarkInTransitAsync(1, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(SalesOrderStatus.InTransit, order.SalesStatus);
    }

    [Fact]
    public async Task MarkInTransitAsync_ReturnsConflict_WhenOrderStateIsInvalid()
    {
        var inventory = CreateInventory(inventoryId: 1, productId: 1, quantityOnHand: 50);
        var order = CreatePendingOrder(inventory);

        _salesOrderRepositoryMock.Setup(r => r.FindAsync(
            It.IsAny<Expression<Func<SalesOrder, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>())).ReturnsAsync(order);

        var service = CreateService();

        var result = await service.MarkInTransitAsync(1, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Conflict, result.Error.Type);
    }

    [Fact]
    public async Task ShipOrderAsync_ReturnsSuccess_WhenOrderIsInTransit()
    {
        var inventory = CreateInventory(inventoryId: 1, productId: 1, quantityOnHand: 50);
        var order = CreatePendingOrder(inventory);
        order.Confirm();
        order.MarkInTransit();

        _salesOrderRepositoryMock.Setup(r => r.FindAsync(
            It.IsAny<Expression<Func<SalesOrder, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>())).ReturnsAsync(order);

        var service = CreateService();

        var result = await service.ShipOrderAsync(1, "TRACK-1", CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(SalesOrderStatus.Shipped, order.SalesStatus);
        Assert.Equal("TRACK-1", order.TrackingNumber);
    }

    [Fact]
    public async Task CompleteOrderAsync_ReturnsSuccess_WhenOrderIsShipped()
    {
        var inventory = CreateInventory(inventoryId: 1, productId: 1, quantityOnHand: 50);
        var order = CreatePendingOrder(inventory);
        order.Confirm();
        order.MarkInTransit();
        order.MarkShipped("TRACK-1");

        _salesOrderRepositoryMock.Setup(r => r.FindAsync(
            It.IsAny<Expression<Func<SalesOrder, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>())).ReturnsAsync(order);

        var service = CreateService();

        var result = await service.CompleteOrderAsync(1, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(SalesOrderStatus.Completed, order.SalesStatus);
    }

    [Fact]
    public async Task CancelOrderAsync_ReturnsSuccess_WhenOrderIsPendingAndItemsHaveInventory()
    {
        var inventory = CreateInventory(inventoryId: 1, productId: 1, quantityOnHand: 50);
        var order = CreatePendingOrder(inventory, quantity: 4, orderId: 30);
        AttachInventoryToAllOrderItems(order, inventory);

        _salesOrderRepositoryMock.Setup(r => r.FindAsync(
            It.IsAny<Expression<Func<SalesOrder, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.Is<string>(s => s == "Items,Items.Inventory,Items.Inventory.Product"))).ReturnsAsync(order);

        var service = CreateService();

        var result = await service.CancelOrderAsync(1, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(SalesOrderStatus.Cancelled, order.SalesStatus);
        Assert.Equal(50m, inventory.QuantityOnHand);
        _salesOrderRepositoryMock.Verify(r => r.FindAsync(
            It.IsAny<Expression<Func<SalesOrder, bool>>>(),
            It.IsAny<CancellationToken>(),
            "Items,Items.Inventory,Items.Inventory.Product"), Times.Once);
    }

    [Fact]
    public async Task ReturnOrderAsync_ReturnsSuccess_WhenOrderIsCompleted()
    {
        var inventory = CreateInventory(inventoryId: 1, productId: 1, quantityOnHand: 50);
        var order = CreatePendingOrder(inventory);
        order.Confirm();
        order.MarkInTransit();
        order.MarkShipped("TRACK-1");
        order.Complete();

        _salesOrderRepositoryMock.Setup(r => r.FindAsync(
            It.IsAny<Expression<Func<SalesOrder, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>())).ReturnsAsync(order);

        var service = CreateService();

        var result = await service.ReturnOrderAsync(1, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(SalesOrderStatus.Returned, order.SalesStatus);
    }

    [Fact]
    public async Task ConfirmOrderAsync_ReturnsFailure_WhenSaveChangesThrows()
    {
        var inventory = CreateInventory(inventoryId: 1, productId: 1, quantityOnHand: 50);
        var order = CreatePendingOrder(inventory);

        _salesOrderRepositoryMock.Setup(r => r.FindAsync(
            It.IsAny<Expression<Func<SalesOrder, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>())).ReturnsAsync(order);

        var service = CreateService();

        _uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Persistence failed"));

        var result = await service.ConfirmOrderAsync(1, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Failure, result.Error.Type);
    }
}
