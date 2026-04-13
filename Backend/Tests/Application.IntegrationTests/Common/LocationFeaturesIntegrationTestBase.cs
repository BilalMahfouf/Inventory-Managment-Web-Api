using Application.Locations.DTOs.Request;
using Application.Locations.Services;
using Domain.Inventories.Entities;
using Domain.Products.Entities;
using FluentAssertions;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Application.IntegrationTests.Common;

public abstract class LocationFeaturesIntegrationTestBase : IClassFixture<IntegrationTestWebAppFactory>, IAsyncLifetime
{
    private const string LocationTypePrefix = "LocationType-IT-";
    private const string LocationPrefix = "Location-IT-";
    private const string ProductPrefix = "LocationProduct-IT-";

    private readonly AsyncServiceScope _scope;

    protected LocationFeaturesIntegrationTestBase(IntegrationTestWebAppFactory factory)
    {
        _scope = factory.Services.CreateAsyncScope();

        LocationService = _scope.ServiceProvider.GetRequiredService<LocationService>();
        LocationTypeService = _scope.ServiceProvider.GetRequiredService<LocationTypeService>();
        AppDbContext = _scope.ServiceProvider.GetRequiredService<InventoryManagmentDBContext>();
    }

    protected LocationService LocationService { get; }

    protected LocationTypeService LocationTypeService { get; }

    protected InventoryManagmentDBContext AppDbContext { get; }

    protected static int TestUserId => IntegrationTestWebAppFactory.SeedUserId;

    public async Task InitializeAsync()
    {
        await CleanupLocationFeatureDataAsync();
    }

    public async Task DisposeAsync()
    {
        await CleanupLocationFeatureDataAsync();
        await _scope.DisposeAsync();
    }

    protected async Task AssertBaselineSeedIsAvailableAsync()
    {
        (await AppDbContext.Users.AnyAsync(e => e.Id == TestUserId)).Should().BeTrue();
        (await AppDbContext.LocationTypes.AnyAsync(e => e.Name == IntegrationTestWebAppFactory.DefaultLocationTypeName)).Should().BeTrue();
        (await AppDbContext.Locations.AnyAsync(e => e.Name == IntegrationTestWebAppFactory.DefaultLocationName)).Should().BeTrue();
        (await AppDbContext.ProductCategories.AnyAsync(e => e.Name == IntegrationTestWebAppFactory.DefaultProductCategoryName)).Should().BeTrue();
        (await AppDbContext.UnitOfMeasures.AnyAsync(e => e.Name == IntegrationTestWebAppFactory.DefaultUnitOfMeasureName)).Should().BeTrue();
    }

    protected LocationTypeCreateRequest BuildValidLocationTypeCreateRequest(string? name = null)
    {
        var resolvedName = EnsurePrefixed(name, LocationTypePrefix);

        return new LocationTypeCreateRequest
        {
            Name = resolvedName,
            Description = "Location type integration test",
        };
    }

    protected LocationCreateRequest BuildValidLocationCreateRequest(int locationTypeId, string? name = null)
    {
        var resolvedName = EnsurePrefixed(name, LocationPrefix);

        return new LocationCreateRequest
        {
            Name = resolvedName,
            Address = "Location integration test address",
            LocationTypeId = locationTypeId,
        };
    }

    protected async Task<int> GetDefaultLocationTypeIdAsync()
    {
        return await AppDbContext.LocationTypes
            .Where(e => e.Name == IntegrationTestWebAppFactory.DefaultLocationTypeName)
            .Select(e => e.Id)
            .SingleAsync();
    }

    protected async Task<LocationType> CreateLocationTypeDirectAsync(string? name = null)
    {
        var resolvedName = EnsurePrefixed(name, LocationTypePrefix);

        var locationType = new LocationType
        {
            Name = resolvedName,
            Description = "Direct location type for integration tests",
            CreatedByUserId = TestUserId,
        };

        AppDbContext.LocationTypes.Add(locationType);
        await AppDbContext.SaveChangesAsync();

        return locationType;
    }

