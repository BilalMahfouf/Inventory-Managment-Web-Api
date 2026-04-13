using Application.IntegrationTests.Common;
using Application.Shared.Paging;
using Domain.Shared.Errors;
using FluentAssertions;

namespace Application.IntegrationTests.Services;

public sealed class TransferQueriesTests : StockMovementFeaturesIntegrationTestBase
{
    public TransferQueriesTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task GetByIdAsync_InvalidId_ReturnsValidationFailure()
    {
        var result = await TransferQueries.GetByIdAsync(0, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorType.Validation);
    }

    [Fact]
    public async Task GetByIdAsync_MissingTransfer_ReturnsNotFoundFailure()
    {
        var result = await TransferQueries.GetByIdAsync(999_999, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public async Task GetByIdAsync_ExistingTransfer_ReturnsProjectedTransferDetails()
    {
        var product = await CreateProductAsync();
        var fromLocationId = await GetDefaultLocationIdAsync();
        var toLocation = await CreateAdditionalLocationAsync();

        var transfer = await CreateStockTransferDirectAsync(product.Id, fromLocationId, toLocation.Id, quantity: 3m);

        var result = await TransferQueries.GetByIdAsync(transfer.Id, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();

        var payload = result.Value;
        GetValue<int>(payload, "Id").Should().Be(transfer.Id);
        GetValue<int>(payload, "FromLocationId").Should().Be(fromLocationId);
        GetValue<string>(payload, "FromLocationName").Should().Be(IntegrationTestWebAppFactory.DefaultLocationName);
        GetValue<int>(payload, "ToLocationId").Should().Be(toLocation.Id);
        GetValue<string>(payload, "ToLocationName").Should().Be(toLocation.Name);
        GetValue<int>(payload, "ProdcutId").Should().Be(product.Id);
        GetValue<string>(payload, "ProductName").Should().Be(product.Name);
        GetValue<decimal>(payload, "Quantity").Should().Be(3m);
        GetValue<string>(payload, "Status").Should().Be("Pending");
    }

    [Fact]
    public async Task GetStockTransfersAsync_EmptyDataSet_ReturnsNotFoundFailure()
    {
        var result = await InventoryQueries.GetStockTransfersAsync(new TableRequest
        {
            Page = 1,
            PageSize = 10,
        }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public async Task GetStockTransfersAsync_SortByQuantityDescAndPaginate_ReturnsExpectedOrderAndPages()
    {
        var product = await CreateProductAsync();
        var fromLocationId = await GetDefaultLocationIdAsync();
        var toLocation = await CreateAdditionalLocationAsync();

        await CreateStockTransferDirectAsync(product.Id, fromLocationId, toLocation.Id, quantity: 1m);
        await CreateStockTransferDirectAsync(product.Id, fromLocationId, toLocation.Id, quantity: 3m);
        await CreateStockTransferDirectAsync(product.Id, fromLocationId, toLocation.Id, quantity: 2m);

        var firstPage = await InventoryQueries.GetStockTransfersAsync(new TableRequest
        {
            Page = 1,
            PageSize = 2,
            SortColumn = "quantity",
            SortOrder = "desc",
        }, CancellationToken.None);

        var secondPage = await InventoryQueries.GetStockTransfersAsync(new TableRequest
        {
            Page = 2,
            PageSize = 2,
            SortColumn = "quantity",
            SortOrder = "desc",
        }, CancellationToken.None);

        firstPage.IsSuccess.Should().BeTrue();
        firstPage.Value.TotalCount.Should().Be(3);
        firstPage.Value.Page.Should().Be(1);
        firstPage.Value.PageSize.Should().Be(2);
        firstPage.Value.Item.Should().HaveCount(2);
        firstPage.Value.Item.Select(e => e.Quantity).Should().ContainInOrder([3m, 2m]);

        secondPage.IsSuccess.Should().BeTrue();
        secondPage.Value.TotalCount.Should().Be(3);
        secondPage.Value.Item.Should().ContainSingle();
        secondPage.Value.Item.Single().Quantity.Should().Be(1m);
    }

    [Fact]
    public async Task GetStockTransfersAsync_PageOutOfRange_ReturnsNotFoundFailure()
    {
        var product = await CreateProductAsync();
        var fromLocationId = await GetDefaultLocationIdAsync();
        var toLocation = await CreateAdditionalLocationAsync();

        await CreateStockTransferDirectAsync(product.Id, fromLocationId, toLocation.Id, quantity: 2m);

        var result = await InventoryQueries.GetStockTransfersAsync(new TableRequest
        {
            Page = 99,
            PageSize = 10,
            SortColumn = "createdAt",
            SortOrder = "desc",
        }, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorType.NotFound);
    }

    private static TProperty GetValue<TProperty>(object source, string propertyName)
    {
        var propertyInfo = source.GetType().GetProperty(propertyName);
        propertyInfo.Should().NotBeNull();
        return (TProperty)propertyInfo!.GetValue(source)!;
    }
}
