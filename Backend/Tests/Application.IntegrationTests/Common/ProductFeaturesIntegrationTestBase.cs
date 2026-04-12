using Application.Products.Contracts;
using Application.Products.DTOs.Request.Products;
using Domain.Inventories.Entities;
using Domain.Products.Entities;
using Domain.Products.Enums;
using Domain.Suppliers.Entities;
using FluentAssertions;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Application.IntegrationTests.Common;

public abstract class ProductFeaturesIntegrationTestBase : IClassFixture<IntegrationTestWebAppFactory>, IAsyncLifetime
{
    private readonly AsyncServiceScope _scope;

    protected ProductFeaturesIntegrationTestBase(IntegrationTestWebAppFactory factory)
    {
        _scope = factory.Services.CreateAsyncScope();

        ProductService = _scope.ServiceProvider.GetRequiredService<IProductService>();
        ProductQueries = _scope.ServiceProvider.GetRequiredService<IProductQueries>();
        ProductCategoryService = _scope.ServiceProvider.GetRequiredService<IProductCategoryService>();
        ProductCategoryQueries = _scope.ServiceProvider.GetRequiredService<IProductCategoryQueries>();
        AppDbContext = _scope.ServiceProvider.GetRequiredService<InventoryManagmentDBContext>();
    }

    protected IProductService ProductService { get; }

    protected IProductQueries ProductQueries { get; }

    protected IProductCategoryService ProductCategoryService { get; }

    protected IProductCategoryQueries ProductCategoryQueries { get; }

    protected InventoryManagmentDBContext AppDbContext { get; }

    protected static int TestUserId => IntegrationTestWebAppFactory.SeedUserId;

    public async Task InitializeAsync()
    {
        await CleanupProductFeatureDataAsync();
    }

    public async Task DisposeAsync()
    {
        await CleanupProductFeatureDataAsync();
        await _scope.DisposeAsync();
    }

    protected async Task AssertBaselineSeedIsAvailableAsync()
    {
        (await AppDbContext.Users.AnyAsync(e => e.Id == TestUserId)).Should().BeTrue();
        (await AppDbContext.ProductCategories.AnyAsync(e => e.Name == IntegrationTestWebAppFactory.DefaultProductCategoryName)).Should().BeTrue();
        (await AppDbContext.UnitOfMeasures.AnyAsync(e => e.Name == IntegrationTestWebAppFactory.DefaultUnitOfMeasureName)).Should().BeTrue();
        (await AppDbContext.Locations.AnyAsync(e => e.Name == IntegrationTestWebAppFactory.DefaultLocationName)).Should().BeTrue();
    }

    protected async Task<ProductCreateRequest> BuildValidProductCreateRequestAsync(
        string? sku = null,
        string? name = null,
        decimal unitPrice = 20m,
        decimal costPrice = 15m,
        decimal quantityOnHand = 12m,
        decimal reorderLevel = 3m,
        decimal maxLevel = 50m)
    {
        var categoryId = await GetDefaultCategoryIdAsync();
        var unitOfMeasureId = await GetDefaultUnitOfMeasureIdAsync();
        var locationId = await GetDefaultLocationIdAsync();

        var token = Guid.NewGuid().ToString("N")[..10];

        return new ProductCreateRequest
        {
            SKU = sku ?? $"SKU-{token}",
            Name = name ?? $"Product-{token}",
            Description = "Integration test product",
            CategoryId = categoryId,
            UnitOfMeasureId = unitOfMeasureId,
            UnitPrice = unitPrice,
            CostPrice = costPrice,
            LocationId = locationId,
            QuantityOnHand = quantityOnHand,
            ReorderLevel = reorderLevel,
            MaxLevel = maxLevel,
        };
    }

