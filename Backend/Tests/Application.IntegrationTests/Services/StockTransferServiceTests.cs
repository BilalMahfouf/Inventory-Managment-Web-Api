using Application.IntegrationTests.Common;
using Application.StockMovements.DTOs.Request;
using Domain.Inventories.Enums;
using Domain.Shared.Errors;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Application.IntegrationTests.Services;

public sealed class StockTransferServiceTests : StockMovementFeaturesIntegrationTestBase
{
    public StockTransferServiceTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task TransferStockAsync_ValidRequest_UpdatesInventoriesAndCreatesTransfer()
    {
        await AssertBaselineSeedIsAvailableAsync();
        var product = await CreateProductAsync();
        var fromLocationId = await GetDefaultLocationIdAsync();
        var toLocation = await CreateAdditionalLocationAsync();

        await CreateInventoryDirectAsync(product.Id, fromLocationId, quantityOnHand: 20m, reorderLevel: 2m, maxLevel: 100m);
        await CreateInventoryDirectAsync(product.Id, toLocation.Id, quantityOnHand: 5m, reorderLevel: 2m, maxLevel: 100m);

        var request = new StockTransferRequest
        {
            ProductId = product.Id,
            FromLocationId = fromLocationId,
            ToLocationId = toLocation.Id,
            Quantity = 7m,
        };

        var result = await StockTransferService.TransferStockAsync(request, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeGreaterThan(0);

        AppDbContext.ChangeTracker.Clear();

        var fromInventory = await AppDbContext.Inventories
            .SingleAsync(e => e.ProductId == product.Id && e.LocationId == fromLocationId);

        var toInventory = await AppDbContext.Inventories
            .SingleAsync(e => e.ProductId == product.Id && e.LocationId == toLocation.Id);

        fromInventory.QuantityOnHand.Should().Be(13m);
        toInventory.QuantityOnHand.Should().Be(12m);

        var transfer = await AppDbContext.StockTransfers
            .SingleAsync(e => e.Id == result.Value);

        transfer.ProductId.Should().Be(product.Id);
        transfer.FromLocationId.Should().Be(fromLocationId);
        transfer.ToLocationId.Should().Be(toLocation.Id);
        transfer.Quantity.Should().Be(7m);
        transfer.TransferStatus.Should().Be(TransferStatus.Pending);
    }

    [Fact]
    public async Task TransferStockAsync_SourceInventoryMissing_ReturnsNotFoundFailure()
    {
        var product = await CreateProductAsync();
        var toLocation = await CreateAdditionalLocationAsync();

        await CreateInventoryDirectAsync(product.Id, toLocation.Id, quantityOnHand: 5m, reorderLevel: 2m, maxLevel: 20m);

        var request = new StockTransferRequest
        {
            ProductId = product.Id,
            FromLocationId = await GetDefaultLocationIdAsync(),
            ToLocationId = toLocation.Id,
            Quantity = 1m,
        };

        var result = await StockTransferService.TransferStockAsync(request, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public async Task TransferStockAsync_DestinationInventoryMissing_ReturnsNotFoundFailure()
    {
        var product = await CreateProductAsync();
        var fromLocationId = await GetDefaultLocationIdAsync();
        var toLocation = await CreateAdditionalLocationAsync();

        await CreateInventoryDirectAsync(product.Id, fromLocationId, quantityOnHand: 8m, reorderLevel: 1m, maxLevel: 30m);

        var request = new StockTransferRequest
        {
            ProductId = product.Id,
            FromLocationId = fromLocationId,
            ToLocationId = toLocation.Id,
            Quantity = 2m,
        };

        var result = await StockTransferService.TransferStockAsync(request, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public async Task TransferStockAsync_ZeroQuantity_ReturnsConflictFailure()
    {
        var product = await CreateProductAsync();
        var fromLocationId = await GetDefaultLocationIdAsync();
        var toLocation = await CreateAdditionalLocationAsync();

        await CreateInventoryDirectAsync(product.Id, fromLocationId, quantityOnHand: 10m, reorderLevel: 1m, maxLevel: 50m);
        await CreateInventoryDirectAsync(product.Id, toLocation.Id, quantityOnHand: 10m, reorderLevel: 1m, maxLevel: 50m);

        var request = new StockTransferRequest
        {
            ProductId = product.Id,
            FromLocationId = fromLocationId,
            ToLocationId = toLocation.Id,
            Quantity = 0m,
        };

        var result = await StockTransferService.TransferStockAsync(request, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorType.Conflict);
    }

    [Fact]
    public async Task TransferStockAsync_DestinationMaxLevelExceeded_ReturnsConflictAndDoesNotPersistChanges()
    {
        var product = await CreateProductAsync();
        var fromLocationId = await GetDefaultLocationIdAsync();
        var toLocation = await CreateAdditionalLocationAsync();

        await CreateInventoryDirectAsync(product.Id, fromLocationId, quantityOnHand: 15m, reorderLevel: 2m, maxLevel: 100m);
        await CreateInventoryDirectAsync(product.Id, toLocation.Id, quantityOnHand: 9m, reorderLevel: 2m, maxLevel: 10m);

        var request = new StockTransferRequest
        {
            ProductId = product.Id,
            FromLocationId = fromLocationId,
            ToLocationId = toLocation.Id,
            Quantity = 2m,
        };

        var result = await StockTransferService.TransferStockAsync(request, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorType.Conflict);

        AppDbContext.ChangeTracker.Clear();

        var fromInventory = await AppDbContext.Inventories
            .SingleAsync(e => e.ProductId == product.Id && e.LocationId == fromLocationId);

        var toInventory = await AppDbContext.Inventories
            .SingleAsync(e => e.ProductId == product.Id && e.LocationId == toLocation.Id);

        fromInventory.QuantityOnHand.Should().Be(15m);
        toInventory.QuantityOnHand.Should().Be(9m);

        (await AppDbContext.StockTransfers.AnyAsync(e => e.ProductId == product.Id && e.FromLocationId == fromLocationId && e.ToLocationId == toLocation.Id))
            .Should()
            .BeFalse();
    }

    [Fact]
    public async Task TransferStockAsync_DecimalQuantityBoundary_TransfersFractionalStock()
    {
        const decimal transferQuantity = 0.25m;

        var product = await CreateProductAsync();
        var fromLocationId = await GetDefaultLocationIdAsync();
        var toLocation = await CreateAdditionalLocationAsync();

        await CreateInventoryDirectAsync(product.Id, fromLocationId, quantityOnHand: 1.5m, reorderLevel: 0.1m, maxLevel: 100m);
        await CreateInventoryDirectAsync(product.Id, toLocation.Id, quantityOnHand: 0.5m, reorderLevel: 0.1m, maxLevel: 100m);

        var request = new StockTransferRequest
        {
            ProductId = product.Id,
            FromLocationId = fromLocationId,
            ToLocationId = toLocation.Id,
            Quantity = transferQuantity,
        };

        var result = await StockTransferService.TransferStockAsync(request, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();

        AppDbContext.ChangeTracker.Clear();

        var fromInventory = await AppDbContext.Inventories
            .SingleAsync(e => e.ProductId == product.Id && e.LocationId == fromLocationId);

        var toInventory = await AppDbContext.Inventories
            .SingleAsync(e => e.ProductId == product.Id && e.LocationId == toLocation.Id);

        fromInventory.QuantityOnHand.Should().Be(1.25m);
        toInventory.QuantityOnHand.Should().Be(0.75m);
    }
}
