using Application.IntegrationTests.Common;
using Application.Locations.DTOs.Request;
using Domain.Shared.Errors;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Application.IntegrationTests.Services;

public sealed class LocationFeatureTests : LocationFeaturesIntegrationTestBase
{
    public LocationFeatureTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task CreateAsync_ValidRequest_PersistsLocation()
    {
        await AssertBaselineSeedIsAvailableAsync();
        var locationType = await CreateLocationTypeDirectAsync();
        var request = BuildValidLocationCreateRequest(locationType.Id);

        var result = await LocationService.CreateAsync(request, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().Be(request.Name);
        result.Value.Address.Should().Be(request.Address);
        result.Value.LocationTypeId.Should().Be(locationType.Id);
        result.Value.IsActive.Should().BeTrue();

        AppDbContext.ChangeTracker.Clear();

        var persisted = await AppDbContext.Locations
            .SingleAsync(e => e.Id == result.Value.Id);

        persisted.Name.Should().Be(request.Name);
        persisted.Address.Should().Be(request.Address);
        persisted.LocationTypeId.Should().Be(locationType.Id);
        persisted.CreatedByUserId.Should().Be(TestUserId);
        persisted.IsDeleted.Should().BeFalse();
    }

    [Fact]
    public async Task CreateAsync_InvalidRequest_ReturnsValidationFailure()
    {
        var request = new LocationCreateRequest
        {
            Name = string.Empty,
            Address = string.Empty,
            LocationTypeId = 0,
        };

        var result = await LocationService.CreateAsync(request, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorType.Validation);
    }

    [Fact]
    public async Task CreateAsync_DuplicateName_ReturnsValidationFailure()
    {
        var locationType = await CreateLocationTypeDirectAsync();
        var duplicateName = $"Duplicate-{Guid.NewGuid().ToString("N")[..10]}";

        var first = await LocationService.CreateAsync(
            BuildValidLocationCreateRequest(locationType.Id, duplicateName),
            CancellationToken.None);

        var second = await LocationService.CreateAsync(
            BuildValidLocationCreateRequest(locationType.Id, duplicateName),
            CancellationToken.None);

        first.IsSuccess.Should().BeTrue();
        second.IsSuccess.Should().BeFalse();
        second.Error.Type.Should().Be(ErrorType.Validation);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsLocations()
    {
        var locationType = await CreateLocationTypeDirectAsync();
        var first = await CreateLocationDirectAsync(locationType.Id, name: $"List-A-{Guid.NewGuid().ToString("N")[..8]}");
        var second = await CreateLocationDirectAsync(locationType.Id, name: $"List-B-{Guid.NewGuid().ToString("N")[..8]}");

        var result = await LocationService.GetAllAsync(CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Contain(e => e.Id == first.Id && e.Name == first.Name);
        result.Value.Should().Contain(e => e.Id == second.Id && e.Name == second.Name);
    }

    [Fact]
    public async Task FindAsync_ReturnsExpectedSuccessInvalidAndNotFoundCases()
    {
        var locationType = await CreateLocationTypeDirectAsync();
        var location = await CreateLocationDirectAsync(locationType.Id);

        var success = await LocationService.FindAsync(location.Id, CancellationToken.None);
        var invalid = await LocationService.FindAsync(0, CancellationToken.None);
        var missing = await LocationService.FindAsync(999_999, CancellationToken.None);

        success.IsSuccess.Should().BeTrue();
        success.Value.Id.Should().Be(location.Id);

        invalid.IsSuccess.Should().BeFalse();
        invalid.Error.Type.Should().Be(ErrorType.Validation);

        missing.IsSuccess.Should().BeFalse();
        missing.Error.Type.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public async Task UpdateAsync_ValidRequest_UpdatesLocation()
    {
        var sourceType = await CreateLocationTypeDirectAsync();
        var targetType = await CreateLocationTypeDirectAsync();
        var location = await CreateLocationDirectAsync(sourceType.Id);

        var request = new LocationUpdateRequest
        {
            Id = location.Id,
            Name = $"Updated-{Guid.NewGuid().ToString("N")[..8]}",
            Address = "Updated address",
            LocationTypeId = targetType.Id,
            IsActive = true,
        };

        var result = await LocationService.UpdateAsync(location.Id, request, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(location.Id);
        result.Value.Name.Should().Be(request.Name);
        result.Value.Address.Should().Be(request.Address);
        result.Value.LocationTypeId.Should().Be(targetType.Id);

        AppDbContext.ChangeTracker.Clear();

        var persisted = await AppDbContext.Locations.SingleAsync(e => e.Id == location.Id);
        persisted.Name.Should().Be(request.Name);
        persisted.Address.Should().Be(request.Address);
        persisted.LocationTypeId.Should().Be(targetType.Id);
    }

    [Fact]
    public async Task UpdateAsync_InvalidIdValidationAndMissingEntity_ReturnExpectedFailures()
    {
        var locationType = await CreateLocationTypeDirectAsync();
        var location = await CreateLocationDirectAsync(locationType.Id);

        var validRequest = new LocationUpdateRequest
        {
            Id = location.Id,
            Name = $"Valid-{Guid.NewGuid().ToString("N")[..8]}",
            Address = "Updated address",
            LocationTypeId = locationType.Id,
            IsActive = true,
        };

        var invalidRequest = new LocationUpdateRequest
        {
            Id = location.Id,
            Name = string.Empty,
            Address = string.Empty,
            LocationTypeId = 0,
            IsActive = true,
        };

        var invalidId = await LocationService.UpdateAsync(0, validRequest, CancellationToken.None);
        var validation = await LocationService.UpdateAsync(location.Id, invalidRequest, CancellationToken.None);

        var missingRequest = new LocationUpdateRequest
        {
            Id = 999_999,
            Name = $"Missing-{Guid.NewGuid().ToString("N")[..8]}",
            Address = "Address",
            LocationTypeId = locationType.Id,
            IsActive = true,
        };

        var missing = await LocationService.UpdateAsync(999_999, missingRequest, CancellationToken.None);

        invalidId.IsSuccess.Should().BeFalse();
        invalidId.Error.Type.Should().Be(ErrorType.Validation);

        validation.IsSuccess.Should().BeFalse();
        validation.Error.Type.Should().Be(ErrorType.Validation);

        missing.IsSuccess.Should().BeFalse();
        missing.Error.Type.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public async Task ActivateDeactivateAsync_ReturnExpectedSuccessConflictAndFailureCases()
    {
        var location = await CreateLocationDirectAsync(isActive: true);

        var deactivate = await LocationService.DeactivateAsync(location.Id, CancellationToken.None);
        var deactivateAgain = await LocationService.DeactivateAsync(location.Id, CancellationToken.None);

        var activate = await LocationService.ActivateAsync(location.Id, CancellationToken.None);
        var activateAgain = await LocationService.ActivateAsync(location.Id, CancellationToken.None);

        var invalid = await LocationService.ActivateAsync(0, CancellationToken.None);
        var missing = await LocationService.DeactivateAsync(999_999, CancellationToken.None);

        deactivate.IsSuccess.Should().BeTrue();
        deactivateAgain.IsSuccess.Should().BeFalse();
        deactivateAgain.Error.Type.Should().Be(ErrorType.Conflict);

        activate.IsSuccess.Should().BeTrue();
        activateAgain.IsSuccess.Should().BeFalse();
        activateAgain.Error.Type.Should().Be(ErrorType.Conflict);

        invalid.IsSuccess.Should().BeFalse();
        invalid.Error.Type.Should().Be(ErrorType.Validation);

        missing.IsSuccess.Should().BeFalse();
        missing.Error.Type.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public async Task SoftDeleteAsync_ReturnExpectedSuccessInvalidAndNotFoundCases()
    {
        var location = await CreateLocationDirectAsync();

        var invalid = await LocationService.SoftDeleteAsync(0, CancellationToken.None);
        var success = await LocationService.SoftDeleteAsync(location.Id, CancellationToken.None);
        var missing = await LocationService.SoftDeleteAsync(location.Id, CancellationToken.None);

        invalid.IsSuccess.Should().BeFalse();
        invalid.Error.Type.Should().Be(ErrorType.Validation);

        success.IsSuccess.Should().BeTrue();

        missing.IsSuccess.Should().BeFalse();
        missing.Error.Type.Should().Be(ErrorType.NotFound);

        AppDbContext.ChangeTracker.Clear();

        var deleted = await AppDbContext.Locations
            .IgnoreQueryFilters()
            .SingleAsync(e => e.Id == location.Id);

        deleted.IsDeleted.Should().BeTrue();
        deleted.DeletedAt.Should().NotBeNull();
        deleted.DeletedByUserId.Should().Be(TestUserId);
    }

    [Fact]
    public async Task GetLocationInventoriesAsync_ReturnExpectedInvalidNotFoundAndSuccessCases()
    {
        var location = await CreateLocationDirectAsync();

        var invalid = await LocationService.GetLocationInventoriesAsync(0, CancellationToken.None);
        var notFound = await LocationService.GetLocationInventoriesAsync(location.Id, CancellationToken.None);

        var product = await CreateProductDirectAsync();
        var inventory = await CreateInventoryDirectAsync(product.Id, location.Id, quantityOnHand: 7m, reorderLevel: 2m, maxLevel: 30m);

        var success = await LocationService.GetLocationInventoriesAsync(location.Id, CancellationToken.None);

        invalid.IsSuccess.Should().BeFalse();
        invalid.Error.Type.Should().Be(ErrorType.Validation);

        notFound.IsSuccess.Should().BeFalse();
        notFound.Error.Type.Should().Be(ErrorType.NotFound);

        success.IsSuccess.Should().BeTrue();
        success.Value.Should().Contain(e =>
            e.Id == inventory.Id
            && e.ProductId == product.Id
            && e.LocationId == location.Id
            && e.QuantityOnHand == 7m);
    }

    [Fact]
    public async Task GetLocationsNamesAsync_ReturnsLookupValues()
    {
        var first = await CreateLocationDirectAsync(name: $"Name-A-{Guid.NewGuid().ToString("N")[..8]}");
        var second = await CreateLocationDirectAsync(name: $"Name-B-{Guid.NewGuid().ToString("N")[..8]}");

        var result = await LocationService.GetLocationsNamesAsync(CancellationToken.None);

        result.IsSuccess.Should().BeTrue();

        var ids = result.Value
            .Select(e => (int)e.GetType().GetProperty("Id")!.GetValue(e)!)
            .ToList();

        var names = result.Value
            .Select(e => (string)e.GetType().GetProperty("Name")!.GetValue(e)!)
            .ToList();

        ids.Should().Contain(first.Id);
        ids.Should().Contain(second.Id);
        names.Should().Contain(first.Name);
        names.Should().Contain(second.Name);
    }

    [Fact]
    public async Task AddLocationTypeAsync_ValidRequest_PersistsLocationType()
    {
        var request = BuildValidLocationTypeCreateRequest();

        var result = await LocationTypeService.AddLocationTypeAsync(request, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().Be(request.Name);
        result.Value.Description.Should().Be(request.Description);

        AppDbContext.ChangeTracker.Clear();

        var persisted = await AppDbContext.LocationTypes
            .SingleAsync(e => e.Id == result.Value.Id);

        persisted.Name.Should().Be(request.Name);
        persisted.Description.Should().Be(request.Description);
        persisted.CreatedByUserId.Should().Be(TestUserId);
    }

    [Fact]
    public async Task AddLocationTypeAsync_InvalidAndDuplicateRequests_ReturnValidationFailures()
    {
        var invalid = await LocationTypeService.AddLocationTypeAsync(new LocationTypeCreateRequest
        {
            Name = string.Empty,
            Description = "Invalid",
        }, CancellationToken.None);

        var name = $"LT-Duplicate-{Guid.NewGuid().ToString("N")[..8]}";

        var first = await LocationTypeService.AddLocationTypeAsync(
            BuildValidLocationTypeCreateRequest(name),
            CancellationToken.None);

        var duplicate = await LocationTypeService.AddLocationTypeAsync(
            BuildValidLocationTypeCreateRequest(name),
            CancellationToken.None);

        invalid.IsSuccess.Should().BeFalse();
        invalid.Error.Type.Should().Be(ErrorType.Validation);

        first.IsSuccess.Should().BeTrue();

        duplicate.IsSuccess.Should().BeFalse();
        duplicate.Error.Type.Should().Be(ErrorType.Validation);
    }

    [Fact]
    public async Task GetAllLocationTypesAsync_ReturnsLocationTypes()
    {
        var first = await CreateLocationTypeDirectAsync(name: $"Type-A-{Guid.NewGuid().ToString("N")[..8]}");
        var second = await CreateLocationTypeDirectAsync(name: $"Type-B-{Guid.NewGuid().ToString("N")[..8]}");

        var result = await LocationTypeService.GetAllLocationTypesAsync(CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Contain(e => e.Id == first.Id && e.Name == first.Name);
        result.Value.Should().Contain(e => e.Id == second.Id && e.Name == second.Name);
    }

    [Fact]
    public async Task FindLocationTypeAsync_ReturnsExpectedSuccessInvalidAndNotFoundCases()
    {
        var locationType = await CreateLocationTypeDirectAsync();

        var success = await LocationTypeService.FindAsync(locationType.Id, CancellationToken.None);
        var invalid = await LocationTypeService.FindAsync(0, CancellationToken.None);
        var missing = await LocationTypeService.FindAsync(999_999, CancellationToken.None);

        success.IsSuccess.Should().BeTrue();
        success.Value.Id.Should().Be(locationType.Id);

        invalid.IsSuccess.Should().BeFalse();
        invalid.Error.Type.Should().Be(ErrorType.Validation);

        missing.IsSuccess.Should().BeFalse();
        missing.Error.Type.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public async Task SoftDeleteLocationTypeAsync_ReturnExpectedSuccessInvalidAndNotFoundCases()
    {
        var locationType = await CreateLocationTypeDirectAsync();

        var invalid = await LocationTypeService.SoftDeleteAsync(0, CancellationToken.None);
        var success = await LocationTypeService.SoftDeleteAsync(locationType.Id, CancellationToken.None);
        var missing = await LocationTypeService.SoftDeleteAsync(locationType.Id, CancellationToken.None);

        invalid.IsSuccess.Should().BeFalse();
        invalid.Error.Type.Should().Be(ErrorType.Validation);

        success.IsSuccess.Should().BeTrue();

        missing.IsSuccess.Should().BeFalse();
        missing.Error.Type.Should().Be(ErrorType.NotFound);

        AppDbContext.ChangeTracker.Clear();

        var deleted = await AppDbContext.LocationTypes
            .IgnoreQueryFilters()
            .SingleAsync(e => e.Id == locationType.Id);

        deleted.IsDeleted.Should().BeTrue();
        deleted.DeletedAt.Should().NotBeNull();
        deleted.DeletedByUserId.Should().Be(TestUserId);
    }
}