    protected async Task<Location> CreateLocationDirectAsync(
        int? locationTypeId = null,
        bool isActive = true,
        string? name = null)
    {
        var resolvedLocationTypeId = locationTypeId ?? await GetDefaultLocationTypeIdAsync();
        var resolvedName = EnsurePrefixed(name, LocationPrefix);

        var location = new Location
        {
            Name = resolvedName,
            Address = "Direct location for integration tests",
            IsActive = isActive,
            LocationTypeId = resolvedLocationTypeId,
            CreatedByUserId = TestUserId,
        };

        AppDbContext.Locations.Add(location);
        await AppDbContext.SaveChangesAsync();

        return location;
    }

    protected async Task<Product> CreateProductDirectAsync(
        bool isActive = true,
        string? sku = null,
        string? name = null,
        decimal unitPrice = 10m,
        decimal cost = 8m)
    {
        var categoryId = await AppDbContext.ProductCategories
            .Where(e => e.Name == IntegrationTestWebAppFactory.DefaultProductCategoryName)
            .Select(e => e.Id)
            .SingleAsync();

        var unitOfMeasureId = await AppDbContext.UnitOfMeasures
            .Where(e => e.Name == IntegrationTestWebAppFactory.DefaultUnitOfMeasureName)
            .Select(e => e.Id)
            .SingleAsync();

        var token = Guid.NewGuid().ToString("N")[..10];
        var resolvedName = EnsurePrefixed(name, ProductPrefix);

        var product = new Product
        {
            Sku = sku ?? $"SKU-{token}",
            Name = resolvedName,
            Description = "Location feature integration product",
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

    protected async Task<Inventory> CreateInventoryDirectAsync(
        int productId,
        int locationId,
        decimal quantityOnHand = 10m,
        decimal reorderLevel = 2m,
        decimal maxLevel = 20m)
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

    protected async Task CleanupLocationFeatureDataAsync()
    {
        AppDbContext.ChangeTracker.Clear();

        var locationTypeIds = await AppDbContext.LocationTypes
            .IgnoreQueryFilters()
            .Where(e => EF.Functions.Like(e.Name, $"{LocationTypePrefix}%"))
            .Select(e => e.Id)
            .ToListAsync();

        var locationIds = await AppDbContext.Locations
            .IgnoreQueryFilters()
            .Where(e => EF.Functions.Like(e.Name, $"{LocationPrefix}%"))
            .Select(e => e.Id)
            .ToListAsync();

        if (locationTypeIds.Count > 0)
        {
            var locationIdsByType = await AppDbContext.Locations
                .IgnoreQueryFilters()
                .Where(e => locationTypeIds.Contains(e.LocationTypeId))
                .Select(e => e.Id)
                .ToListAsync();

            locationIds = locationIds
                .Union(locationIdsByType)
                .ToList();
        }

        var productIds = await AppDbContext.Products
            .IgnoreQueryFilters()
            .Where(e => EF.Functions.Like(e.Name, $"{ProductPrefix}%"))
            .Select(e => e.Id)
            .ToListAsync();

        if (locationIds.Count > 0 || productIds.Count > 0)
        {
            var inventoryIds = await AppDbContext.Inventories
                .IgnoreQueryFilters()
                .Where(e => locationIds.Contains(e.LocationId) || productIds.Contains(e.ProductId))
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
        }

        if (productIds.Count > 0)
        {
            await AppDbContext.Products
                .IgnoreQueryFilters()
                .Where(e => productIds.Contains(e.Id))
                .ExecuteDeleteAsync();
        }

        if (locationIds.Count > 0)
        {
            await AppDbContext.Locations
                .IgnoreQueryFilters()
                .Where(e => locationIds.Contains(e.Id))
                .ExecuteDeleteAsync();
        }

        if (locationTypeIds.Count > 0)
        {
            await AppDbContext.LocationTypes
                .IgnoreQueryFilters()
                .Where(e => locationTypeIds.Contains(e.Id))
                .ExecuteDeleteAsync();
        }

        await AppDbContext.SaveChangesAsync();
    }

    private static string EnsurePrefixed(string? candidate, string prefix)
    {
        if (string.IsNullOrWhiteSpace(candidate))
        {
            return $"{prefix}{Guid.NewGuid().ToString("N")[..10]}";
        }

        return candidate.StartsWith(prefix, StringComparison.Ordinal)
            ? candidate
            : $"{prefix}{candidate}";
    }
}
