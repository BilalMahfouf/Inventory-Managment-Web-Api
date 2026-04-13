using Application.IntegrationTests.Common;
using Application.Sales.RequestResponse;
using Application.Shared.Paging;
using Domain.Sales.Entities;
using Domain.Sales.Enums;
using Domain.Shared.Errors;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

using AppSalesOrderItemRequest = Application.Sales.RequestResponse.SalesOrderItemRequest;

namespace Application.IntegrationTests.Services;

public sealed class SalesOrderServiceTests : BaseIntegrationTest
{
    public SalesOrderServiceTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task CreateSalesOrderAsync_ValidOrder_PersistsOrderWithExpectedFields()
    {
        // Arrange
        await AssertBaselineSeedIsAvailableAsync();
        var customer = await CreateCustomerAsync();
        var (product, inventory) = await CreateProductInventoryAsync(quantityOnHand: 100m, unitPrice: 12m);

        var request = new CreateSalesOrderRequest(
            customer.Id,
            "Order description",
            false,
            "Main shipping address",
            PaymentStatus.Unpaid,
            [new AppSalesOrderItemRequest(product.Id, inventory.LocationId, 4m)]);

        // Act
        var result = await SalesOrderService.CreateSalesOrderAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();

        AppDbContext.ChangeTracker.Clear();
        var persistedOrder = await AppDbContext.SalesOrders
            .Include(e => e.Items)
            .SingleAsync(e => e.Id == result.Value);

        persistedOrder.CustomerId.Should().Be(customer.Id);
        persistedOrder.IsWalkIn.Should().BeFalse();
        persistedOrder.Description.Should().Be("Order description");
        persistedOrder.ShippingAddress.Should().Be("Main shipping address");
        persistedOrder.SalesStatus.Should().Be(SalesOrderStatus.Pending);
        persistedOrder.Items.Should().ContainSingle();
        persistedOrder.Items.Single().OrderedQuantity.Should().Be(4m);
    }

    [Fact]
    public async Task CreateSalesOrderAsync_ValidWalkInOrder_PersistsAsCompleted()
    {
        // Arrange
        var (product, inventory) = await CreateProductInventoryAsync(quantityOnHand: 60m, unitPrice: 8m);

        var request = new CreateSalesOrderRequest(
            null,
            "Walk-in order",
            true,
            null,
            PaymentStatus.Unpaid,
            [new AppSalesOrderItemRequest(product.Id, inventory.LocationId, 2m)]);

        // Act
        var result = await SalesOrderService.CreateSalesOrderAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();

        AppDbContext.ChangeTracker.Clear();
        var persistedOrder = await AppDbContext.SalesOrders
            .Include(e => e.Items)
            .SingleAsync(e => e.Id == result.Value);

        persistedOrder.CustomerId.Should().BeNull();
        persistedOrder.IsWalkIn.Should().BeTrue();
        persistedOrder.SalesStatus.Should().Be(SalesOrderStatus.Completed);
        persistedOrder.Items.Should().ContainSingle();
    }

    [Fact]
    public async Task CreateSalesOrderAsync_NullItems_ReturnsValidationFailure()
    {
        // Arrange
        var request = new CreateSalesOrderRequest(
            1,
            "Order",
            false,
            "Address",
            PaymentStatus.Unpaid,
            null!);

        // Act
        var result = await SalesOrderService.CreateSalesOrderAsync(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorType.Validation);
    }

    [Fact]
    public async Task CreateSalesOrderAsync_EmptyItems_ReturnsValidationFailure()
    {
        // Arrange
        var request = new CreateSalesOrderRequest(
            1,
            "Order",
            false,
            "Address",
            PaymentStatus.Unpaid,
            []);

        // Act
        var result = await SalesOrderService.CreateSalesOrderAsync(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorType.Validation);
    }

    [Fact]
    public async Task CreateSalesOrderAsync_MissingCustomerForNonWalkIn_ReturnsValidationFailure()
    {
        // Arrange
        var request = new CreateSalesOrderRequest(
            null,
            "Order",
            false,
            "Address",
            PaymentStatus.Unpaid,
            [new AppSalesOrderItemRequest(1, 1, 1m)]);

        // Act
        var result = await SalesOrderService.CreateSalesOrderAsync(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorType.Validation);
    }

