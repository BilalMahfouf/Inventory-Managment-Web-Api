using Application.Inventories;
using Application.StockMovements.Contracts;
using Application.StockMovements.DTOs.Request;
using Application.StockMovements.Services;
using Domain.Inventories.Entities;
using Domain.Inventories.Enums;
using Domain.Products.Entities;
using FluentAssertions;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Application.IntegrationTests.Common;

public abstract class StockMovementFeaturesIntegrationTestBase : IClassFixture<IntegrationTestWebAppFactory>, IAsyncLifetime
{
    private const string ProductPrefix = "StockTransfer-IT-Product-";
    private const string LocationPrefix = "StockTransfer-IT-Location-";
    private const string StockMovementTypePrefix = "StockMovementType-IT-";

    private readonly AsyncServiceScope _scope;

    protected StockMovementFeaturesIntegrationTestBase(IntegrationTestWebAppFactory factory)
    {
        _scope = factory.Services.CreateAsyncScope();

        StockMovementTypeService = _scope.ServiceProvider.GetRequiredService<StockMovementTypeService>();
        StockTransferService = _scope.ServiceProvider.GetRequiredService<StockTransferService>();
        TransferQueries = _scope.ServiceProvider.GetRequiredService<ITransferQueries>();
        InventoryQueries = _scope.ServiceProvider.GetRequiredService<IInventoryQueries>();
        AppDbContext = _scope.ServiceProvider.GetRequiredService<InventoryManagmentDBContext>();
    }

    protected StockMovementTypeService StockMovementTypeService { get; }

    protected StockTransferService StockTransferService { get; }

    protected ITransferQueries TransferQueries { get; }

    protected IInventoryQueries InventoryQueries { get; }

    protected InventoryManagmentDBContext AppDbContext { get; }

    protected static int TestUserId => IntegrationTestWebAppFactory.SeedUserId;

    public async Task InitializeAsync()
    {
        await CleanupStockMovementFeatureDataAsync();
    }

    public async Task DisposeAsync()
    {
        await CleanupStockMovementFeatureDataAsync();
        await _scope.DisposeAsync();
    }

    protected async Task AssertBaselineSeedIsAvailableAsync()
    {
        (await AppDbContext.Users.AnyAsync(e => e.Id == TestUserId)).Should().BeTrue();
        (await AppDbContext.ProductCategories.AnyAsync(e => e.Name == IntegrationTestWebAppFactory.DefaultProductCategoryName)).Should().BeTrue();
        (await AppDbContext.UnitOfMeasures.AnyAsync(e => e.Name == IntegrationTestWebAppFactory.DefaultUnitOfMeasureName)).Should().BeTrue();
        (await AppDbContext.Locations.AnyAsync(e => e.Name == IntegrationTestWebAppFactory.DefaultLocationName)).Should().BeTrue();
    }

    protected string GenerateStockMovementTypeName()
    {
        return $"{StockMovementTypePrefix}{Guid.NewGuid().ToString("N")[..8]}";
    }

    protected StockMovementTypeRequest BuildValidStockMovementTypeRequest(
        string? name = null,
        string? description = null,
        byte direction = (byte)StockMovementDirection.In)
    {
        return new StockMovementTypeRequest
        {
            Name = name ?? GenerateStockMovementTypeName(),
            Description = description ?? "Stock movement type for integration testing",
            Direction = direction,
        };
    }

    protected async Task<StockMovementType> CreateStockMovementTypeDirectAsync(
        string? name = null,
        string? description = "Seeded by stock movement integration test",
        StockMovementDirection direction = StockMovementDirection.In)
    {
        var type = new StockMovementType
        {
            Name = name ?? GenerateStockMovementTypeName(),
            Description = description,
            Direction = direction,
            CreatedByUserId = TestUserId,
        };

        AppDbContext.StockMovementTypes.Add(type);
        await AppDbContext.SaveChangesAsync();

        return type;
    }

    protected async Task<int> GetDefaultCategoryIdAsync()
    {
        return await AppDbContext.ProductCategories
            .Where(e => e.Name == IntegrationTestWebAppFactory.DefaultProductCategoryName)
            .Select(e => e.Id)
            .SingleAsync();
    }

    protected async Task<int> GetDefaultUnitOfMeasureIdAsync()
    {
        return await AppDbContext.UnitOfMeasures
            .Where(e => e.Name == IntegrationTestWebAppFactory.DefaultUnitOfMeasureName)
            .Select(e => e.Id)
            .SingleAsync();
    }

    protected async Task<int> GetDefaultLocationIdAsync()
    {
        return await AppDbContext.Locations
            .Where(e => e.Name == IntegrationTestWebAppFactory.DefaultLocationName)
            .Select(e => e.Id)
            .SingleAsync();
    }

