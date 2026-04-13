using Application.IntegrationTests.Common;
using Application.Inventories.DTOs.Request;
using Domain.Inventories;
using Domain.Sales;
using Domain.Shared.Events;
using FluentAssertions;
using Infrastructure.Outbox;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Application.IntegrationTests.Services;

public sealed class DomainEventsIntegrationTests : InventoryFeaturesIntegrationTestBase
{
    public DomainEventsIntegrationTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public void DependencyInjection_ShouldResolve_CustomDomainEventServices_AndLowStockHandler()
    {
        var publisher = Services.GetService<IDomainEventPublisher>();
        var dispatcher = Services.GetService<IDomainEventDispatcher>();
        var handlers = Services.GetServices<IDomainEventHandler<LowStockDomainEvent>>();

        publisher.Should().NotBeNull();
        dispatcher.Should().NotBeNull();
        handlers.Should().NotBeEmpty();
    }

    [Fact]
    public async Task UpdateAsync_WhenStockDropsBelowReorderLevel_ShouldCreateLowStockOutboxMessage()
    {
        var product = await CreateProductAsync(isActive: true);
        var locationId = await GetDefaultLocationIdAsync();

        var createResult = await InventoryService.CreateAsync(
            await BuildValidCreateRequestAsync(
                productId: product.Id,
                locationId: locationId,
                quantityOnHand: 20m,
                reorderLevel: 5m,
                maxLevel: 30m),
            CancellationToken.None);

        createResult.IsSuccess.Should().BeTrue();

        var beforeCount = await AppDbContext.Set<OutboxMessages>()
            .CountAsync(e => e.Name == nameof(LowStockDomainEvent));

        var updateResult = await InventoryService.UpdateAsync(
            createResult.Value.Id,
            new InventoryUpdateRequest
            {
                QuantityOnHand = 2m,
                ReorderLevel = 5m,
                MaxLevel = 30m,
            },
            CancellationToken.None);

        updateResult.IsSuccess.Should().BeTrue();

        AppDbContext.ChangeTracker.Clear();

        var outboxEntries = await AppDbContext.Set<OutboxMessages>()
            .Where(e => e.Name == nameof(LowStockDomainEvent))
            .OrderByDescending(e => e.CreatedOnUtc)
            .ToListAsync();

        outboxEntries.Count.Should().Be(beforeCount + 1);

        var latest = outboxEntries.First();
        latest.ProcessedOnUtc.Should().BeNull();
        latest.Content.Should().Contain(nameof(LowStockDomainEvent));
    }

    [Fact]
    public async Task PublisherAndDispatcher_ShouldHandleRegisteredAndUnregisteredEvents()
    {
        var publisher = Services.GetRequiredService<IDomainEventPublisher>();
        var dispatcher = Services.GetRequiredService<IDomainEventDispatcher>();

        var product = await CreateProductAsync(isActive: true);
        var locationId = await GetDefaultLocationIdAsync();

        var createResult = await InventoryService.CreateAsync(
            await BuildValidCreateRequestAsync(
                productId: product.Id,
                locationId: locationId,
                quantityOnHand: 3m,
                reorderLevel: 5m,
                maxLevel: 30m),
            CancellationToken.None);

        createResult.IsSuccess.Should().BeTrue();

        var lowStockEvent = new LowStockDomainEvent(product.Id, locationId, 3m);
        var unhandledEvent = new SalesOrderCompletedDomainEvent(999_999);

        await publisher
            .Invoking(p => p.PublishAsync(lowStockEvent, CancellationToken.None))
            .Should()
            .NotThrowAsync();

        await publisher
            .Invoking(p => p.PublishAsync(unhandledEvent, CancellationToken.None))
            .Should()
            .NotThrowAsync();

        await dispatcher
            .Invoking(d => d.DispatchAsync(new IDomainEvent[] { lowStockEvent, unhandledEvent }, CancellationToken.None))
            .Should()
            .NotThrowAsync();
    }
}
