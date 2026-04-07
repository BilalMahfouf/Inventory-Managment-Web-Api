using Application.Sales.Queries;
using Application.Sales.Services;
using Domain.Customers.Entities;
using Domain.Inventories.Entities;
using Domain.Products.Entities;
using Domain.Shared.ValueObjects;
using FluentAssertions;
using Infrastructure.Outbox;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Application.IntegrationTests.Common;

public abstract class BaseIntegrationTest : IClassFixture<IntegrationTestWebAppFactory>, IAsyncLifetime
{
    private readonly AsyncServiceScope _scope;

    protected BaseIntegrationTest(IntegrationTestWebAppFactory factory)
    {
        _scope = factory.Services.CreateAsyncScope();
        SalesOrderService = _scope.ServiceProvider.GetRequiredService<SalesOrderService>();
        SalesOrderQueries = _scope.ServiceProvider.GetRequiredService<ISalesOrderQueries>();
        AppDbContext = _scope.ServiceProvider.GetRequiredService<InventoryManagmentDBContext>();
    }

    protected SalesOrderService SalesOrderService { get; }

    protected ISalesOrderQueries SalesOrderQueries { get; }

    protected InventoryManagmentDBContext AppDbContext { get; }

    protected static int TestUserId => IntegrationTestWebAppFactory.SeedUserId;

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync()
    {
        await CleanupSalesOrderDataAsync();
        await _scope.DisposeAsync();
    }

    protected async Task<Customer> CreateCustomerAsync(string? suffix = null)
    {
        var token = suffix ?? Guid.NewGuid().ToString("N")[..8];

        var customer = Customer.Create(
            name: $"Customer-{token}",
            customerCategoryId: null,
            email: $"customer-{token}@ims.local",
            phone: "01000000000",
            address: Address.Create("Street", "City", "State", "12345"));

        customer.CreatedByUserId = TestUserId;

        AppDbContext.Customers.Add(customer);
        await AppDbContext.SaveChangesAsync();

        return customer;
    }

    protected async Task<(Product Product, Inventory Inventory)> CreateProductInventoryAsync(
        decimal quantityOnHand = 100m,
        decimal unitPrice = 10m,
        bool isActive = true)
    {
        var categoryId = await AppDbContext.ProductCategories
            .Where(e => e.Name == IntegrationTestWebAppFactory.DefaultProductCategoryName)
            .Select(e => e.Id)
            .SingleAsync();

        var unitOfMeasureId = await AppDbContext.UnitOfMeasures
            .Where(e => e.Name == IntegrationTestWebAppFactory.DefaultUnitOfMeasureName)
            .Select(e => e.Id)
            .SingleAsync();

        var locationId = await AppDbContext.Locations
            .Where(e => e.Name == IntegrationTestWebAppFactory.DefaultLocationName)
            .Select(e => e.Id)
            .SingleAsync();

        var token = Guid.NewGuid().ToString("N")[..12];

        var product = new Product
        {
            Sku = $"SKU-{token}",
            Name = $"Product-{token}",
            Description = "Integration test product",
            CategoryId = categoryId,
            UnitOfMeasureId = unitOfMeasureId,
            UnitPrice = unitPrice,
            Cost = unitPrice <= 1m ? 0.5m : unitPrice - 1m,
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
            ReorderLevel = 1m,
            MaxLevel = Math.Max(quantityOnHand + 100m, 1000m),
            CreatedByUserId = TestUserId,
        };

        AppDbContext.Inventories.Add(inventory);
        await AppDbContext.SaveChangesAsync();

        return (product, inventory);
    }

    private async Task CleanupSalesOrderDataAsync()
    {
        AppDbContext.ChangeTracker.Clear();

        await AppDbContext.SalesOrderItems.ExecuteDeleteAsync();
        await AppDbContext.SalesOrders.ExecuteDeleteAsync();
        await AppDbContext.Set<OutboxMessages>()
            .Where(e => EF.Functions.Like(e.Name, "SalesOrder%"))
            .ExecuteDeleteAsync();

        await AppDbContext.SaveChangesAsync();
    }

    protected async Task AssertBaselineSeedIsAvailableAsync()
    {
        (await AppDbContext.Users.AnyAsync(e => e.Id == TestUserId)).Should().BeTrue();
        (await AppDbContext.ProductCategories.AnyAsync(e => e.Name == IntegrationTestWebAppFactory.DefaultProductCategoryName)).Should().BeTrue();
        (await AppDbContext.UnitOfMeasures.AnyAsync(e => e.Name == IntegrationTestWebAppFactory.DefaultUnitOfMeasureName)).Should().BeTrue();
        (await AppDbContext.Locations.AnyAsync(e => e.Name == IntegrationTestWebAppFactory.DefaultLocationName)).Should().BeTrue();
    }
}
