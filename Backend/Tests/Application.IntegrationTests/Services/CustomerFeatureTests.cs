using Application.Customers;
using Application.Customers.Dtos;
using Application.IntegrationTests.Common;
using Application.Shared.Paging;
using Domain.Shared.Errors;
using FluentAssertions;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Application.IntegrationTests.Services;

public sealed class CustomerFeatureTests : CustomerFeaturesIntegrationTestBase
{
    public CustomerFeatureTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task AddAsync_ValidRequest_PersistsCustomerAndReturnsProjection()
    {
        await AssertBaselineSeedIsAvailableAsync();
        var categoryId = await CreateCustomerCategoryAsync();
        var request = BuildValidCustomerCreateRequest(categoryId);

        var result = await CustomerService.AddAsync(request);

        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().Be(request.Name);
        result.Value.Email.Should().Be(request.Email);
        result.Value.Phone.Should().Be(request.Phone);
        result.Value.CustomerCategoryId.Should().Be(categoryId);

        var persisted = await AppDbContext.Customers.SingleAsync(e => e.Id == result.Value.Id);
        persisted.Name.Should().Be(request.Name);
        persisted.Email.Should().Be(request.Email);
        persisted.Phone.Should().Be(request.Phone);
        persisted.CustomerCategoryId.Should().Be(categoryId);
    }

