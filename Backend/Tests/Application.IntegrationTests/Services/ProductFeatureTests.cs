using Application.IntegrationTests.Common;
using Application.Products.DTOs.Request.Products;
using Application.Shared.Paging;
using Domain.Shared.Errors;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Application.IntegrationTests.Services;

public sealed class ProductFeatureTests : ProductFeaturesIntegrationTestBase
{
    public ProductFeatureTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task CreateAsync_ValidRequest_PersistsProductAndInventory()
    {
        await AssertBaselineSeedIsAvailableAsync();
        var request = await BuildValidProductCreateRequestAsync();

        var result = await ProductService.CreateAsync(request);

        result.IsSuccess.Should().BeTrue();
        result.Value.SKU.Should().Be(request.SKU);
        result.Value.Name.Should().Be(request.Name);
        result.Value.IsActive.Should().BeTrue();

        var persistedProduct = await AppDbContext.Products
            .SingleAsync(e => e.Id == result.Value.Id);
        persistedProduct.Sku.Should().Be(request.SKU);
        persistedProduct.CategoryId.Should().Be(request.CategoryId);

        var persistedInventory = await AppDbContext.Inventories
            .SingleAsync(e => e.ProductId == persistedProduct.Id && e.LocationId == request.LocationId);
        persistedInventory.QuantityOnHand.Should().Be(request.QuantityOnHand);
        persistedInventory.ReorderLevel.Should().Be(request.ReorderLevel);
        persistedInventory.MaxLevel.Should().Be(request.MaxLevel);

        (await AppDbContext.StockMovements.AnyAsync(e => e.ProductId == persistedProduct.Id)).Should().BeTrue();
    }

    [Fact]
    public async Task CreateAsync_DuplicateSku_ReturnsConflictFailure()
    {
        var sku = $"SKU-{Guid.NewGuid().ToString("N")[..10]}";
        var first = await BuildValidProductCreateRequestAsync(sku: sku);
        var second = await BuildValidProductCreateRequestAsync(sku: sku, name: "Second product");

        var firstResult = await ProductService.CreateAsync(first);
        var secondResult = await ProductService.CreateAsync(second);

        firstResult.IsSuccess.Should().BeTrue();
        secondResult.IsSuccess.Should().BeFalse();
        secondResult.Error.Type.Should().Be(ErrorType.Conflict);
    }

