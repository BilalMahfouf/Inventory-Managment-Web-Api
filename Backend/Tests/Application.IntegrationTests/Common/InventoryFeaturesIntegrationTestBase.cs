using Application.Inventories;
using Application.Inventories.DTOs.Request;
using Domain.Inventories.Entities;
using Domain.Products.Entities;
using FluentAssertions;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Application.IntegrationTests.Common;

public abstract class InventoryFeaturesIntegrationTestBase : IClassFixture<IntegrationTestWebAppFactory>, IAsyncLifetime
{
    private const string ProductPrefix = "Inventory-IT-Product-";
    private const string LocationPrefix = "Inventory-IT-Location-";

    private readonly AsyncServiceScope _scope;

    protected InventoryFeaturesIntegrationTestBase(IntegrationTestWebAppFactory factory)
    {
        _scope = factory.Services.CreateAsyncScope();

        InventoryService = _scope.ServiceProvider.GetRequiredService<InventoryService>();
        InventoryQueries = _scope.ServiceProvider.GetRequiredService<IInventoryQueries>();
        AppDbContext = _scope.ServiceProvider.GetRequiredService<InventoryManagmentDBContext>();
    }

    protected InventoryService InventoryService { get; }

    protected IInventoryQueries InventoryQueries { get; }

    protected InventoryManagmentDBContext AppDbContext { get; }

    protected static int TestUserId => IntegrationTestWebAppFactory.SeedUserId;

    public async Task InitializeAsync()
    {
        await CleanupInventoryFeatureDataAsync();
    }

    public async Task DisposeAsync()
    {
        await CleanupInventoryFeatureDataAsync();
        await _scope.DisposeAsync();
    }

    protected async Task AssertBaselineSeedIsAvailableAsync()
    {
        (await AppDbContext.Users.AnyAsync(e => e.Id == TestUserId)).Should().BeTrue();
        (await AppDbContext.ProductCategories.AnyAsync(e => e.Name == IntegrationTestWebAppFactory.DefaultProductCategoryName)).Should().BeTrue();
        (await AppDbContext.UnitOfMeasures.AnyAsync(e => e.Name == IntegrationTestWebAppFactory.DefaultUnitOfMeasureName)).Should().BeTrue();
        (await AppDbContext.Locations.AnyAsync(e => e.Name == IntegrationTestWebAppFactory.DefaultLocationName)).Should().BeTrue();
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
            Sku = sku ?? $"SKU-{token}",
            Name = name ?? $"{ProductPrefix}{token}",
            Description = "Inventory integration test product",
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
            Address = "Inventory integration location",
            IsActive = isActive,
            LocationTypeId = locationTypeId,
            CreatedByUserId = TestUserId,
        };

        AppDbContext.Locations.Add(location);
        await AppDbContext.SaveChangesAsync();

        return location;
    }

    protected async Task<InventoryCreateRequest> BuildValidCreateRequestAsync(
        int? productId = null,
        int? locationId = null,
        decimal quantityOnHand = 10m,
        decimal reorderLevel = 3m,
        decimal maxLevel = 20m)
    {
        var resolvedProductId = productId ?? (await CreateProductAsync()).Id;
        var resolvedLocationId = locationId ?? await GetDefaultLocationIdAsync();

        return new InventoryCreateRequest
        {
            ProductId = resolvedProductId,
            LocationId = resolvedLocationId,
            QuantityOnHand = quantityOnHand,
            ReorderLevel = reorderLevel,
            MaxLevel = maxLevel,
        };
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

    protected async Task<StockTransfer> CreateStockTransferAsync(
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

    protected async Task CleanupInventoryFeatureDataAsync()
    {
        AppDbContext.ChangeTracker.Clear();

        await AppDbContext.StockTransfers
            .IgnoreQueryFilters()
            .ExecuteDeleteAsync();
        await AppDbContext.StockMovements
            .IgnoreQueryFilters()
            .ExecuteDeleteAsync();
        await AppDbContext.Inventories
            .IgnoreQueryFilters()
            .ExecuteDeleteAsync();
        await AppDbContext.Products
            .IgnoreQueryFilters()
            .Where(e => EF.Functions.Like(e.Name, $"{ProductPrefix}%"))
            .ExecuteDeleteAsync();
        await AppDbContext.Locations
            .IgnoreQueryFilters()
            .Where(e => EF.Functions.Like(e.Name, $"{LocationPrefix}%"))
            .ExecuteDeleteAsync();

        await AppDbContext.SaveChangesAsync();
    }
}