    protected async Task<Product> CreateProductAsync(
        bool isActive = true,
        decimal unitPrice = 10m,
        decimal cost = 8m,
        string? sku = null,
        string? name = null)
    {
        var token = Guid.NewGuid().ToString("N")[..10];
        var categoryId = await GetDefaultCategoryIdAsync();
        var unitOfMeasureId = await GetDefaultUnitOfMeasureIdAsync();

        var product = new Product
        {
            Sku = sku ?? $"STM-{token}",
            Name = name ?? $"{ProductPrefix}{token}",
            Description = "Stock movement integration test product",
            CategoryId = categoryId,
            UnitOfMeasureId = unitOfMeasureId,
            UnitPrice = unitPrice,
            Cost = cost,
            IsActive = isActive,
            CreatedByUserId = TestUserId,
        };

        AppDbContext.Products.Add(product);
        await AppDbContext.SaveChangesAsync();

        return product;
    }

    protected async Task<Location> CreateAdditionalLocationAsync(bool isActive = true)
    {
        var token = Guid.NewGuid().ToString("N")[..10];

        var locationTypeId = await AppDbContext.LocationTypes
            .Where(e => e.Name == IntegrationTestWebAppFactory.DefaultLocationTypeName)
            .Select(e => e.Id)
            .SingleAsync();

        var location = new Location
        {
            Name = $"{LocationPrefix}{token}",
            Address = "Stock movement integration location",
            IsActive = isActive,
            LocationTypeId = locationTypeId,
            CreatedByUserId = TestUserId,
        };

        AppDbContext.Locations.Add(location);
        await AppDbContext.SaveChangesAsync();

        return location;
    }

    protected async Task<Inventory> CreateInventoryDirectAsync(
        int productId,
        int locationId,
        decimal quantityOnHand,
        decimal reorderLevel,
        decimal maxLevel)
    {
        var inventory = new Inventory
        {
            ProductId = productId,
            LocationId = locationId,
            QuantityOnHand = quantityOnHand,
            ReorderLevel = reorderLevel,
            MaxLevel = maxLevel,
            CreatedByUserId = TestUserId,
        };

        AppDbContext.Inventories.Add(inventory);
        await AppDbContext.SaveChangesAsync();

        return inventory;
    }

    protected async Task<StockTransfer> CreateStockTransferDirectAsync(
        int productId,
        int fromLocationId,
        int toLocationId,
        decimal quantity)
    {
        var transfer = StockTransfer.Create(productId, fromLocationId, toLocationId, quantity);
        transfer.CreatedByUserId = TestUserId;

        AppDbContext.StockTransfers.Add(transfer);
        await AppDbContext.SaveChangesAsync();

        return transfer;
    }

    protected async Task CleanupStockMovementFeatureDataAsync()
    {
        AppDbContext.ChangeTracker.Clear();

        var testProductIds = await AppDbContext.Products
            .IgnoreQueryFilters()
            .Where(e => EF.Functions.Like(e.Name, $"{ProductPrefix}%"))
            .Select(e => e.Id)
            .ToListAsync();

        var testLocationIds = await AppDbContext.Locations
            .IgnoreQueryFilters()
            .Where(e => EF.Functions.Like(e.Name, $"{LocationPrefix}%"))
            .Select(e => e.Id)
            .ToListAsync();

        var transferIds = await AppDbContext.StockTransfers
            .IgnoreQueryFilters()
            .Where(e => testProductIds.Contains(e.ProductId)
                || testLocationIds.Contains(e.FromLocationId)
                || testLocationIds.Contains(e.ToLocationId))
            .Select(e => e.Id)
            .ToListAsync();

        if (transferIds.Count > 0)
        {
            await AppDbContext.StockTransfers
                .IgnoreQueryFilters()
                .Where(e => transferIds.Contains(e.Id))
                .ExecuteDeleteAsync();
        }

        var inventoryIds = await AppDbContext.Inventories
            .IgnoreQueryFilters()
            .Where(e => testProductIds.Contains(e.ProductId)
                || testLocationIds.Contains(e.LocationId))
            .Select(e => e.Id)
            .ToListAsync();

        if (inventoryIds.Count > 0)
        {
            await AppDbContext.StockMovements
                .IgnoreQueryFilters()
                .Where(e => inventoryIds.Contains(e.InventoryId))
                .ExecuteDeleteAsync();

            await AppDbContext.Inventories
                .IgnoreQueryFilters()
                .Where(e => inventoryIds.Contains(e.Id))
                .ExecuteDeleteAsync();
        }

        if (testProductIds.Count > 0)
        {
            await AppDbContext.Products
                .IgnoreQueryFilters()
                .Where(e => testProductIds.Contains(e.Id))
                .ExecuteDeleteAsync();
        }

        if (testLocationIds.Count > 0)
        {
            await AppDbContext.Locations
                .IgnoreQueryFilters()
                .Where(e => testLocationIds.Contains(e.Id))
                .ExecuteDeleteAsync();
        }

        await AppDbContext.StockMovementTypes
            .IgnoreQueryFilters()
            .Where(e => EF.Functions.Like(e.Name, $"{StockMovementTypePrefix}%"))
            .ExecuteDeleteAsync();

        await AppDbContext.SaveChangesAsync();
    }
}
