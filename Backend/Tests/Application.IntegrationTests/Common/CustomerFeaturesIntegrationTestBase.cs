using Application.Customers;
using Application.Customers.Dtos;
using Application.Sales.RequestResponse;
using Application.Sales.Services;
using Domain.Customers.Entities;
using Domain.Inventories.Entities;
using Domain.Products.Entities;
using Domain.Sales.Enums;
using Domain.Shared.ValueObjects;
using FluentAssertions;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Application.IntegrationTests.Common;

public abstract class CustomerFeaturesIntegrationTestBase : IClassFixture<IntegrationTestWebAppFactory>, IAsyncLifetime
{
    private const string CustomerPrefix = "Customer-IT-";
    private const string CategoryPrefix = "CustomerCategory-IT-";
    private const string ProductPrefix = "CustomerOrderProduct-IT-";
    private const string OrderDescriptionPrefix = "CustomerOrder-IT-";

    private readonly AsyncServiceScope _scope;

    protected CustomerFeaturesIntegrationTestBase(IntegrationTestWebAppFactory factory)
    {
        _scope = factory.Services.CreateAsyncScope();

        CustomerService = _scope.ServiceProvider.GetRequiredService<CustomerService>();
        CustomerQueries = _scope.ServiceProvider.GetRequiredService<ICustomerQueries>();
        CustomerCategoryService = _scope.ServiceProvider.GetRequiredService<CustomerCategoryService>();
        SalesOrderService = _scope.ServiceProvider.GetRequiredService<SalesOrderService>();
        AppDbContext = _scope.ServiceProvider.GetRequiredService<InventoryManagmentDBContext>();
    }

    protected CustomerService CustomerService { get; }

    protected ICustomerQueries CustomerQueries { get; }

    protected CustomerCategoryService CustomerCategoryService { get; }

    protected SalesOrderService SalesOrderService { get; }

    protected InventoryManagmentDBContext AppDbContext { get; }

    protected static int TestUserId => IntegrationTestWebAppFactory.SeedUserId;

    public async Task InitializeAsync()
    {
        await CleanupCustomerFeatureDataAsync();
    }

    public async Task DisposeAsync()
    {
        await CleanupCustomerFeatureDataAsync();
        await _scope.DisposeAsync();
    }

    protected async Task AssertBaselineSeedIsAvailableAsync()
    {
        (await AppDbContext.Users.AnyAsync(e => e.Id == TestUserId)).Should().BeTrue();
        (await AppDbContext.ProductCategories.AnyAsync(e => e.Name == IntegrationTestWebAppFactory.DefaultProductCategoryName)).Should().BeTrue();
        (await AppDbContext.UnitOfMeasures.AnyAsync(e => e.Name == IntegrationTestWebAppFactory.DefaultUnitOfMeasureName)).Should().BeTrue();
        (await AppDbContext.Locations.AnyAsync(e => e.Name == IntegrationTestWebAppFactory.DefaultLocationName)).Should().BeTrue();
    }

    protected async Task<int> CreateCustomerCategoryAsync(
        string? name = null,
        bool isIndividual = false,
        string? description = "Customer feature integration category")
    {
        var category = new CustomerCategory
        {
            Name = name ?? $"{CategoryPrefix}{Guid.NewGuid().ToString("N")[..8]}",
            IsIndividual = isIndividual,
            Description = description,
            CreatedByUserId = TestUserId,
        };

        AppDbContext.CustomerCategories.Add(category);
        await AppDbContext.SaveChangesAsync();

        return category.Id;
    }

    protected CustomerCreateRequest BuildValidCustomerCreateRequest(
        int customerCategoryId,
        string? name = null,
        string? email = null,
        string? phone = null)
    {
        var token = Guid.NewGuid().ToString("N")[..8];

        return new CustomerCreateRequest
        {
            Name = name ?? $"{CustomerPrefix}{token}",
            CustomerCategoryId = customerCategoryId,
            Email = email ?? $"customer-{token}@ims.local",
            Phone = phone ?? "01000000000",
            Street = "Street 1",
            City = "City",
            State = "State",
            ZipCode = "12345",
        };
    }

    protected async Task<Customer> CreateCustomerDirectAsync(
        int? customerCategoryId = null,
        string? name = null,
        string? email = null)
    {
        var token = Guid.NewGuid().ToString("N")[..8];
        var categoryId = customerCategoryId ?? await CreateCustomerCategoryAsync();

        var customer = Customer.Create(
            name: name ?? $"{CustomerPrefix}{token}",
            customerCategoryId: categoryId,
            email: email ?? $"customer-direct-{token}@ims.local",
            phone: "01000000000",
            address: Address.Create("Street", "City", "State", "12345"));

        customer.CreatedByUserId = TestUserId;

        AppDbContext.Customers.Add(customer);
        await AppDbContext.SaveChangesAsync();

        return customer;
    }