    [Fact]
    public async Task CreateAsync_InvalidRequest_ReturnsValidationFailure()
    {
        var request = await BuildValidProductCreateRequestAsync();
        request = request with
        {
            SKU = string.Empty,
            Name = string.Empty,
            CategoryId = 0,
        };

        var result = await ProductService.CreateAsync(request);

        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorType.Validation);
    }

    [Fact]
    public async Task CreateAsync_CostGreaterThanUnitPrice_ReturnsConflictFailure()
    {
        var request = await BuildValidProductCreateRequestAsync(unitPrice: 10m, costPrice: 15m);

        var result = await ProductService.CreateAsync(request);

        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorType.Conflict);
    }

    [Fact]
    public async Task CreateAsync_MaxLevelLessThanQuantityOnHand_ReturnsConflictFailure()
    {
        var request = await BuildValidProductCreateRequestAsync(quantityOnHand: 20m, maxLevel: 10m);

        var result = await ProductService.CreateAsync(request);

        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorType.Conflict);
    }

    [Fact]
    public async Task UpdateAsync_ValidRequest_UpdatesProductAndInventory()
    {
        var (product, inventory) = await CreateProductInventoryAsync(quantityOnHand: 12m, unitPrice: 20m, costPrice: 10m);
        var updateRequest = new ProductUpdateRequest
        {
            Name = "Updated product",
            Description = "Updated description",
            CategoryId = await GetDefaultCategoryIdAsync(),
            UnitPrice = 30m,
            CostPrice = 12m,
            LocationId = inventory.LocationId,
            QuantityOnHand = 18m,
            ReorderLevel = 2m,
            MaxLevel = 40m,
        };

        var result = await ProductService.UpdateAsync(product.Id, updateRequest);

        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().Be("Updated product");
        result.Value.UnitPrice.Should().Be(30m);

        AppDbContext.ChangeTracker.Clear();
        var updatedProduct = await AppDbContext.Products.SingleAsync(e => e.Id == product.Id);
        var updatedInventory = await AppDbContext.Inventories
            .SingleAsync(e => e.ProductId == product.Id && e.LocationId == inventory.LocationId);

        updatedProduct.Name.Should().Be("Updated product");
        updatedProduct.Cost.Should().Be(12m);
        updatedInventory.QuantityOnHand.Should().Be(18m);
        updatedInventory.ReorderLevel.Should().Be(2m);
        updatedInventory.MaxLevel.Should().Be(40m);
    }

    [Fact]
    public async Task UpdateAsync_InvalidId_ReturnsValidationFailure()
    {
        var request = new ProductUpdateRequest
        {
            Name = "Updated product",
            Description = "Updated",
            CategoryId = await GetDefaultCategoryIdAsync(),
            UnitPrice = 10m,
            CostPrice = 5m,
            LocationId = await GetDefaultLocationIdAsync(),
            QuantityOnHand = 10m,
            ReorderLevel = 2m,
            MaxLevel = 15m,
        };

        var result = await ProductService.UpdateAsync(0, request);

        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorType.Validation);
    }

    [Fact]
    public async Task UpdateAsync_MissingProduct_ReturnsNotFoundFailure()
    {
        var request = new ProductUpdateRequest
        {
            Name = "Updated product",
            Description = "Updated",
            CategoryId = await GetDefaultCategoryIdAsync(),
            UnitPrice = 10m,
            CostPrice = 5m,
            LocationId = await GetDefaultLocationIdAsync(),
            QuantityOnHand = 10m,
            ReorderLevel = 2m,
            MaxLevel = 15m,
        };

        var result = await ProductService.UpdateAsync(999_999, request);

        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public async Task UpdateAsync_MissingInventoryForLocation_ReturnsNotFoundFailure()
    {
        var (product, _) = await CreateProductInventoryAsync();
        var request = new ProductUpdateRequest
        {
            Name = "Updated product",
            Description = "Updated",
            CategoryId = await GetDefaultCategoryIdAsync(),
            UnitPrice = 10m,
            CostPrice = 5m,
            LocationId = 999_999,
            QuantityOnHand = 10m,
            ReorderLevel = 2m,
            MaxLevel = 15m,
        };

        var result = await ProductService.UpdateAsync(product.Id, request);

        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public async Task UpdateAsync_InvalidPricing_ReturnsValidationFailure()
    {
        var (product, inventory) = await CreateProductInventoryAsync();
        var request = new ProductUpdateRequest
        {
            Name = "Updated product",
            Description = "Updated",
            CategoryId = await GetDefaultCategoryIdAsync(),
            UnitPrice = 10m,
            CostPrice = 12m,
            LocationId = inventory.LocationId,
            QuantityOnHand = 10m,
            ReorderLevel = 2m,
            MaxLevel = 15m,
        };

        var result = await ProductService.UpdateAsync(product.Id, request);

        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorType.Validation);
    }

    [Fact]
    public async Task UpdateAsync_MaxLevelLessThanReorderLevel_ReturnsConflictFailure()
    {
        var (product, inventory) = await CreateProductInventoryAsync();
        var request = new ProductUpdateRequest
        {
            Name = "Updated product",
            Description = "Updated",
            CategoryId = await GetDefaultCategoryIdAsync(),
            UnitPrice = 10m,
            CostPrice = 5m,
            LocationId = inventory.LocationId,
            QuantityOnHand = 10m,
            ReorderLevel = 20m,
            MaxLevel = 15m,
        };

        var result = await ProductService.UpdateAsync(product.Id, request);

        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorType.Conflict);
    }

    [Fact]
    public async Task ActivateDeactivateDeleteLifecycle_WorksWithExpectedFailures()
    {
        var (product, _) = await CreateProductInventoryAsync(isActive: true);

        var deactivate = await ProductService.DeactivateAsync(product.Id);
        var deactivateAgain = await ProductService.DeactivateAsync(product.Id);
        var activate = await ProductService.ActivateAsync(product.Id);
        var activateAgain = await ProductService.ActivateAsync(product.Id);
        var delete = await ProductService.DeleteAsync(product.Id);
        var deleteAgain = await ProductService.DeleteAsync(product.Id);

        deactivate.IsSuccess.Should().BeTrue();
        deactivateAgain.IsSuccess.Should().BeFalse();
        deactivateAgain.Error.Type.Should().Be(ErrorType.Conflict);

        activate.IsSuccess.Should().BeTrue();
        activateAgain.IsSuccess.Should().BeFalse();
        activateAgain.Error.Type.Should().Be(ErrorType.Conflict);

        delete.IsSuccess.Should().BeTrue();
        deleteAgain.IsSuccess.Should().BeFalse();
        deleteAgain.Error.Type.Should().Be(ErrorType.NotFound);

        var deletedEntity = await AppDbContext.Products
            .IgnoreQueryFilters()
            .SingleAsync(e => e.Id == product.Id);
        deletedEntity.IsDeleted.Should().BeTrue();
        deletedEntity.DeletedByUserId.Should().Be(TestUserId);
    }

    [Fact]
    public async Task ActivateDeactivateDelete_WithInvalidId_ReturnsValidationFailure()
    {
        var deactivate = await ProductService.DeactivateAsync(0);
        var activate = await ProductService.ActivateAsync(0);
        var delete = await ProductService.DeleteAsync(0);

        deactivate.IsSuccess.Should().BeFalse();
        deactivate.Error.Type.Should().Be(ErrorType.Validation);

        activate.IsSuccess.Should().BeFalse();
        activate.Error.Type.Should().Be(ErrorType.Validation);

        delete.IsSuccess.Should().BeFalse();
        delete.Error.Type.Should().Be(ErrorType.Validation);
    }

    [Fact]
    public async Task GetAllAsync_WhenProductsExist_ReturnsPagedProducts()
    {
        await CreateProductInventoryAsync();
        await CreateProductInventoryAsync();

        var result = await ProductService.GetAllAsync(page: 1, pageSize: 10, search: null);

        result.IsSuccess.Should().BeTrue();
        result.Value.Item.Should().NotBeEmpty();
        result.Value.Page.Should().Be(1);
    }

    [Fact]
    public async Task FindProductInInventoryAsync_ValidAndInvalidCases_ReturnExpectedResults()
    {
        var (product, _) = await CreateProductInventoryAsync(quantityOnHand: 9m);

        var success = await ProductService.FindProductInInventoryAsync(product.Id);
        var invalidId = await ProductService.FindProductInInventoryAsync(0);
        var notFound = await ProductService.FindProductInInventoryAsync(999_999);

        success.IsSuccess.Should().BeTrue();
        success.Value.Should().ContainSingle(e => e.ProductId == product.Id);

        invalidId.IsSuccess.Should().BeFalse();
        invalidId.Error.Type.Should().Be(ErrorType.Validation);

        notFound.IsSuccess.Should().BeFalse();
        notFound.Error.Type.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public async Task GetProductsWithLowStockAsync_ReturnsExpectedSuccessAndNotFound()
    {
        var notFound = await ProductService.GetProductsWithLowStockAsync();
        notFound.IsSuccess.Should().BeFalse();
        notFound.Error.Type.Should().Be(ErrorType.NotFound);

        var (product, inventory) = await CreateProductInventoryAsync(quantityOnHand: 2m);
        inventory.ReorderLevel = 5m;
        AppDbContext.Inventories.Update(inventory);
        await AppDbContext.SaveChangesAsync();

        var success = await ProductService.GetProductsWithLowStockAsync();
        success.IsSuccess.Should().BeTrue();
        success.Value.Should().Contain(e => e.ProductId == product.Id && e.LocationId == inventory.LocationId);
    }

    [Fact]
    public async Task FindProductSuppliersAsync_ValidAndInvalidCases_ReturnExpectedResults()
    {
        var invalidId = await ProductService.FindProductSuppliersAsync(0);
        invalidId.IsSuccess.Should().BeFalse();
        invalidId.Error.Type.Should().Be(ErrorType.Validation);

        var (product, _) = await CreateProductInventoryAsync();

        var notFound = await ProductService.FindProductSuppliersAsync(product.Id);
        notFound.IsSuccess.Should().BeFalse();
        notFound.Error.Type.Should().Be(ErrorType.NotFound);

        await CreateProductSupplierAsync(product.Id);
        var success = await ProductService.FindProductSuppliersAsync(product.Id);

        success.IsSuccess.Should().BeTrue();
        success.Value.Should().ContainSingle(e => e.ProductId == product.Id);
    }

    [Fact]
    public async Task ProductQueries_GetByIdAsync_ReturnsExpectedSuccessAndNotFound()
    {
        var createRequest = await BuildValidProductCreateRequestAsync();
        var createResult = await ProductService.CreateAsync(createRequest);

        createResult.IsSuccess.Should().BeTrue();

        var success = await ProductQueries.GetByIdAsync(createResult.Value.Id);
        var notFound = await ProductQueries.GetByIdAsync(999_999);

        success.IsSuccess.Should().BeTrue();
        success.Value.Id.Should().Be(createResult.Value.Id);
        success.Value.SKU.Should().Be(createRequest.SKU);

        notFound.IsSuccess.Should().BeFalse();
        notFound.Error.Type.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public async Task ProductQueries_GetAllAsync_ReturnsExpectedPagedResults()
    {
        var firstRequest = await BuildValidProductCreateRequestAsync();
        var secondRequest = await BuildValidProductCreateRequestAsync();

        (await ProductService.CreateAsync(firstRequest)).IsSuccess.Should().BeTrue();
        (await ProductService.CreateAsync(secondRequest)).IsSuccess.Should().BeTrue();

        var success = await ProductQueries.GetAllAsync(new TableRequest
        {
            Page = 1,
            PageSize = 10,
            SortColumn = "sku",
            SortOrder = "asc",
        });

        var outOfRange = await ProductQueries.GetAllAsync(new TableRequest
        {
            Page = 99,
            PageSize = 10,
        });

        success.IsSuccess.Should().BeTrue();
        success.Value.Item.Should().NotBeEmpty();
        success.Value.TotalCount.Should().BeGreaterThanOrEqualTo(2);

        outOfRange.IsSuccess.Should().BeFalse();
        outOfRange.Error.Type.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public async Task ProductQueries_GetProductDashboardSummaryAsync_ReturnsAggregateObject()
    {
        var request = await BuildValidProductCreateRequestAsync(unitPrice: 15m, costPrice: 10m, quantityOnHand: 7m);
        (await ProductService.CreateAsync(request)).IsSuccess.Should().BeTrue();

        var result = await ProductQueries.GetProductDashboardSummaryAsync();

        result.IsSuccess.Should().BeTrue();

        var summary = result.Value;
        var totalProducts = (int)summary.GetType().GetProperty("TotalProducts")!.GetValue(summary)!;
        var inventoryValue = (decimal)summary.GetType().GetProperty("InventoryValue")!.GetValue(summary)!;

        totalProducts.Should().BeGreaterThan(0);
        inventoryValue.Should().BeGreaterThan(0m);
    }

    [Fact]
    public async Task ProductQueries_GetStockMovementsHistoryAsync_ReturnsExpectedSuccessAndNotFound()
    {
        var noData = await ProductQueries.GetStockMovementsHistoryAsync(new TableRequest
        {
            Page = 1,
            PageSize = 10,
        });

        noData.IsSuccess.Should().BeFalse();
        noData.Error.Type.Should().Be(ErrorType.NotFound);

        var request = await BuildValidProductCreateRequestAsync(quantityOnHand: 9m);
        (await ProductService.CreateAsync(request)).IsSuccess.Should().BeTrue();

        var success = await ProductQueries.GetStockMovementsHistoryAsync(new TableRequest
        {
            Page = 1,
            PageSize = 10,
            SortColumn = "createdAt",
            SortOrder = "desc",
        });

        success.IsSuccess.Should().BeTrue();
        success.Value.Item.Should().NotBeEmpty();
    }
}