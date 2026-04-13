using System.Net;
using Application.IntegrationTests.Common;
using Application.UnitOfMeasures.DTOs;
using Application.UnitOfMeasures.Services;
using Domain.Shared.Errors;
using FluentAssertions;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Application.IntegrationTests.Services;

public sealed class UnitOfMeasureServiceTests : IClassFixture<IntegrationTestWebAppFactory>, IAsyncLifetime
{
    private const int InvalidId = 0;
    private const int MissingId = 999_999;
    private const int MaxNameLength = 50;
    private const string UnitPrefix = "UnitOfMeasure-IT-";

    private readonly AsyncServiceScope _scope;

    public UnitOfMeasureServiceTests(IntegrationTestWebAppFactory factory)
    {
        _scope = factory.Services.CreateAsyncScope();
        UnitOfMeasureService = _scope.ServiceProvider.GetRequiredService<UnitOfMeasureService>();
        AppDbContext = _scope.ServiceProvider.GetRequiredService<InventoryManagmentDBContext>();
        HttpClient = factory.CreateClient();
    }

    private UnitOfMeasureService UnitOfMeasureService { get; }

    private InventoryManagmentDBContext AppDbContext { get; }

    private HttpClient HttpClient { get; }

    private static int TestUserId => IntegrationTestWebAppFactory.SeedUserId;

    public async Task InitializeAsync()
    {
        await CleanupUnitOfMeasureDataAsync();
    }

    public async Task DisposeAsync()
    {
        await CleanupUnitOfMeasureDataAsync();
        await _scope.DisposeAsync();
    }