    [Fact]
    public async Task CreateSalesOrderAsync_WalkInWithCustomer_ReturnsValidationFailure()
    {
        // Arrange
        var request = new CreateSalesOrderRequest(
            1,
            "Order",
            true,
            null,
            PaymentStatus.Unpaid,
            [new AppSalesOrderItemRequest(1, 1, 1m)]);

        // Act
        var result = await SalesOrderService.CreateSalesOrderAsync(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorType.Validation);
    }

    [Fact]
    public async Task CreateSalesOrderAsync_NegativeQuantity_ReturnsValidationFailure()
    {
        // Arrange
        var (product, inventory) = await CreateProductInventoryAsync(quantityOnHand: 20m);

        var request = new CreateSalesOrderRequest(
            1,
            "Order",
            false,
            "Address",
            PaymentStatus.Unpaid,
            [new AppSalesOrderItemRequest(product.Id, inventory.LocationId, -3m)]);

        // Act
        var result = await SalesOrderService.CreateSalesOrderAsync(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorType.Validation);
    }

    [Fact]
    public async Task CreateSalesOrderAsync_MissingInventory_ReturnsNotFoundFailure()
    {
        // Arrange
        var customer = await CreateCustomerAsync();
        var (product, _) = await CreateProductInventoryAsync(quantityOnHand: 10m);

        var request = new CreateSalesOrderRequest(
            customer.Id,
            "Order",
            false,
            "Address",
            PaymentStatus.Unpaid,
            [new AppSalesOrderItemRequest(product.Id, 999_999, 1m)]);

        // Act
        var result = await SalesOrderService.CreateSalesOrderAsync(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public async Task CreateSalesOrderAsync_InventoryProductMismatch_ReturnsNotFoundFailure()
    {
        // Arrange
        var customer = await CreateCustomerAsync();
        var (_, inventory) = await CreateProductInventoryAsync(quantityOnHand: 20m);

        var request = new CreateSalesOrderRequest(
            customer.Id,
            "Order",
            false,
            "Address",
            PaymentStatus.Unpaid,
            [new AppSalesOrderItemRequest(inventory.ProductId + 100, inventory.LocationId, 1m)]);

        // Act
        var result = await SalesOrderService.CreateSalesOrderAsync(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public async Task CreateSalesOrderAsync_InsufficientStock_ReturnsConflictFailure()
    {
        // Arrange
        var customer = await CreateCustomerAsync();
        var (product, inventory) = await CreateProductInventoryAsync(quantityOnHand: 2m);

        var request = new CreateSalesOrderRequest(
            customer.Id,
            "Order",
            false,
            "Address",
            PaymentStatus.Unpaid,
            [new AppSalesOrderItemRequest(product.Id, inventory.LocationId, 10m)]);

        // Act
        var result = await SalesOrderService.CreateSalesOrderAsync(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorType.Conflict);
    }

    [Fact]
    public async Task CreateSalesOrderAsync_DuplicatePayload_CreatesDistinctOrders()
    {
        // Arrange
        var customer = await CreateCustomerAsync();
        var (product, inventory) = await CreateProductInventoryAsync(quantityOnHand: 100m);

        var request = new CreateSalesOrderRequest(
            customer.Id,
            "Duplicate check",
            false,
            "Address",
            PaymentStatus.Unpaid,
            [new AppSalesOrderItemRequest(product.Id, inventory.LocationId, 1m)]);

        // Act
        var firstResult = await SalesOrderService.CreateSalesOrderAsync(request);
        var secondResult = await SalesOrderService.CreateSalesOrderAsync(request);

        // Assert
        firstResult.IsSuccess.Should().BeTrue();
        secondResult.IsSuccess.Should().BeTrue();
        secondResult.Value.Should().NotBe(firstResult.Value);

        var persistedCount = await AppDbContext.SalesOrders.CountAsync();
        persistedCount.Should().Be(2);
    }

    [Fact]
    public async Task UpdateSalesOrderAsync_ValidPendingOrder_UpdatesPersistedFieldsAndItems()
    {
        // Arrange
        var customer = await CreateCustomerAsync();
        var (_, firstInventory) = await CreateProductInventoryAsync(quantityOnHand: 50m, unitPrice: 10m);
        var (_, secondInventory) = await CreateProductInventoryAsync(quantityOnHand: 80m, unitPrice: 15m);

        var createResult = await SalesOrderService.CreateSalesOrderAsync(new CreateSalesOrderRequest(
            customer.Id,
            "Original description",
            false,
            "Original address",
            PaymentStatus.Unpaid,
            [new AppSalesOrderItemRequest(firstInventory.ProductId, firstInventory.LocationId, 3m)]));

        createResult.IsSuccess.Should().BeTrue();

        var updateRequest = new UpdateSalesOrderRequest(
            customer.Id,
            "Updated description",
            "Updated address",
            [
                new AppSalesOrderItemRequest(secondInventory.ProductId, secondInventory.LocationId, 1m),
                new AppSalesOrderItemRequest(secondInventory.ProductId, secondInventory.LocationId, 2m),
            ]);

        // Act
        var updateResult = await SalesOrderService.UpdateSalesOrderAsync(createResult.Value, updateRequest);

        // Assert
        updateResult.IsSuccess.Should().BeTrue($"UpdateSalesOrderAsync failed: {updateResult.Error.Description}");

        AppDbContext.ChangeTracker.Clear();
        var order = await AppDbContext.SalesOrders
            .Include(e => e.Items)
            .SingleAsync(e => e.Id == createResult.Value);

        var updatedFirstInventory = await AppDbContext.Inventories.SingleAsync(e => e.Id == firstInventory.Id);
        var updatedSecondInventory = await AppDbContext.Inventories.SingleAsync(e => e.Id == secondInventory.Id);

        order.Description.Should().Be("Updated description");
        order.ShippingAddress.Should().Be("Updated address");
        order.Items.Should().ContainSingle();
        order.Items.Single().InventoryId.Should().Be(secondInventory.Id);
        order.Items.Single().OrderedQuantity.Should().Be(3m);

        updatedFirstInventory.QuantityOnHand.Should().Be(50m);
        updatedSecondInventory.QuantityOnHand.Should().Be(77m);
    }

    [Fact]
    public async Task UpdateSalesOrderAsync_NonExistingOrder_ReturnsNotFoundFailure()
    {
        // Arrange
        var request = new UpdateSalesOrderRequest(
            1,
            "Updated",
            "Address",
            null);

        // Act
        var result = await SalesOrderService.UpdateSalesOrderAsync(999_999, request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public async Task UpdateSalesOrderAsync_NonPendingOrder_ReturnsConflictFailure()
    {
        // Arrange
        var (_, inventory) = await CreateProductInventoryAsync(quantityOnHand: 40m);
        var walkInOrderResult = await SalesOrderService.CreateSalesOrderAsync(new CreateSalesOrderRequest(
            null,
            "Walk-in",
            true,
            null,
            PaymentStatus.Unpaid,
            [new AppSalesOrderItemRequest(inventory.ProductId, inventory.LocationId, 2m)]));

        walkInOrderResult.IsSuccess.Should().BeTrue();

        var updateRequest = new UpdateSalesOrderRequest(
            null,
            "Updated",
            "Address",
            [new AppSalesOrderItemRequest(inventory.ProductId, inventory.LocationId, 1m)]);

        // Act
        var result = await SalesOrderService.UpdateSalesOrderAsync(walkInOrderResult.Value, updateRequest);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorType.Conflict);
    }

    [Fact]
    public async Task UpdateSalesOrderAsync_MissingInventory_ReturnsNotFoundFailure()
    {
        // Arrange
        var customer = await CreateCustomerAsync();
        var (_, inventory) = await CreateProductInventoryAsync(quantityOnHand: 40m);

        var createResult = await SalesOrderService.CreateSalesOrderAsync(new CreateSalesOrderRequest(
            customer.Id,
            "Seed",
            false,
            "Address",
            PaymentStatus.Unpaid,
            [new AppSalesOrderItemRequest(inventory.ProductId, inventory.LocationId, 2m)]));

        createResult.IsSuccess.Should().BeTrue();

        var updateRequest = new UpdateSalesOrderRequest(
            customer.Id,
            "Updated",
            "Address",
            [new AppSalesOrderItemRequest(inventory.ProductId, 777_777, 1m)]);

        // Act
        var result = await SalesOrderService.UpdateSalesOrderAsync(createResult.Value, updateRequest);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public async Task ConfirmOrderAsync_PendingOrder_TransitionsToConfirmed()
    {
        // Arrange
        var orderId = await CreatePendingOrderAsync();

        // Act
        var result = await SalesOrderService.ConfirmOrderAsync(orderId);

        // Assert
        result.IsSuccess.Should().BeTrue();

        AppDbContext.ChangeTracker.Clear();
        var order = await AppDbContext.SalesOrders.SingleAsync(e => e.Id == orderId);
        order.SalesStatus.Should().Be(SalesOrderStatus.Confirmed);
    }

    [Fact]
    public async Task ConfirmOrderAsync_NonPendingOrder_ReturnsConflictFailure()
    {
        // Arrange
        var walkInOrderId = await CreateWalkInCompletedOrderAsync();

        // Act
        var result = await SalesOrderService.ConfirmOrderAsync(walkInOrderId);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorType.Conflict);
    }

    [Fact]
    public async Task MarkInTransitAsync_ConfirmedOrder_TransitionsToInTransit()
    {
        // Arrange
        var orderId = await CreateConfirmedOrderAsync();

        // Act
        var result = await SalesOrderService.MarkInTransitAsync(orderId);

        // Assert
        result.IsSuccess.Should().BeTrue();

        AppDbContext.ChangeTracker.Clear();
        var order = await AppDbContext.SalesOrders.SingleAsync(e => e.Id == orderId);
        order.SalesStatus.Should().Be(SalesOrderStatus.InTransit);
    }

    [Fact]
    public async Task MarkInTransitAsync_PendingOrder_ReturnsConflictFailure()
    {
        // Arrange
        var orderId = await CreatePendingOrderAsync();

        // Act
        var result = await SalesOrderService.MarkInTransitAsync(orderId);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorType.Conflict);
    }

    [Fact]
    public async Task ShipOrderAsync_InTransitOrder_TransitionsToShippedAndSetsTracking()
    {
        // Arrange
        var orderId = await CreateInTransitOrderAsync();

        // Act
        var result = await SalesOrderService.ShipOrderAsync(orderId, "TRACK-123");

        // Assert
        result.IsSuccess.Should().BeTrue();

        AppDbContext.ChangeTracker.Clear();
        var order = await AppDbContext.SalesOrders.SingleAsync(e => e.Id == orderId);
        order.SalesStatus.Should().Be(SalesOrderStatus.Shipped);
        order.TrackingNumber.Should().Be("TRACK-123");
    }

    [Fact]
    public async Task ShipOrderAsync_ConfirmedOrder_ReturnsConflictFailure()
    {
        // Arrange
        var orderId = await CreateConfirmedOrderAsync();

        // Act
        var result = await SalesOrderService.ShipOrderAsync(orderId, "TRACK-123");

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorType.Conflict);
    }

    [Fact]
    public async Task CompleteOrderAsync_ShippedOrder_TransitionsToCompleted()
    {
        // Arrange
        var orderId = await CreateShippedOrderAsync();

        // Act
        var result = await SalesOrderService.CompleteOrderAsync(orderId);

        // Assert
        result.IsSuccess.Should().BeTrue();

        AppDbContext.ChangeTracker.Clear();
        var order = await AppDbContext.SalesOrders.SingleAsync(e => e.Id == orderId);
        order.SalesStatus.Should().Be(SalesOrderStatus.Completed);
    }

    [Fact]
    public async Task CompleteOrderAsync_InTransitOrder_ReturnsConflictFailure()
    {
        // Arrange
        var orderId = await CreateInTransitOrderAsync();

        // Act
        var result = await SalesOrderService.CompleteOrderAsync(orderId);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorType.Conflict);
    }

    [Fact]
    public async Task CancelOrderAsync_ExistingPendingOrder_CancelsAndRestoresStock()
    {
        // Arrange
        var customer = await CreateCustomerAsync();
        var (_, inventory) = await CreateProductInventoryAsync(quantityOnHand: 40m);

        var createResult = await SalesOrderService.CreateSalesOrderAsync(new CreateSalesOrderRequest(
            customer.Id,
            "Seed",
            false,
            "Address",
            PaymentStatus.Unpaid,
            [new AppSalesOrderItemRequest(inventory.ProductId, inventory.LocationId, 5m)]));

        createResult.IsSuccess.Should().BeTrue();

        // Act
        var cancelResult = await SalesOrderService.CancelOrderAsync(createResult.Value);

        // Assert
        cancelResult.IsSuccess.Should().BeTrue();

        AppDbContext.ChangeTracker.Clear();
        var order = await AppDbContext.SalesOrders.SingleAsync(e => e.Id == createResult.Value);
        var updatedInventory = await AppDbContext.Inventories.SingleAsync(e => e.Id == inventory.Id);

        order.SalesStatus.Should().Be(SalesOrderStatus.Cancelled);
        updatedInventory.QuantityOnHand.Should().Be(40m);
    }

    [Fact]
    public async Task CancelOrderAsync_NonExistingOrder_ReturnsNotFoundFailure()
    {
        // Arrange
        const int missingOrderId = 999_999;

        // Act
        var result = await SalesOrderService.CancelOrderAsync(missingOrderId);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public async Task CancelOrderAsync_ShippedOrder_ReturnsConflictFailure()
    {
        // Arrange
        var orderId = await CreateShippedOrderAsync();

        // Act
        var result = await SalesOrderService.CancelOrderAsync(orderId);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorType.Conflict);
    }

    [Fact]
    public async Task ReturnOrderAsync_CompletedOrder_TransitionsToReturned()
    {
        // Arrange
        var orderId = await CreateCompletedOrderAsync();

        // Act
        var result = await SalesOrderService.ReturnOrderAsync(orderId);

        // Assert
        result.IsSuccess.Should().BeTrue();

        AppDbContext.ChangeTracker.Clear();
        var order = await AppDbContext.SalesOrders.SingleAsync(e => e.Id == orderId);
        order.SalesStatus.Should().Be(SalesOrderStatus.Returned);
    }

    [Fact]
    public async Task ReturnOrderAsync_NonCompletedOrder_ReturnsConflictFailure()
    {
        // Arrange
        var orderId = await CreateShippedOrderAsync();

        // Act
        var result = await SalesOrderService.ReturnOrderAsync(orderId);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorType.Conflict);
    }

    [Fact]
    public async Task UpdatePaymentAsync_NegativeAmount_ReturnsValidationFailure()
    {
        // Act
        var result = await SalesOrderService.UpdatePaymentAsync(new(999_999, -1m, PaymentStatus.Unpaid));

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorType.Validation);
    }

