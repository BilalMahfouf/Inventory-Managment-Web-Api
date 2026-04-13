using Application.IntegrationTests.Common;
using Domain.Inventories.Enums;
using Domain.Shared.Errors;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Application.IntegrationTests.Services;

public sealed class StockMovementTypeServiceTests : StockMovementFeaturesIntegrationTestBase
{
    public StockMovementTypeServiceTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task AddAsync_ValidRequest_PersistsAndReturnsCreatedType()
    {
        await AssertBaselineSeedIsAvailableAsync();
        var request = BuildValidStockMovementTypeRequest(direction: (byte)StockMovementDirection.In);

        var result = await StockMovementTypeService.AddAsync(request, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().Be(request.Name);
        result.Value.Description.Should().Be(request.Description);

        AppDbContext.ChangeTracker.Clear();

        var persisted = await AppDbContext.StockMovementTypes
            .IgnoreQueryFilters()
            .SingleAsync(e => e.Id == result.Value.Id);

        persisted.Name.Should().Be(request.Name);
        persisted.Direction.Should().Be(StockMovementDirection.In);
        persisted.IsDeleted.Should().BeFalse();
    }

    [Fact]
    public async Task AddAsync_MaxLengthNameBoundary_ReturnsSuccess()
    {
        const int maxLength = 100;
        var request = BuildValidStockMovementTypeRequest(
            name: new string('A', maxLength),
            direction: (byte)StockMovementDirection.Transfer);

        var result = await StockMovementTypeService.AddAsync(request, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Length.Should().Be(maxLength);
    }

    [Fact]
    public async Task AddAsync_NameLongerThanMaxLength_ReturnsValidationFailure()
    {
        var request = BuildValidStockMovementTypeRequest(
            name: new string('B', 101),
            direction: (byte)StockMovementDirection.Out);

        var result = await StockMovementTypeService.AddAsync(request, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorType.Validation);
    }

    [Fact]
    public async Task AddAsync_DuplicateName_ReturnsValidationFailure()
    {
        var name = GenerateStockMovementTypeName();
        var firstRequest = BuildValidStockMovementTypeRequest(name: name);
        var secondRequest = BuildValidStockMovementTypeRequest(name: name, description: "duplicate");

        var first = await StockMovementTypeService.AddAsync(firstRequest, CancellationToken.None);
        var second = await StockMovementTypeService.AddAsync(secondRequest, CancellationToken.None);

        first.IsSuccess.Should().BeTrue();
        second.IsSuccess.Should().BeFalse();
        second.Error.Type.Should().Be(ErrorType.Validation);
    }

    [Fact]
    public async Task AddAsync_InvalidDirection_ReturnsValidationFailure()
    {
        var request = BuildValidStockMovementTypeRequest(direction: 0);

        var result = await StockMovementTypeService.AddAsync(request, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorType.Validation);
    }

    [Fact]
    public async Task GetAllAsync_AfterCreatingType_ReturnsCreatedType()
    {
        var request = BuildValidStockMovementTypeRequest();
        var created = await StockMovementTypeService.AddAsync(request, CancellationToken.None);

        var result = await StockMovementTypeService.GetAllAsync(CancellationToken.None);

        created.IsSuccess.Should().BeTrue();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Contain(e => e.Id == created.Value.Id && e.Name == request.Name);
    }

    [Fact]
    public async Task FindAsync_InvalidId_ReturnsValidationFailure()
    {
        var result = await StockMovementTypeService.FindAsync(0, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorType.Validation);
    }

    [Fact]
    public async Task FindAsync_MissingId_ReturnsNotFoundFailure()
    {
        var result = await StockMovementTypeService.FindAsync(999_999, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public async Task FindAsync_ExistingType_ReturnsDirectionLabel()
    {
        var created = await StockMovementTypeService.AddAsync(
            BuildValidStockMovementTypeRequest(direction: (byte)StockMovementDirection.Out),
            CancellationToken.None);

        var result = await StockMovementTypeService.FindAsync(created.Value.Id, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Direction.Should().Be(StockMovementDirection.Out.ToString());
    }

    [Fact]
    public async Task UpdateAsync_ValidRequest_PersistsUpdatedValues()
    {
        var created = await StockMovementTypeService.AddAsync(
            BuildValidStockMovementTypeRequest(direction: (byte)StockMovementDirection.In),
            CancellationToken.None);

        var request = BuildValidStockMovementTypeRequest(
            name: GenerateStockMovementTypeName(),
            description: "Updated description",
            direction: (byte)StockMovementDirection.Transfer);

        var result = await StockMovementTypeService.UpdateAsync(created.Value.Id, request, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().Be(request.Name);
        result.Value.Description.Should().Be(request.Description);

        AppDbContext.ChangeTracker.Clear();

        var persisted = await AppDbContext.StockMovementTypes
            .IgnoreQueryFilters()
            .SingleAsync(e => e.Id == created.Value.Id);

        persisted.Name.Should().Be(request.Name);
        persisted.Description.Should().Be(request.Description);
        persisted.Direction.Should().Be(StockMovementDirection.Transfer);
    }

    [Fact]
    public async Task UpdateAsync_DuplicateName_ReturnsValidationFailure()
    {
        var first = await StockMovementTypeService.AddAsync(BuildValidStockMovementTypeRequest(), CancellationToken.None);
        var second = await StockMovementTypeService.AddAsync(BuildValidStockMovementTypeRequest(), CancellationToken.None);

        var updateRequest = BuildValidStockMovementTypeRequest(name: second.Value.Name);

        var result = await StockMovementTypeService.UpdateAsync(first.Value.Id, updateRequest, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorType.Validation);
    }

    [Fact]
    public async Task UpdateAsync_MissingId_ReturnsNotFoundFailure()
    {
        var request = BuildValidStockMovementTypeRequest();

        var result = await StockMovementTypeService.UpdateAsync(999_999, request, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public async Task SoftDeleteAsync_ExistingType_SetsDeleteAuditFields()
    {
        var created = await StockMovementTypeService.AddAsync(BuildValidStockMovementTypeRequest(), CancellationToken.None);

        var result = await StockMovementTypeService.SoftDeleteAsync(created.Value.Id, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();

        AppDbContext.ChangeTracker.Clear();

        var deleted = await AppDbContext.StockMovementTypes
            .IgnoreQueryFilters()
            .SingleAsync(e => e.Id == created.Value.Id);

        deleted.IsDeleted.Should().BeTrue();
        deleted.DeletedAt.Should().NotBeNull();
        deleted.DeletedByUserId.Should().Be(TestUserId);
    }

    [Fact]
    public async Task SoftDeleteAsync_InvalidId_ReturnsValidationFailure()
    {
        var result = await StockMovementTypeService.SoftDeleteAsync(0, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorType.Validation);
    }

    [Fact]
    public async Task SoftDeleteAsync_MissingId_ReturnsNotFoundFailure()
    {
        var result = await StockMovementTypeService.SoftDeleteAsync(999_999, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorType.NotFound);
    }
}