    [Fact]
    public async Task AddAsync_ValidRequest_ReturnsCreatedUnit()
    {
        // Arrange
        var request = BuildValidRequest();

        // Act
        var result = await UnitOfMeasureService.AddAsync(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().Be(request.Name);
    }

    [Fact]
    public async Task AddAsync_ValidRequest_PersistsAuditFields()
    {
        // Arrange
        var request = BuildValidRequest();

        // Act
        var result = await UnitOfMeasureService.AddAsync(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        AppDbContext.ChangeTracker.Clear();
        var persistedEntity = await AppDbContext.UnitOfMeasures
            .IgnoreQueryFilters()
            .SingleAsync(e => e.Id == result.Value.Id);

        persistedEntity.CreatedByUserId.Should().Be(TestUserId);
        persistedEntity.IsDeleted.Should().BeFalse();
    }

    [Fact]
    public async Task AddAsync_EmptyName_ReturnsValidationFailure()
    {
        // Arrange
        var request = BuildValidRequest(name: string.Empty);

        // Act
        var result = await UnitOfMeasureService.AddAsync(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorType.Validation);
    }

    [Fact]
    public async Task AddAsync_NameAtBoundaryLength_ReturnsSuccess()
    {
        // Arrange
        var request = BuildValidRequest(name: new string('A', MaxNameLength));

        // Act
        var result = await UnitOfMeasureService.AddAsync(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Length.Should().Be(MaxNameLength);
    }

    [Fact]
    public async Task AddAsync_NameLongerThanBoundary_ReturnsValidationFailure()
    {
        // Arrange
        var request = BuildValidRequest(name: new string('B', MaxNameLength + 1));

        // Act
        var result = await UnitOfMeasureService.AddAsync(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorType.Validation);
    }

    [Fact]
    public async Task AddAsync_DuplicateName_ReturnsConflictFailure()
    {
        // Arrange
        var duplicateName = GenerateUnitName();
        var firstRequest = BuildValidRequest(name: duplicateName);
        var secondRequest = BuildValidRequest(name: duplicateName);

        // Act
        var first = await UnitOfMeasureService.AddAsync(firstRequest, CancellationToken.None);
        var second = await UnitOfMeasureService.AddAsync(secondRequest, CancellationToken.None);

        // Assert
        first.IsSuccess.Should().BeTrue();
        second.IsSuccess.Should().BeFalse();
        second.Error.Type.Should().Be(ErrorType.Conflict);
    }

    [Fact]
    public async Task FindAsync_InvalidId_ReturnsValidationFailure()
    {
        // Arrange
        // Act
        var result = await UnitOfMeasureService.FindAsync(InvalidId, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorType.Validation);
    }

    [Fact]
    public async Task FindAsync_MissingId_ReturnsNotFoundFailure()
    {
        // Arrange
        // Act
        var result = await UnitOfMeasureService.FindAsync(MissingId, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public async Task GetAllAsync_AfterAddingUnit_ReturnsCreatedUnit()
    {
        // Arrange
        var created = await UnitOfMeasureService.AddAsync(BuildValidRequest(), CancellationToken.None);

        // Act
        var result = await UnitOfMeasureService.GetAllAsync(CancellationToken.None);

        // Assert
        created.IsSuccess.Should().BeTrue();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Contain(e => e.Id == created.Value.Id);
    }

    [Fact]
    public async Task UpdateAsync_ValidRequest_PersistsUpdatedValues()
    {
        // Arrange
        var created = await UnitOfMeasureService.AddAsync(BuildValidRequest(), CancellationToken.None);
        var request = BuildValidRequest(name: GenerateUnitName(), description: "Updated description");

        // Act
        var result = await UnitOfMeasureService.UpdateAsync(created.Value.Id, request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().Be(request.Name);
        result.Value.Description.Should().Be(request.Description);
    }

    [Fact]
    public async Task UpdateAsync_DuplicateName_ReturnsConflictFailure()
    {
        // Arrange
        var first = await UnitOfMeasureService.AddAsync(BuildValidRequest(), CancellationToken.None);
        var second = await UnitOfMeasureService.AddAsync(BuildValidRequest(), CancellationToken.None);
        var updateRequest = BuildValidRequest(name: second.Value.Name);

        // Act
        var result = await UnitOfMeasureService.UpdateAsync(first.Value.Id, updateRequest, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorType.Conflict);
    }

    [Fact]
    public async Task UpdateAsync_DeletedUnit_ReturnsValidationFailure()
    {
        // Arrange
        var created = await UnitOfMeasureService.AddAsync(BuildValidRequest(), CancellationToken.None);
        var deleted = await UnitOfMeasureService.SoftDeleteAsync(created.Value.Id, CancellationToken.None);
        var request = BuildValidRequest(name: GenerateUnitName());

        // Act
        var result = await UnitOfMeasureService.UpdateAsync(created.Value.Id, request, CancellationToken.None);

        // Assert
        deleted.IsSuccess.Should().BeTrue();
        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public async Task SoftDeleteAsync_ExistingUnit_SetsDeleteAuditFields()
    {
        // Arrange
        var created = await UnitOfMeasureService.AddAsync(BuildValidRequest(), CancellationToken.None);

        // Act
        var result = await UnitOfMeasureService.SoftDeleteAsync(created.Value.Id, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        AppDbContext.ChangeTracker.Clear();
        var deletedEntity = await AppDbContext.UnitOfMeasures
            .IgnoreQueryFilters()
            .SingleAsync(e => e.Id == created.Value.Id);

        deletedEntity.IsDeleted.Should().BeTrue();
        deletedEntity.DeletedAt.Should().NotBeNull();
        deletedEntity.DeletedByUserId.Should().Be(TestUserId);
    }

    [Fact]
    public async Task SoftDeleteAsync_AlreadyDeletedUnit_ReturnsNotFoundFailure()
    {
        // Arrange
        var created = await UnitOfMeasureService.AddAsync(BuildValidRequest(), CancellationToken.None);
        var firstDelete = await UnitOfMeasureService.SoftDeleteAsync(created.Value.Id, CancellationToken.None);

        // Act
        var secondDelete = await UnitOfMeasureService.SoftDeleteAsync(created.Value.Id, CancellationToken.None);

        // Assert
        firstDelete.IsSuccess.Should().BeTrue();
        secondDelete.IsSuccess.Should().BeFalse();
        secondDelete.Error.Type.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public async Task GetUnitsNamesAsync_AfterAddingUnit_ReturnsUnitNameProjection()
    {
        // Arrange
        var created = await UnitOfMeasureService.AddAsync(BuildValidRequest(), CancellationToken.None);

        // Act
        var result = await UnitOfMeasureService.GetUnitsNamesAsync(CancellationToken.None);

        // Assert
        created.IsSuccess.Should().BeTrue();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().ContainSingle(e => GetValue<int>(e, "Id") == created.Value.Id);
    }

    [Fact]
    public async Task AddAsync_NullDescription_ReturnsSuccess()
    {
        // Arrange
        var request = BuildValidRequest(description: null);

        // Act
        var result = await UnitOfMeasureService.AddAsync(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Description.Should().BeNull();
    }

    [Fact]
    public async Task GetAllEndpoint_WithoutAuthorization_ReturnsUnauthorized()
    {
        // Arrange
        // Act
        var response = await HttpClient.GetAsync("/api/unit-of-measures/");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    private static TProperty GetValue<TProperty>(object source, string propertyName)
    {
        var propertyInfo = source.GetType().GetProperty(propertyName);
        propertyInfo.Should().NotBeNull();
        return (TProperty)propertyInfo!.GetValue(source)!;
    }

    private static string GenerateUnitName()
    {
        return $"{UnitPrefix}{Guid.NewGuid():N}";
    }

    private static UnitOfMeasureRequest BuildValidRequest(
        string? name = null,
        string? description = "Integration unit of measure")
    {
        return new UnitOfMeasureRequest
        {
            Name = name ?? GenerateUnitName(),
            Description = description,
        };
    }

    private async Task CleanupUnitOfMeasureDataAsync()
    {
        AppDbContext.ChangeTracker.Clear();

        await AppDbContext.UnitOfMeasures
            .IgnoreQueryFilters()
            .Where(e => EF.Functions.Like(e.Name, $"{UnitPrefix}%"))
            .ExecuteDeleteAsync();

        await AppDbContext.SaveChangesAsync();
    }
}