    [Fact]
    public async Task UpdatePaymentAsync_NonExistingOrder_ReturnsNotFoundFailure()
    {
        // Act
        var result = await SalesOrderService.UpdatePaymentAsync(new(999_999, 0m, PaymentStatus.Unpaid));

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public async Task UpdatePaymentAsync_ValidPartialPayment_UpdatesAmountAndStatus()
    {
        // Arrange
        var customer = await CreateCustomerAsync();
        var (_, inventory) = await CreateProductInventoryAsync(quantityOnHand: 40m, unitPrice: 10m);

        var createResult = await SalesOrderService.CreateSalesOrderAsync(new CreateSalesOrderRequest(
            customer.Id,
            "Seed",
            false,
            "Address",
            PaymentStatus.Unpaid,
            [new AppSalesOrderItemRequest(inventory.ProductId, inventory.LocationId, 3m)]));

        createResult.IsSuccess.Should().BeTrue();

        // Act
        var updateResult = await SalesOrderService.UpdatePaymentAsync(new(createResult.Value, 15m, PaymentStatus.PartiallyPaid));

        // Assert
        updateResult.IsSuccess.Should().BeTrue(updateResult.Error.Description);

        AppDbContext.ChangeTracker.Clear();
        var order = await AppDbContext.SalesOrders
            .Include(e => e.Items)
            .SingleAsync(e => e.Id == createResult.Value);

        order.TotalAmount.Should().Be(30m);
        order.TotalPaidAmount.Should().Be(15m);
        order.PaymentStatus.Should().Be(PaymentStatus.PartiallyPaid);
    }

    [Fact]
    public async Task UpdatePaymentAsync_ValidFullPayment_UpdatesAmountAndStatus()
    {
        // Arrange
        var customer = await CreateCustomerAsync();
        var (_, inventory) = await CreateProductInventoryAsync(quantityOnHand: 40m, unitPrice: 10m);

        var createResult = await SalesOrderService.CreateSalesOrderAsync(new CreateSalesOrderRequest(
            customer.Id,
            "Seed",
            false,
            "Address",
            PaymentStatus.Unpaid,
            [new AppSalesOrderItemRequest(inventory.ProductId, inventory.LocationId, 3m)]));

        createResult.IsSuccess.Should().BeTrue();

        // Act
        var updateResult = await SalesOrderService.UpdatePaymentAsync(new(createResult.Value, 30m, PaymentStatus.Paid));

        // Assert
        updateResult.IsSuccess.Should().BeTrue(updateResult.Error.Description);

        AppDbContext.ChangeTracker.Clear();
        var order = await AppDbContext.SalesOrders
            .Include(e => e.Items)
            .SingleAsync(e => e.Id == createResult.Value);

        order.TotalAmount.Should().Be(30m);
        order.TotalPaidAmount.Should().Be(30m);
        order.PaymentStatus.Should().Be(PaymentStatus.Paid);
    }

    [Fact]
    public async Task UpdatePaymentAsync_AmountExceedsTotal_ReturnsConflictFailure()
    {
        // Arrange
        var customer = await CreateCustomerAsync();
        var (_, inventory) = await CreateProductInventoryAsync(quantityOnHand: 40m, unitPrice: 10m);

        var createResult = await SalesOrderService.CreateSalesOrderAsync(new CreateSalesOrderRequest(
            customer.Id,
            "Seed",
            false,
            "Address",
            PaymentStatus.Unpaid,
            [new AppSalesOrderItemRequest(inventory.ProductId, inventory.LocationId, 3m)]));

        createResult.IsSuccess.Should().BeTrue();

        // Act
        var updateResult = await SalesOrderService.UpdatePaymentAsync(new(createResult.Value, 31m, PaymentStatus.Paid));

        // Assert
        updateResult.IsSuccess.Should().BeFalse();
        updateResult.Error.Type.Should().Be(ErrorType.Conflict);
    }

    [Fact]
    public async Task UpdatePaymentAsync_MarkAsPaidWithLessThanTotal_ReturnsConflictFailure()
    {
        // Arrange
        var customer = await CreateCustomerAsync();
        var (_, inventory) = await CreateProductInventoryAsync(quantityOnHand: 40m, unitPrice: 10m);

        var createResult = await SalesOrderService.CreateSalesOrderAsync(new CreateSalesOrderRequest(
            customer.Id,
            "Seed",
            false,
            "Address",
            PaymentStatus.Unpaid,
            [new AppSalesOrderItemRequest(inventory.ProductId, inventory.LocationId, 3m)]));

        createResult.IsSuccess.Should().BeTrue();

        // Act
        var updateResult = await SalesOrderService.UpdatePaymentAsync(new(createResult.Value, 10m, PaymentStatus.Paid));

        // Assert
        updateResult.IsSuccess.Should().BeFalse();
        updateResult.Error.Type.Should().Be(ErrorType.Conflict);
    }

    [Fact]
    public async Task UpdatePaymentAsync_MarkAsUnpaidWithPositiveAmount_ReturnsConflictFailure()
    {
        // Arrange
        var customer = await CreateCustomerAsync();
        var (_, inventory) = await CreateProductInventoryAsync(quantityOnHand: 40m, unitPrice: 10m);

        var createResult = await SalesOrderService.CreateSalesOrderAsync(new CreateSalesOrderRequest(
            customer.Id,
            "Seed",
            false,
            "Address",
            PaymentStatus.Unpaid,
            [new AppSalesOrderItemRequest(inventory.ProductId, inventory.LocationId, 3m)]));

        createResult.IsSuccess.Should().BeTrue();

        // Act
        var updateResult = await SalesOrderService.UpdatePaymentAsync(new(createResult.Value, 10m, PaymentStatus.Unpaid));

        // Assert
        updateResult.IsSuccess.Should().BeFalse();
        updateResult.Error.Type.Should().Be(ErrorType.Conflict);
    }

    [Fact]
    public async Task UpdatePaymentAsync_AlreadyPaidOrder_ReturnsConflictFailure()
    {
        // Arrange
        var customer = await CreateCustomerAsync();
        var (_, inventory) = await CreateProductInventoryAsync(quantityOnHand: 40m, unitPrice: 10m);

        var createResult = await SalesOrderService.CreateSalesOrderAsync(new CreateSalesOrderRequest(
            customer.Id,
            "Seed",
            false,
            "Address",
            PaymentStatus.Unpaid,
            [new AppSalesOrderItemRequest(inventory.ProductId, inventory.LocationId, 3m)]));

        createResult.IsSuccess.Should().BeTrue();

        var firstUpdateResult = await SalesOrderService.UpdatePaymentAsync(new(createResult.Value, 30m, PaymentStatus.Paid));
        firstUpdateResult.IsSuccess.Should().BeTrue(firstUpdateResult.Error.Description);

        // Act
        var secondUpdateResult = await SalesOrderService.UpdatePaymentAsync(new(createResult.Value, 30m, PaymentStatus.Paid));

        // Assert
        secondUpdateResult.IsSuccess.Should().BeFalse();
        secondUpdateResult.Error.Type.Should().Be(ErrorType.Conflict);
    }

    [Fact]
    public async Task GetSalesOrderByIdAsync_ExistingOrder_ReturnsExpectedAggregate()
    {
        // Arrange
        var customer = await CreateCustomerAsync();
        var (_, inventory) = await CreateProductInventoryAsync(quantityOnHand: 40m, unitPrice: 20m);

        var createResult = await SalesOrderService.CreateSalesOrderAsync(new CreateSalesOrderRequest(
            customer.Id,
            "Query me",
            false,
            "Address",
            PaymentStatus.Unpaid,
            [new AppSalesOrderItemRequest(inventory.ProductId, inventory.LocationId, 2m)]));

        createResult.IsSuccess.Should().BeTrue();

        // Act
        var result = await SalesOrderQueries.GetSalesOrderByIdAsync(createResult.Value);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(createResult.Value);
        result.Value.CustomerId.Should().Be(customer.Id);
        result.Value.IsWalkIn.Should().BeFalse();
        result.Value.TotalAmount.Should().Be(40m);
        result.Value.Items.Should().ContainSingle();
    }

    [Fact]
    public async Task GetSalesOrderByIdAsync_NonExistingOrder_ReturnsNotFoundFailure()
    {
        // Arrange
        const int missingOrderId = 999_999;

        // Act
        var result = await SalesOrderQueries.GetSalesOrderByIdAsync(missingOrderId);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public async Task GetSalesOrdersAsync_EmptyDatabase_ReturnsNotFoundFailure()
    {
        // Arrange
        var tableRequest = TableRequest.Create(pageSize: 10, page: 1);

        // Act
        var result = await SalesOrderQueries.GetSalesOrdersAsync(
            tableRequest,
            status: null,
            customerId: null,
            dateFrom: null,
            dateTo: null);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public async Task GetSalesOrdersAsync_WithSeededOrders_ReturnsExpectedCountAndData()
    {
        // Arrange
        var customer = await CreateCustomerAsync();
        var (_, inventoryOne) = await CreateProductInventoryAsync(quantityOnHand: 40m, unitPrice: 12m);
        var (_, inventoryTwo) = await CreateProductInventoryAsync(quantityOnHand: 40m, unitPrice: 20m);

        var nonWalkInCreate = await SalesOrderService.CreateSalesOrderAsync(new CreateSalesOrderRequest(
            customer.Id,
            "Customer order",
            false,
            "Address",
            PaymentStatus.Unpaid,
            [new AppSalesOrderItemRequest(inventoryOne.ProductId, inventoryOne.LocationId, 2m)]));

        var walkInCreate = await SalesOrderService.CreateSalesOrderAsync(new CreateSalesOrderRequest(
            null,
            "Walk-in order",
            true,
            null,
            PaymentStatus.Unpaid,
            [new AppSalesOrderItemRequest(inventoryTwo.ProductId, inventoryTwo.LocationId, 1m)]));

        nonWalkInCreate.IsSuccess.Should().BeTrue();
        walkInCreate.IsSuccess.Should().BeTrue();

        var tableRequest = TableRequest.Create(pageSize: 20, page: 1);

        // Act
        var result = await SalesOrderQueries.GetSalesOrdersAsync(
            tableRequest,
            status: null,
            customerId: null,
            dateFrom: null,
            dateTo: null);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.TotalCount.Should().Be(2);
        result.Value.Item.Should().HaveCount(2);
        result.Value.Item.Should().Contain(e => e.CustomerName == "Walk-in");
        result.Value.Item.Should().Contain(e => e.CustomerId == customer.Id);
    }

    private async Task<int> CreatePendingOrderAsync(decimal quantity = 2m)
    {
        var customer = await CreateCustomerAsync();
        var (_, inventory) = await CreateProductInventoryAsync(quantityOnHand: 50m);

        var createResult = await SalesOrderService.CreateSalesOrderAsync(new CreateSalesOrderRequest(
            customer.Id,
            "Seed pending",
            false,
            "Address",
            PaymentStatus.Unpaid,
            [new AppSalesOrderItemRequest(inventory.ProductId, inventory.LocationId, quantity)]));

        createResult.IsSuccess.Should().BeTrue(createResult.Error.Description);

        return createResult.Value;
    }

    private async Task<int> CreateWalkInCompletedOrderAsync()
    {
        var (_, inventory) = await CreateProductInventoryAsync(quantityOnHand: 50m);

        var createResult = await SalesOrderService.CreateSalesOrderAsync(new CreateSalesOrderRequest(
            null,
            "Seed walk-in",
            true,
            null,
            PaymentStatus.Unpaid,
            [new AppSalesOrderItemRequest(inventory.ProductId, inventory.LocationId, 1m)]));

        createResult.IsSuccess.Should().BeTrue(createResult.Error.Description);

        return createResult.Value;
    }

    private async Task<int> CreateConfirmedOrderAsync()
    {
        var orderId = await CreatePendingOrderAsync();
        var confirmResult = await SalesOrderService.ConfirmOrderAsync(orderId);
        confirmResult.IsSuccess.Should().BeTrue(confirmResult.Error.Description);
        return orderId;
    }

    private async Task<int> CreateInTransitOrderAsync()
    {
        var orderId = await CreateConfirmedOrderAsync();
        var transitResult = await SalesOrderService.MarkInTransitAsync(orderId);
        transitResult.IsSuccess.Should().BeTrue(transitResult.Error.Description);
        return orderId;
    }

    private async Task<int> CreateShippedOrderAsync()
    {
        var orderId = await CreateInTransitOrderAsync();
        var shipResult = await SalesOrderService.ShipOrderAsync(orderId, "seed-track");
        shipResult.IsSuccess.Should().BeTrue(shipResult.Error.Description);
        return orderId;
    }

    private async Task<int> CreateCompletedOrderAsync()
    {
        var orderId = await CreateShippedOrderAsync();
        var completeResult = await SalesOrderService.CompleteOrderAsync(orderId);
        completeResult.IsSuccess.Should().BeTrue(completeResult.Error.Description);
        return orderId;
    }
}