    [Fact]
    public async Task AddAsync_InvalidRequest_ReturnsValidationFailure()
    {
        var categoryId = await CreateCustomerCategoryAsync();
        var request = BuildValidCustomerCreateRequest(categoryId) with
        {
            Name = string.Empty,
            Email = "invalid-email",
            Phone = string.Empty,
            City = string.Empty,
            State = string.Empty,
            Street = string.Empty,
            ZipCode = string.Empty,
        };

        var result = await CustomerService.AddAsync(request);

        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorType.Validation);
    }

    [Fact]
    public async Task AddAsync_MissingCategory_ReturnsValidationFailure()
    {
        var request = BuildValidCustomerCreateRequest(999_999);

        var result = await CustomerService.AddAsync(request);

        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorType.Validation);
    }

    [Fact]
    public async Task UpdateAsync_InvalidId_ReturnsValidationFailure()
    {
        var categoryId = await CreateCustomerCategoryAsync();
        var request = new UpdateCustomerRequest
        {
            Name = "Updated",
            CustomerCategoryId = categoryId,
            Email = "updated@ims.local",
            Phone = "01000000000",
            Street = "Street",
            City = "City",
            State = "State",
            ZipCode = "12345",
        };

        var result = await CustomerService.UpdateAsync(0, request);

        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorType.Validation);
    }

    [Fact]
    public async Task UpdateAsync_MissingCustomer_ReturnsNotFoundFailure()
    {
        var categoryId = await CreateCustomerCategoryAsync();
        var request = new UpdateCustomerRequest
        {
            Name = "Updated",
            CustomerCategoryId = categoryId,
            Email = "updated@ims.local",
            Phone = "01000000000",
            Street = "Street",
            City = "City",
            State = "State",
            ZipCode = "12345",
        };

        var result = await CustomerService.UpdateAsync(999_999, request);

        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public async Task UpdateAsync_MissingCategory_ReturnsFailure()
    {
        var categoryId = await CreateCustomerCategoryAsync();
        var createResult = await CustomerService.AddAsync(BuildValidCustomerCreateRequest(categoryId));
        createResult.IsSuccess.Should().BeTrue();

        var request = new UpdateCustomerRequest
        {
            Name = "Updated",
            CustomerCategoryId = 999_999,
            Email = "updated@ims.local",
            Phone = "01000000000",
            Street = "New Street",
            City = "New City",
            State = "NS",
            ZipCode = "54321",
        };

        var result = await CustomerService.UpdateAsync(createResult.Value.Id, request);

        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorType.Failure);
    }

    [Fact]
    public async Task UpdateAsync_ValidRequest_UpdatesPersistedCustomerFields()
    {
        var sourceCategoryId = await CreateCustomerCategoryAsync();
        var targetCategoryId = await CreateCustomerCategoryAsync();

        var createResult = await CustomerService.AddAsync(BuildValidCustomerCreateRequest(sourceCategoryId));
        createResult.IsSuccess.Should().BeTrue();

        var request = new UpdateCustomerRequest
        {
            Name = $"Customer-IT-Updated-{Guid.NewGuid().ToString("N")[..6]}",
            CustomerCategoryId = targetCategoryId,
            Email = "updated-customer@ims.local",
            Phone = "01111111111",
            Street = "Updated Street",
            City = "Updated City",
            State = "US",
            ZipCode = "67890",
        };

        var result = await CustomerService.UpdateAsync(createResult.Value.Id, request);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(createResult.Value.Id);

        AppDbContext.ChangeTracker.Clear();
        var persisted = await AppDbContext.Customers.SingleAsync(e => e.Id == createResult.Value.Id);

        persisted.Name.Should().Be(request.Name);
        persisted.CustomerCategoryId.Should().Be(targetCategoryId);
        persisted.Email.Should().Be(request.Email);
        persisted.Phone.Should().Be(request.Phone);
        persisted.Address.Street.Should().Be(request.Street);
        persisted.Address.City.Should().Be(request.City);
        persisted.Address.State.Should().Be(request.State);
        persisted.Address.ZipCode.Should().Be(request.ZipCode);
    }

    [Fact]
    public async Task SoftDeleteAsync_ValidId_SoftDeletesCustomer()
    {
        var categoryId = await CreateCustomerCategoryAsync();
        var createResult = await CustomerService.AddAsync(BuildValidCustomerCreateRequest(categoryId));
        createResult.IsSuccess.Should().BeTrue();

        var result = await CustomerService.SoftDeleteAsync(createResult.Value.Id, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();

        var deleted = await AppDbContext.Customers
            .IgnoreQueryFilters()
            .SingleAsync(e => e.Id == createResult.Value.Id);

        deleted.IsDeleted.Should().BeTrue();
        deleted.DeletedByUserId.Should().Be(TestUserId);
    }

    [Fact]
    public async Task SoftDeleteAsync_InvalidId_ReturnsValidationFailure()
    {
        var result = await CustomerService.SoftDeleteAsync(0, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorType.Validation);
    }

    [Fact]
    public async Task SoftDeleteAsync_AlreadyDeleted_ReturnsNotFoundFailure()
    {
        var categoryId = await CreateCustomerCategoryAsync();
        var createResult = await CustomerService.AddAsync(BuildValidCustomerCreateRequest(categoryId));
        createResult.IsSuccess.Should().BeTrue();

        (await CustomerService.SoftDeleteAsync(createResult.Value.Id, CancellationToken.None))
            .IsSuccess.Should().BeTrue();

        var secondDelete = await CustomerService.SoftDeleteAsync(createResult.Value.Id, CancellationToken.None);

        secondDelete.IsSuccess.Should().BeFalse();
        secondDelete.Error.Type.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public async Task CustomerQueries_GetByIdAsync_InvalidId_ReturnsValidationFailure()
    {
        var result = await CustomerQueries.GetByIdAsync(0);

        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorType.Validation);
    }

    [Fact]
    public async Task CustomerQueries_GetByIdAsync_MissingId_ReturnsNotFoundFailure()
    {
        var result = await CustomerQueries.GetByIdAsync(999_999);

        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public async Task CustomerQueries_GetByIdAsync_ExistingCustomer_ReturnsProjectedFields()
    {
        var categoryId = await CreateCustomerCategoryAsync();
        var createResult = await CustomerService.AddAsync(BuildValidCustomerCreateRequest(categoryId));
        createResult.IsSuccess.Should().BeTrue();

        var result = await CustomerQueries.GetByIdAsync(createResult.Value.Id);

        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(createResult.Value.Id);
        result.Value.CustomerCategoryId.Should().Be(categoryId);
        result.Value.CustomerCategoryName.Should().NotBeNullOrWhiteSpace();
        result.Value.CreatedByUserId.Should().Be(TestUserId);
        result.Value.CreatedByUserName.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task CustomerQueries_GetAllAsync_NoMatchedSearch_ReturnsNotFound()
    {
        var request = new TableRequest
        {
            Page = 1,
            PageSize = 10,
            search = $"MISSING-{Guid.NewGuid():N}",
            SortColumn = "name",
            SortOrder = "asc",
        };

        var result = await CustomerQueries.GetAllAsync(request);

        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public async Task CustomerQueries_GetAllAsync_SearchAndSortByTotalSpent_ReturnsExpectedAggregates()
    {
        var categoryId = await CreateCustomerCategoryAsync();
        var firstRequest = BuildValidCustomerCreateRequest(categoryId, name: $"Customer-IT-A-{Guid.NewGuid().ToString("N")[..6]}");
        var secondRequest = BuildValidCustomerCreateRequest(categoryId, name: $"Customer-IT-B-{Guid.NewGuid().ToString("N")[..6]}");

        var firstCreate = await CustomerService.AddAsync(firstRequest);
        var secondCreate = await CustomerService.AddAsync(secondRequest);

        firstCreate.IsSuccess.Should().BeTrue();
        secondCreate.IsSuccess.Should().BeTrue();

        await CreateCompletedSalesOrderAsync(firstCreate.Value.Id, quantity: 2m, unitPrice: 10m);
        await CreatePendingSalesOrderAsync(firstCreate.Value.Id, quantity: 1m, unitPrice: 20m);
        await CreateCompletedSalesOrderAsync(secondCreate.Value.Id, quantity: 1m, unitPrice: 5m);

        var result = await CustomerQueries.GetAllAsync(new TableRequest
        {
            Page = 1,
            PageSize = 10,
            search = "Customer-IT-",
            SortColumn = "totalspent",
            SortOrder = "desc",
        });

        result.IsSuccess.Should().BeTrue();

        var first = result.Value.Item.Single(e => e.Id == firstCreate.Value.Id);
        var second = result.Value.Item.Single(e => e.Id == secondCreate.Value.Id);

        first.TotalOrders.Should().Be(2);
        first.TotalSpent.Should().Be(20m);
        second.TotalOrders.Should().Be(1);
        second.TotalSpent.Should().Be(5m);

        result.Value.Item.First().Id.Should().Be(firstCreate.Value.Id);
    }

    [Fact]
    public async Task CustomerQueries_GetCustomerSummary_ReturnsExpectedShape()
    {
        var categoryId = await CreateCustomerCategoryAsync();
        var customerCreate = await CustomerService.AddAsync(BuildValidCustomerCreateRequest(categoryId));
        customerCreate.IsSuccess.Should().BeTrue();
        await CreateCompletedSalesOrderAsync(customerCreate.Value.Id, quantity: 2m, unitPrice: 15m);

        var result = await CustomerQueries.GetCustomerSummary();

        result.IsSuccess.Should().BeTrue();

        GetSummaryValue<int>(result.Value, "TotalCustomers").Should().BeGreaterThan(0);
        GetSummaryValue<int>(result.Value, "ActiveCustomers").Should().BeGreaterThanOrEqualTo(0);
        GetSummaryValue<int>(result.Value, "NewCustomersLastMonth").Should().BeGreaterThanOrEqualTo(0);
        GetSummaryValue<decimal>(result.Value, "TotalRevenue").Should().BeGreaterThanOrEqualTo(0m);
    }

    [Fact]
    public async Task CustomerCategoryService_CreateCustomerAsync_ValidRequest_PersistsCategory()
    {
        var request = new CreateCustomerCategoryRequest(
            Name: $"CustomerCategory-IT-{Guid.NewGuid().ToString("N")[..8]}",
            IsIndividual: false,
            Description: "Customer category for integration test");

        var result = await CustomerCategoryService.CreateCustomerAsync(request, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        (await AppDbContext.CustomerCategories.AnyAsync(e => e.Id == result.Value.Id)).Should().BeTrue();
    }

    [Fact]
    public async Task CustomerCategoryService_CreateCustomerAsync_InvalidRequest_ThrowsValidationException()
    {
        var request = new CreateCustomerCategoryRequest(
            Name: string.Empty,
            IsIndividual: false,
            Description: "Invalid");

        var action = async () => await CustomerCategoryService.CreateCustomerAsync(request, CancellationToken.None);

        await action.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task CustomerCategoryService_UpdateCustomerAsync_ValidRequest_UpdatesCategory()
    {
        var categoryId = await CreateCustomerCategoryAsync();
        var command = new UpdateCustomerCategoryCommand(
            Id: categoryId,
            Name: $"CustomerCategory-IT-Updated-{Guid.NewGuid().ToString("N")[..6]}",
            IsIndividual: true,
            Description: "Updated category");

        var result = await CustomerCategoryService.UpdateCustomerAsync(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();

        var persisted = await AppDbContext.CustomerCategories.SingleAsync(e => e.Id == categoryId);
        persisted.Name.Should().Be(command.Name);
        persisted.IsIndividual.Should().BeTrue();
        persisted.Description.Should().Be(command.Description);
    }

    [Fact]
    public async Task CustomerCategoryService_UpdateCustomerAsync_MissingId_ReturnsNotFoundFailure()
    {
        var command = new UpdateCustomerCategoryCommand(
            Id: 999_999,
            Name: "Updated",
            IsIndividual: false,
            Description: "Updated description");

        var result = await CustomerCategoryService.UpdateCustomerAsync(command, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public async Task CustomerCategoryService_UpdateCustomerAsync_InvalidCommand_ThrowsValidationException()
    {
        var command = new UpdateCustomerCategoryCommand(
            Id: 0,
            Name: string.Empty,
            IsIndividual: false,
            Description: "Invalid");

        var action = async () => await CustomerCategoryService.UpdateCustomerAsync(command, CancellationToken.None);

        await action.Should().ThrowAsync<ValidationException>();
    }

    [Fact]
    public async Task CustomerCategoryService_GetByIdAsync_MissingId_ReturnsNotFoundFailure()
    {
        var result = await CustomerCategoryService.GetByIdAsync(999_999, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public async Task CustomerCategoryService_GetByIdAsync_Existing_ReturnsCategory()
    {
        var categoryId = await CreateCustomerCategoryAsync();

        var result = await CustomerCategoryService.GetByIdAsync(categoryId, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Id.Should().Be(categoryId);
    }

    [Fact]
    public async Task CustomerCategoryService_GetAllAsync_ReturnsCreatedCategories()
    {
        var firstCategoryId = await CreateCustomerCategoryAsync();
        var secondCategoryId = await CreateCustomerCategoryAsync();

        var result = await CustomerCategoryService.GetAllAsync(CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Contain(e => e.Id == firstCategoryId);
        result.Value.Should().Contain(e => e.Id == secondCategoryId);
    }

    [Fact]
    public async Task CustomerCategoryService_GetCategoriesNamesAsync_WithCategories_ReturnsIdNamePairs()
    {
        var categoryId = await CreateCustomerCategoryAsync();

        var result = await CustomerCategoryService.GetCategoriesNamesAsync(CancellationToken.None);

        result.IsSuccess.Should().BeTrue();

        var projected = result.Value
            .Select(e => new
            {
                Id = GetSummaryValue<int>(e, "Id"),
                Name = GetSummaryValue<string>(e, "Name"),
            })
            .ToList();

        projected.Should().Contain(e => e.Id == categoryId);
    }

    [Fact]
    public async Task CustomerCategoryService_SoftDeleteAsync_ValidAndRepeat_ReturnsExpectedResults()
    {
        var categoryId = await CreateCustomerCategoryAsync();

        var firstDelete = await CustomerCategoryService.SoftDeleteAsync(categoryId, CancellationToken.None);
        var secondDelete = await CustomerCategoryService.SoftDeleteAsync(categoryId, CancellationToken.None);

        firstDelete.IsSuccess.Should().BeTrue();
        secondDelete.IsSuccess.Should().BeFalse();
        secondDelete.Error.Type.Should().Be(ErrorType.NotFound);

        var deleted = await AppDbContext.CustomerCategories
            .IgnoreQueryFilters()
            .SingleAsync(e => e.Id == categoryId);
        deleted.IsDeleted.Should().BeTrue();
    }

    [Fact]
    public async Task CustomerCategoryService_SoftDeleteAsync_InvalidId_ReturnsValidationFailure()
    {
        var result = await CustomerCategoryService.SoftDeleteAsync(0, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorType.Validation);
    }

    private static T GetSummaryValue<T>(object summary, string propertyName)
    {
        var property = summary.GetType().GetProperty(propertyName);
        property.Should().NotBeNull();

        return (T)property!.GetValue(summary)!;
    }
}