    protected async Task<(Product Product, Inventory Inventory)> CreateProductInventoryAsync(
        decimal quantityOnHand = 25m,
        decimal unitPrice = 10m,
        decimal costPrice = 7m,
        bool isActive = true)
    {
        var categoryId = await GetDefaultCategoryIdAsync();
        var unitOfMeasureId = await GetDefaultUnitOfMeasureIdAsync();
        var locationId = await GetDefaultLocationIdAsync();
        var token = Guid.NewGuid().ToString("N")[..10];

        var product = new Product
        {
            Sku = $"SKU-{token}",
            Name = $"Product-{token}",
            Description = "Direct product for integration tests",
            CategoryId = categoryId,
            UnitOfMeasureId = unitOfMeasureId,
            UnitPrice = unitPrice,
            Cost = costPrice,
            IsActive = isActive,
            CreatedByUserId = TestUserId,
        };

        AppDbContext.Products.Add(product);
        await AppDbContext.SaveChangesAsync();

        var inventory = new Inventory
        {
            ProductId = product.Id,
            LocationId = locationId,
            QuantityOnHand = quantityOnHand,
            ReorderLevel = 5m,
            MaxLevel = Math.Max(quantityOnHand + 20m, 100m),
            CreatedByUserId = TestUserId,
        };

        AppDbContext.Inventories.Add(inventory);
        await AppDbContext.SaveChangesAsync();

        return (product, inventory);
    }

    protected async Task<ProductCategory> CreateCategoryAsync(
        string? name = null,
        string? description = "Integration category",
        int? parentId = null,
        ProductCategoryType type = ProductCategoryType.MainCategory)
    {
        var category = new ProductCategory
        {
            Name = name ?? $"Category-{Guid.NewGuid().ToString("N")[..8]}",
            Description = description,
            ParentId = parentId,
            Type = type,
            CreatedByUserId = TestUserId,
        };

        AppDbContext.ProductCategories.Add(category);
        await AppDbContext.SaveChangesAsync();

        return category;
    }

    protected async Task<ProductSupplier> CreateProductSupplierAsync(int productId)
    {
        var token = Guid.NewGuid().ToString("N")[..8];

        var supplierType = new SupplierType
        {
            Name = $"SupplierType-{token}",
            Description = "Integration supplier type",
            IsIndividual = false,
            CreatedByUserId = TestUserId,
        };

        AppDbContext.SupplierTypes.Add(supplierType);
        await AppDbContext.SaveChangesAsync();

        var supplier = new Supplier
        {
            Name = $"Supplier-{token}",
            SupplierTypeId = supplierType.Id,
            Email = $"supplier-{token}@ims.local",
            Phone = "01000000000",
            Address = "Supplier Address",
            Terms = "Net 30",
            IsActive = true,
            CreatedByUserId = TestUserId,
        };

        AppDbContext.Suppliers.Add(supplier);
        await AppDbContext.SaveChangesAsync();

        var productSupplier = new ProductSupplier
        {
            ProductId = productId,
            SupplierId = supplier.Id,
            SupplierProductCode = $"SUP-{token}",
            LeadTimeDays = 4,
            MinOrderQuantity = 5,
            CreatedByUserId = TestUserId,
        };

        AppDbContext.ProductSuppliers.Add(productSupplier);
        await AppDbContext.SaveChangesAsync();

        return productSupplier;
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

    protected async Task CleanupProductFeatureDataAsync()
    {
        AppDbContext.ChangeTracker.Clear();

        await AppDbContext.ProductSuppliers.ExecuteDeleteAsync();
        await AppDbContext.ProductImages.ExecuteDeleteAsync();
        await AppDbContext.StockMovements.ExecuteDeleteAsync();
        await AppDbContext.Inventories.ExecuteDeleteAsync();
        await AppDbContext.Products.ExecuteDeleteAsync();
        await AppDbContext.Suppliers.ExecuteDeleteAsync();
        await AppDbContext.SupplierTypes.ExecuteDeleteAsync();
        await AppDbContext.ProductCategories
            .Where(e => e.Name != IntegrationTestWebAppFactory.DefaultProductCategoryName)
            .ExecuteDeleteAsync();

        await AppDbContext.SaveChangesAsync();
    }
}