using Application.IntegrationTests.Common;
using Application.Inventories.DTOs.Request;
using Application.Shared.Paging;
using Domain.Inventories.Enums;
using Domain.Shared.Errors;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Application.IntegrationTests.Services;

public sealed class InventoryFeatureTests : InventoryFeaturesIntegrationTestBase
{
    public InventoryFeatureTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task CreateAsync_ValidRequest_PersistsInventoryAndInitialStockMovement()
    {
        await AssertBaselineSeedIsAvailableAsync();
        var product = await CreateProductAsync(isActive: true);
        var locationId = await GetDefaultLocationIdAsync();

        var request = await BuildValidCreateRequestAsync(
            productId: product.Id,
            locationId: locationId,
            quantityOnHand: 12m,
            reorderLevel: 4m,
            maxLevel: 30m);

        var result = await InventoryService.CreateAsync(request, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.ProductId.Should().Be(product.Id);
        result.Value.LocationId.Should().Be(locationId);
        result.Value.QuantityOnHand.Should().Be(12m);

        AppDbContext.ChangeTracker.Clear();

        var persistedInventory = await AppDbContext.Inventories
            .SingleAsync(e => e.Id == result.Value.Id);

        persistedInventory.QuantityOnHand.Should().Be(12m);
        persistedInventory.ReorderLevel.Should().Be(4m);
        persistedInventory.MaxLevel.Should().Be(30m);

        var movements = await AppDbContext.StockMovements
            .Where(e => e.InventoryId == persistedInventory.Id)
            .ToListAsync();

        movements.Should().ContainSingle();
        movements.Single().MovementTypeId.Should().Be((int)StockMovementTypeEnum.InitialStock);
        movements.Single().StockMovmentStatus.Should().Be(StockMovementStatus.Completed);
        movements.Single().Quantity.Should().Be(12m);
    }

    [Fact]
    public async Task CreateAsync_InvalidRequest_ReturnsValidationFailure()
    {
        var request = new InventoryCreateRequest
        {
            ProductId = 0,
            LocationId = 0,
            QuantityOnHand = -1m,
            ReorderLevel = -1m,
            MaxLevel = 0m,
        };

        var result = await InventoryService.CreateAsync(request, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorType.Validation);
    }

    [Fact]
    public async Task CreateAsync_InactiveProduct_ReturnsValidationFailure()
    {
        var inactiveProduct = await CreateProductAsync(isActive: false);
        var locationId = await GetDefaultLocationIdAsync();

        var request = await BuildValidCreateRequestAsync(
            productId: inactiveProduct.Id,
            locationId: locationId);

        var result = await InventoryService.CreateAsync(request, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorType.Validation);
    }

    [Fact]
    public async Task CreateAsync_InactiveLocation_ReturnsValidationFailure()
    {
        var product = await CreateProductAsync(isActive: true);
        var inactiveLocation = await CreateAdditionalLocationAsync(isActive: false);

        var request = await BuildValidCreateRequestAsync(
            productId: product.Id,
            locationId: inactiveLocation.Id);

        var result = await InventoryService.CreateAsync(request, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorType.Validation);
    }

    [Fact]
    public async Task CreateAsync_DuplicateProductLocation_ReturnsConflictFailure()
    {
        var product = await CreateProductAsync(isActive: true);
        var locationId = await GetDefaultLocationIdAsync();

        var request = await BuildValidCreateRequestAsync(
            productId: product.Id,
            locationId: locationId);

        var first = await InventoryService.CreateAsync(request, CancellationToken.None);
        var second = await InventoryService.CreateAsync(request, CancellationToken.None);

        first.IsSuccess.Should().BeTrue();
        second.IsSuccess.Should().BeFalse();
        second.Error.Type.Should().Be(ErrorType.Conflict);
    }

    [Fact]
    public async Task CreateAsync_MaxLevelLessThanQuantityOnHand_ReturnsConflictFailure()
    {
        var product = await CreateProductAsync(isActive: true);
        var locationId = await GetDefaultLocationIdAsync();

        var request = await BuildValidCreateRequestAsync(
            productId: product.Id,
            locationId: locationId,
            quantityOnHand: 50m,
            reorderLevel: 10m,
            maxLevel: 20m);

        var result = await InventoryService.CreateAsync(request, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorType.Conflict);
    }

    [Fact]
    public async Task GetAllAndFindAsync_ReturnExpectedSuccessAndFailureCases()
    {
        var noData = await InventoryService.GetAllAsync(CancellationToken.None);

        noData.IsSuccess.Should().BeFalse();
        noData.Error.Type.Should().Be(ErrorType.NotFound);

        var product = await CreateProductAsync(isActive: true);
        var locationId = await GetDefaultLocationIdAsync();
        var created = await InventoryService.CreateAsync(await BuildValidCreateRequestAsync(product.Id, locationId), CancellationToken.None);

        created.IsSuccess.Should().BeTrue();

        var getAll = await InventoryService.GetAllAsync(CancellationToken.None);
        var findInvalid = await InventoryService.FindAsync(0, CancellationToken.None);
        var findMissing = await InventoryService.FindAsync(999_999, CancellationToken.None);
        var findSuccess = await InventoryService.FindAsync(created.Value.Id, CancellationToken.None);

        getAll.IsSuccess.Should().BeTrue();
        getAll.Value.Should().ContainSingle(e => e.Id == created.Value.Id);

        findInvalid.IsSuccess.Should().BeFalse();
        findInvalid.Error.Type.Should().Be(ErrorType.Validation);

        findMissing.IsSuccess.Should().BeFalse();
        findMissing.Error.Type.Should().Be(ErrorType.NotFound);

        findSuccess.IsSuccess.Should().BeTrue();
        findSuccess.Value.Id.Should().Be(created.Value.Id);
    }

    [Fact]
    public async Task UpdateAsync_ValidRequest_UpdatesInventoryAndCreatesAdjustmentMovement()
    {
        var product = await CreateProductAsync(isActive: true);
        var locationId = await GetDefaultLocationIdAsync();

        var createResult = await InventoryService.CreateAsync(
            await BuildValidCreateRequestAsync(product.Id, locationId, 5m, 2m, 15m),
            CancellationToken.None);

        createResult.IsSuccess.Should().BeTrue();

        var updateRequest = new InventoryUpdateRequest
        {
            QuantityOnHand = 9m,
            ReorderLevel = 3m,
            MaxLevel = 20m,
        };

        var updateResult = await InventoryService.UpdateAsync(createResult.Value.Id, updateRequest, CancellationToken.None);

        updateResult.IsSuccess.Should().BeTrue();
        updateResult.Value.QuantityOnHand.Should().Be(9m);
        updateResult.Value.ReorderLevel.Should().Be(3m);
        updateResult.Value.MaxLevel.Should().Be(20m);

        AppDbContext.ChangeTracker.Clear();

        var persisted = await AppDbContext.Inventories.SingleAsync(e => e.Id == createResult.Value.Id);
        persisted.QuantityOnHand.Should().Be(9m);
        persisted.ReorderLevel.Should().Be(3m);
        persisted.MaxLevel.Should().Be(20m);

        var movements = await AppDbContext.StockMovements
            .Where(e => e.InventoryId == persisted.Id)
            .OrderBy(e => e.Id)
            .ToListAsync();

        movements.Should().HaveCount(2);
        movements.Last().MovementTypeId.Should().Be((int)StockMovementTypeEnum.StockIncreaseAdjustment);
        movements.Last().StockMovmentStatus.Should().Be(StockMovementStatus.Completed);
        movements.Last().Quantity.Should().Be(4m);
    }

    [Fact]
    public async Task UpdateAsync_DecreaseQuantity_CreatesDecreaseAdjustmentMovement()
    {
        var product = await CreateProductAsync(isActive: true);
        var locationId = await GetDefaultLocationIdAsync();

        var createResult = await InventoryService.CreateAsync(
            await BuildValidCreateRequestAsync(product.Id, locationId, 9m, 2m, 20m),
            CancellationToken.None);

        createResult.IsSuccess.Should().BeTrue();

        var updateRequest = new InventoryUpdateRequest
        {
            QuantityOnHand = 4m,
            ReorderLevel = 2m,
            MaxLevel = 20m,
        };

        var updateResult = await InventoryService.UpdateAsync(createResult.Value.Id, updateRequest, CancellationToken.None);

        updateResult.IsSuccess.Should().BeTrue();
        updateResult.Value.QuantityOnHand.Should().Be(4m);

        AppDbContext.ChangeTracker.Clear();

        var movements = await AppDbContext.StockMovements
            .Where(e => e.InventoryId == createResult.Value.Id)
            .OrderBy(e => e.Id)
            .ToListAsync();

        movements.Should().HaveCount(2);
        movements.Last().MovementTypeId.Should().Be((int)StockMovementTypeEnum.StockDecreaseAdjustment);
        movements.Last().StockMovmentStatus.Should().Be(StockMovementStatus.Completed);
        movements.Last().Quantity.Should().Be(5m);
    }

    [Fact]
    public async Task UpdateAsync_SameQuantity_DoesNotCreateNewAdjustmentMovement()
    {
        var product = await CreateProductAsync(isActive: true);
        var locationId = await GetDefaultLocationIdAsync();

        var createResult = await InventoryService.CreateAsync(
            await BuildValidCreateRequestAsync(product.Id, locationId, 5m, 2m, 20m),
            CancellationToken.None);

        createResult.IsSuccess.Should().BeTrue();

        var updateRequest = new InventoryUpdateRequest
        {
            QuantityOnHand = 5m,
            ReorderLevel = 3m,
            MaxLevel = 25m,
        };

        var updateResult = await InventoryService.UpdateAsync(createResult.Value.Id, updateRequest, CancellationToken.None);

        updateResult.IsSuccess.Should().BeTrue();
        updateResult.Value.ReorderLevel.Should().Be(3m);
        updateResult.Value.MaxLevel.Should().Be(25m);

        AppDbContext.ChangeTracker.Clear();

        var movements = await AppDbContext.StockMovements
            .Where(e => e.InventoryId == createResult.Value.Id)
            .OrderBy(e => e.Id)
            .ToListAsync();

        movements.Should().HaveCount(1);
        movements.Single().MovementTypeId.Should().Be((int)StockMovementTypeEnum.InitialStock);
    }

    [Fact]
    public async Task UpdateAsync_InvalidRequestInvalidIdAndMissingEntity_ReturnExpectedFailures()
    {
        var product = await CreateProductAsync(isActive: true);
        var locationId = await GetDefaultLocationIdAsync();
        var created = await InventoryService.CreateAsync(
            await BuildValidCreateRequestAsync(product.Id, locationId),
            CancellationToken.None);

        created.IsSuccess.Should().BeTrue();

        var validRequest = new InventoryUpdateRequest
        {
            QuantityOnHand = 5m,
            ReorderLevel = 1m,
            MaxLevel = 10m,
        };

        var invalidRequest = new InventoryUpdateRequest
        {
            QuantityOnHand = -1m,
            ReorderLevel = -1m,
            MaxLevel = -1m,
        };

        var invalidId = await InventoryService.UpdateAsync(0, validRequest, CancellationToken.None);
        var validation = await InventoryService.UpdateAsync(created.Value.Id, invalidRequest, CancellationToken.None);
        var missing = await InventoryService.UpdateAsync(999_999, validRequest, CancellationToken.None);

        invalidId.IsSuccess.Should().BeFalse();
        invalidId.Error.Type.Should().Be(ErrorType.Validation);

        validation.IsSuccess.Should().BeFalse();
        validation.Error.Type.Should().Be(ErrorType.Validation);

        missing.IsSuccess.Should().BeFalse();
        missing.Error.Type.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public async Task UpdateAsync_MaxLevelLessThanReorderLevel_ReturnsDomainFailure()
    {
        var product = await CreateProductAsync(isActive: true);
        var locationId = await GetDefaultLocationIdAsync();
        var created = await InventoryService.CreateAsync(
            await BuildValidCreateRequestAsync(product.Id, locationId, 6m, 2m, 20m),
            CancellationToken.None);

        created.IsSuccess.Should().BeTrue();

        var request = new InventoryUpdateRequest
        {
            QuantityOnHand = 6m,
            ReorderLevel = 10m,
            MaxLevel = 8m,
        };

        var result = await InventoryService.UpdateAsync(created.Value.Id, request, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Description.Should().Contain("Max level cannot be less than reorder level");
    }

    [Fact]
    public async Task DeleteByIdAsync_WithStockOnHand_ReturnsConflictFailure()
    {
        var product = await CreateProductAsync(isActive: true);
        var locationId = await GetDefaultLocationIdAsync();
        var created = await InventoryService.CreateAsync(
            await BuildValidCreateRequestAsync(product.Id, locationId, 8m, 2m, 20m),
            CancellationToken.None);

        created.IsSuccess.Should().BeTrue();

        var result = await InventoryService.DeleteByIdAsync(created.Value.Id, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorType.Conflict);
    }

    [Fact]
    public async Task DeleteByIdAsync_ZeroStock_SoftDeletesInventory()
    {
        var product = await CreateProductAsync(isActive: true);
        var locationId = await GetDefaultLocationIdAsync();
        var created = await InventoryService.CreateAsync(
            await BuildValidCreateRequestAsync(product.Id, locationId, 0m, 2m, 20m),
            CancellationToken.None);

        created.IsSuccess.Should().BeTrue();

        var result = await InventoryService.DeleteByIdAsync(created.Value.Id, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();

        AppDbContext.ChangeTracker.Clear();

        var deleted = await AppDbContext.Inventories
            .IgnoreQueryFilters()
            .SingleAsync(e => e.Id == created.Value.Id);

        deleted.IsDeleted.Should().BeTrue();
        deleted.DeletedAt.Should().NotBeNull();
        deleted.DeletedByUserId.Should().Be(TestUserId);
    }

    [Fact]
    public async Task DeleteByIdAsync_MissingInventory_ReturnsNotFoundFailure()
    {
        var result = await InventoryService.DeleteByIdAsync(999_999, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public async Task DeleteAsync_HardDelete_ReturnsExpectedLifecycleResults()
    {
        var product = await CreateProductAsync(isActive: true);
        var locationId = await GetDefaultLocationIdAsync();
        var directInventory = await CreateInventoryDirectAsync(
            product.Id,
            locationId,
            quantityOnHand: 0m,
            reorderLevel: 0m,
            maxLevel: 10m);

        var invalid = await InventoryService.DeleteAsync(0, CancellationToken.None);
        var success = await InventoryService.DeleteAsync(directInventory.Id, CancellationToken.None);
        var notFound = await InventoryService.DeleteAsync(directInventory.Id, CancellationToken.None);

        invalid.IsSuccess.Should().BeFalse();
        invalid.Error.Type.Should().Be(ErrorType.Validation);

        success.IsSuccess.Should().BeTrue();

        notFound.IsSuccess.Should().BeFalse();
        notFound.Error.Type.Should().Be(ErrorType.NotFound);

        (await AppDbContext.Inventories.AnyAsync(e => e.Id == directInventory.Id)).Should().BeFalse();
    }

    [Fact]
    public async Task GetInventoryLowStockAsync_ReturnsExpectedNotFoundAndSuccessCases()
    {
        var noData = await InventoryService.GetInventoryLowStockAsync(CancellationToken.None);
        noData.IsSuccess.Should().BeFalse();
        noData.Error.Type.Should().Be(ErrorType.NotFound);

        var locationId = await GetDefaultLocationIdAsync();

        var healthyProduct = await CreateProductAsync(isActive: true);
        await CreateInventoryDirectAsync(healthyProduct.Id, locationId, quantityOnHand: 10m, reorderLevel: 2m, maxLevel: 20m);

        var noLowStock = await InventoryService.GetInventoryLowStockAsync(CancellationToken.None);
        noLowStock.IsSuccess.Should().BeFalse();
        noLowStock.Error.Type.Should().Be(ErrorType.NotFound);

        var lowProduct = await CreateProductAsync(isActive: true);
        var lowInventory = await CreateInventoryDirectAsync(lowProduct.Id, locationId, quantityOnHand: 2m, reorderLevel: 2m, maxLevel: 20m);

        var success = await InventoryService.GetInventoryLowStockAsync(CancellationToken.None);

        success.IsSuccess.Should().BeTrue();
        success.Value.Should().Contain(e => e.Id == lowInventory.Id && e.QuantityOnHand == 2m);
    }

    [Fact]
    public async Task GetInventoryValuationAndCostAsync_ReturnExpectedTotals()
    {
        var locationId = await GetDefaultLocationIdAsync();

        var firstProduct = await CreateProductAsync(unitPrice: 12m, cost: 8m);
        var secondProduct = await CreateProductAsync(unitPrice: 5m, cost: 3m);

        await CreateInventoryDirectAsync(firstProduct.Id, locationId, quantityOnHand: 10m, reorderLevel: 1m, maxLevel: 20m);
        await CreateInventoryDirectAsync(secondProduct.Id, locationId, quantityOnHand: 6m, reorderLevel: 1m, maxLevel: 20m);

        var valuation = await InventoryService.GetInventoryValuationAsync(CancellationToken.None);
        var cost = await InventoryService.GetInventoryCostAsync(CancellationToken.None);

        valuation.IsSuccess.Should().BeTrue();
        cost.IsSuccess.Should().BeTrue();

        valuation.Value.Should().Be(150m);
        cost.Value.Should().Be(98m);
    }

    [Fact]
    public async Task InventoryQueries_GetInventoryTableAsync_ReturnsExpectedPagedResults()
    {
        var empty = await InventoryQueries.GetInventoryTableAsync(new TableRequest
        {
            Page = 1,
            PageSize = 10,
        }, CancellationToken.None);

        empty.IsSuccess.Should().BeFalse();
        empty.Error.Type.Should().Be(ErrorType.NotFound);

        var locationId = await GetDefaultLocationIdAsync();

        var firstProduct = await CreateProductAsync(sku: "INV-TBL-A", name: "Inventory-IT-Product-Table-A", unitPrice: 20m, cost: 14m);
        var secondProduct = await CreateProductAsync(sku: "INV-TBL-B", name: "Inventory-IT-Product-Table-B", unitPrice: 30m, cost: 24m);

        await CreateInventoryDirectAsync(firstProduct.Id, locationId, quantityOnHand: 4m, reorderLevel: 1m, maxLevel: 10m);
        await CreateInventoryDirectAsync(secondProduct.Id, locationId, quantityOnHand: 7m, reorderLevel: 2m, maxLevel: 15m);

        var success = await InventoryQueries.GetInventoryTableAsync(new TableRequest
        {
            Page = 1,
            PageSize = 10,
            SortColumn = "sku",
            SortOrder = "asc",
            search = "INV-TBL-",
        }, CancellationToken.None);

        var outOfRange = await InventoryQueries.GetInventoryTableAsync(new TableRequest
        {
            Page = 99,
            PageSize = 10,
        }, CancellationToken.None);

        success.IsSuccess.Should().BeTrue();
        success.Value.TotalCount.Should().Be(2);
        success.Value.Item.Should().HaveCount(2);
        success.Value.Item.Select(e => e.Sku).Should().Contain(["INV-TBL-A", "INV-TBL-B"]);

        outOfRange.IsSuccess.Should().BeFalse();
        outOfRange.Error.Type.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public async Task InventoryQueries_GetInventoryTableAsync_MapsDerivedFieldsAndDefaultSort()
    {
        var locationId = await GetDefaultLocationIdAsync();

        var outProduct = await CreateProductAsync(sku: "INV-DER-OUT", unitPrice: 14m, cost: 9m);
        var lowProduct = await CreateProductAsync(sku: "INV-DER-LOW", unitPrice: 11m, cost: 8m);
        var inProduct = await CreateProductAsync(sku: "INV-DER-IN", unitPrice: 15m, cost: 10m);

        await CreateInventoryDirectAsync(outProduct.Id, locationId, quantityOnHand: 0m, reorderLevel: 1m, maxLevel: 10m);
        await CreateInventoryDirectAsync(lowProduct.Id, locationId, quantityOnHand: 2m, reorderLevel: 2m, maxLevel: 8m);
        await CreateInventoryDirectAsync(inProduct.Id, locationId, quantityOnHand: 9m, reorderLevel: 2m, maxLevel: 12m);

        var result = await InventoryQueries.GetInventoryTableAsync(new TableRequest
        {
            Page = 1,
            PageSize = 10,
            SortColumn = "unsupported-column",
            SortOrder = "asc",
            search = "INV-DER-",
        }, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.TotalCount.Should().Be(3);
        result.Value.Item.Should().HaveCount(3);

        var items = result.Value.Item.ToDictionary(e => e.Sku);

        items["INV-DER-OUT"].Status.Should().Be("Out Of Stock");
        items["INV-DER-OUT"].StockPercentage.Should().Be(0d);
        items["INV-DER-OUT"].PotentialProfit.Should().Be(0m);

        items["INV-DER-LOW"].Status.Should().Be("Low Stock");
        items["INV-DER-LOW"].StockPercentage.Should().Be(25d);
        items["INV-DER-LOW"].PotentialProfit.Should().Be(6m);

        items["INV-DER-IN"].Status.Should().Be("In Stock");
        items["INV-DER-IN"].StockPercentage.Should().Be(75d);
        items["INV-DER-IN"].PotentialProfit.Should().Be(45m);

        result.Value.Item.Select(e => e.Id).Should().BeInAscendingOrder();
    }

    [Fact]
    public async Task InventoryQueries_GetInventorySummaryAndById_ReturnExpectedResults()
    {
        var locationId = await GetDefaultLocationIdAsync();

        var outProduct = await CreateProductAsync(unitPrice: 10m, cost: 7m);
        var lowProduct = await CreateProductAsync(unitPrice: 8m, cost: 5m);

        var outInventory = await CreateInventoryDirectAsync(outProduct.Id, locationId, quantityOnHand: 0m, reorderLevel: 1m, maxLevel: 5m);
        var lowInventory = await CreateInventoryDirectAsync(lowProduct.Id, locationId, quantityOnHand: 2m, reorderLevel: 5m, maxLevel: 20m);

        var summary = await InventoryQueries.GetInventorySummaryAsync(CancellationToken.None);
        var invalidId = await InventoryQueries.GetByIdAsync(0, CancellationToken.None);
        var notFound = await InventoryQueries.GetByIdAsync(999_999, CancellationToken.None);
        var byId = await InventoryQueries.GetByIdAsync(lowInventory.Id, CancellationToken.None);

        summary.IsSuccess.Should().BeTrue();

        var summaryObject = summary.Value;
        var lowStockItems = (int)summaryObject.GetType().GetProperty("LowStockItems")!.GetValue(summaryObject)!;
        var outOfStockItems = (int)summaryObject.GetType().GetProperty("OutOfStockItems")!.GetValue(summaryObject)!;
        var totalInventoryItems = (int)summaryObject.GetType().GetProperty("TotalInventoryItems")!.GetValue(summaryObject)!;
        var totalPotentialProfit = (decimal)summaryObject.GetType().GetProperty("TotalPotentialProfit")!.GetValue(summaryObject)!;

        lowStockItems.Should().Be(2);
        outOfStockItems.Should().Be(1);
        totalInventoryItems.Should().Be(2);
        totalPotentialProfit.Should().Be(6m);

        invalidId.IsSuccess.Should().BeFalse();
        invalidId.Error.Type.Should().Be(ErrorType.Validation);

        notFound.IsSuccess.Should().BeFalse();
        notFound.Error.Type.Should().Be(ErrorType.NotFound);

        byId.IsSuccess.Should().BeTrue();

        var byIdObject = byId.Value;
        var quantityOnHand = (decimal)byIdObject.GetType().GetProperty("QuantityOnHand")!.GetValue(byIdObject)!;
        var productObject = byIdObject.GetType().GetProperty("Product")!.GetValue(byIdObject)!;
        var productName = (string)productObject.GetType().GetProperty("Name")!.GetValue(productObject)!;
        var locationObject = byIdObject.GetType().GetProperty("Location")!.GetValue(byIdObject)!;
        var locationName = (string)locationObject.GetType().GetProperty("Name")!.GetValue(locationObject)!;

        quantityOnHand.Should().Be(2m);
        productName.Should().Be(lowProduct.Name);
        locationName.Should().Be(IntegrationTestWebAppFactory.DefaultLocationName);

        outInventory.Id.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task InventoryQueries_GetLowStockMessageDetails_ReturnExpectedResults()
    {
        var invalid = await InventoryQueries.GetLowStockMessageDetailsAsync(0, 0, CancellationToken.None);
        var missing = await InventoryQueries.GetLowStockMessageDetailsAsync(999_999, 999_999, CancellationToken.None);

        invalid.IsSuccess.Should().BeFalse();
        invalid.Error.Type.Should().Be(ErrorType.Validation);

        missing.IsSuccess.Should().BeFalse();
        missing.Error.Type.Should().Be(ErrorType.NotFound);

        var product = await CreateProductAsync();
        var locationId = await GetDefaultLocationIdAsync();
        await CreateInventoryDirectAsync(product.Id, locationId, quantityOnHand: 1m, reorderLevel: 5m, maxLevel: 20m);

        var success = await InventoryQueries.GetLowStockMessageDetailsAsync(product.Id, locationId, CancellationToken.None);

        success.IsSuccess.Should().BeTrue();
        success.Value.productName.Should().Be(product.Name);
        success.Value.locationName.Should().Be(IntegrationTestWebAppFactory.DefaultLocationName);
        success.Value.unitOfMeasureName.Should().Be(IntegrationTestWebAppFactory.DefaultUnitOfMeasureName);
    }

    [Fact]
    public async Task InventoryQueries_GetStockTransfersAsync_ReturnExpectedResults()
    {
        var empty = await InventoryQueries.GetStockTransfersAsync(new TableRequest
        {
            Page = 1,
            PageSize = 10,
        }, CancellationToken.None);

        empty.IsSuccess.Should().BeFalse();
        empty.Error.Type.Should().Be(ErrorType.NotFound);

        var product = await CreateProductAsync();
        var fromLocationId = await GetDefaultLocationIdAsync();
        var toLocation = await CreateAdditionalLocationAsync();

        await CreateStockTransferAsync(product.Id, fromLocationId, toLocation.Id, quantity: 3m);

        var success = await InventoryQueries.GetStockTransfersAsync(new TableRequest
        {
            Page = 1,
            PageSize = 10,
            SortColumn = "createdAt",
            SortOrder = "desc",
        }, CancellationToken.None);

        var outOfRange = await InventoryQueries.GetStockTransfersAsync(new TableRequest
        {
            Page = 99,
            PageSize = 10,
        }, CancellationToken.None);

        success.IsSuccess.Should().BeTrue();
        success.Value.TotalCount.Should().Be(1);
        success.Value.Item.Should().ContainSingle();
        success.Value.Item.Single().FromLocation.Should().Be(IntegrationTestWebAppFactory.DefaultLocationName);
        success.Value.Item.Single().ToLocation.Should().Be(toLocation.Name);
        success.Value.Item.Single().Product.Should().Be(product.Name);
        success.Value.Item.Single().Quantity.Should().Be(3m);

        outOfRange.IsSuccess.Should().BeFalse();
        outOfRange.Error.Type.Should().Be(ErrorType.NotFound);
    }
}