    protected async Task<int> CreatePendingSalesOrderAsync(
        int customerId,
        decimal quantity = 2m,
        decimal unitPrice = 10m)
    {
        var (_, inventory) = await CreateProductInventoryAsync(
            quantityOnHand: Math.Max(100m, quantity + 10m),
            unitPrice: unitPrice);

        var request = new CreateSalesOrderRequest(
            customerId,
            $"{OrderDescriptionPrefix}{Guid.NewGuid().ToString("N")[..8]}",
            false,
            "Customer test address",
            PaymentStatus.Unpaid,
            [new SalesOrderItemRequest(inventory.ProductId, inventory.LocationId, quantity)]);

        var result = await SalesOrderService.CreateSalesOrderAsync(request);
        result.IsSuccess.Should().BeTrue();

        return result.Value;
    }

    protected async Task<int> CreateCompletedSalesOrderAsync(
        int customerId,
        decimal quantity = 2m,
        decimal unitPrice = 10m)
    {
        var orderId = await CreatePendingSalesOrderAsync(customerId, quantity, unitPrice);

        (await SalesOrderService.ConfirmOrderAsync(orderId)).IsSuccess.Should().BeTrue();
        (await SalesOrderService.MarkInTransitAsync(orderId)).IsSuccess.Should().BeTrue();
        (await SalesOrderService.ShipOrderAsync(orderId, "TRACK-IT")).IsSuccess.Should().BeTrue();
        (await SalesOrderService.CompleteOrderAsync(orderId)).IsSuccess.Should().BeTrue();

        return orderId;
    }

    private async Task<(Product Product, Inventory Inventory)> CreateProductInventoryAsync(
        decimal quantityOnHand,
        decimal unitPrice)
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

        var token = Guid.NewGuid().ToString("N")[..8];

        var product = new Product
        {
            Sku = $"SKU-CUSTOMER-IT-{token}",
            Name = $"{ProductPrefix}{token}",
            Description = "Customer feature integration product",
            CategoryId = categoryId,
            UnitOfMeasureId = unitOfMeasureId,
            UnitPrice = unitPrice,
            Cost = unitPrice <= 1m ? 0.5m : unitPrice - 1m,
            IsActive = true,
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
            MaxLevel = Math.Max(quantityOnHand + 20m, 500m),
            CreatedByUserId = TestUserId,
        };

        AppDbContext.Inventories.Add(inventory);
        await AppDbContext.SaveChangesAsync();

        return (product, inventory);
    }

    protected async Task CleanupCustomerFeatureDataAsync()
    {
        AppDbContext.ChangeTracker.Clear();

        var orderIds = await AppDbContext.SalesOrders
            .IgnoreQueryFilters()
            .Where(e => e.Description != null && EF.Functions.Like(e.Description, $"{OrderDescriptionPrefix}%"))
            .Select(e => e.Id)
            .ToListAsync();

        if (orderIds.Count > 0)
        {
            await AppDbContext.SalesOrderItems
                .IgnoreQueryFilters()
                .Where(e => orderIds.Contains(e.SalesOrderId))
                .ExecuteDeleteAsync();

            await AppDbContext.SalesOrders
                .IgnoreQueryFilters()
                .Where(e => orderIds.Contains(e.Id))
                .ExecuteDeleteAsync();
        }

        var productIds = await AppDbContext.Products
            .IgnoreQueryFilters()
            .Where(e => EF.Functions.Like(e.Name, $"{ProductPrefix}%"))
            .Select(e => e.Id)
            .ToListAsync();

        if (productIds.Count > 0)
        {
            var inventoryIds = await AppDbContext.Inventories
                .IgnoreQueryFilters()
                .Where(e => productIds.Contains(e.ProductId))
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

            await AppDbContext.Products
                .IgnoreQueryFilters()
                .Where(e => productIds.Contains(e.Id))
                .ExecuteDeleteAsync();
        }

        var customerCategoryIds = await AppDbContext.CustomerCategories
            .IgnoreQueryFilters()
            .Where(e => EF.Functions.Like(e.Name, $"{CategoryPrefix}%"))
            .Select(e => e.Id)
            .ToListAsync();

        if (customerCategoryIds.Count > 0)
        {
            await AppDbContext.Customers
                .IgnoreQueryFilters()
                .Where(e => EF.Functions.Like(e.Name, $"{CustomerPrefix}%")
                    || (e.CustomerCategoryId.HasValue && customerCategoryIds.Contains(e.CustomerCategoryId.Value)))
                .ExecuteDeleteAsync();

            await AppDbContext.CustomerCategories
                .IgnoreQueryFilters()
                .Where(e => customerCategoryIds.Contains(e.Id))
                .ExecuteDeleteAsync();
        }
        else
        {
            await AppDbContext.Customers
                .IgnoreQueryFilters()
                .Where(e => EF.Functions.Like(e.Name, $"{CustomerPrefix}%"))
                .ExecuteDeleteAsync();
        }

        await AppDbContext.SaveChangesAsync();
    }
}