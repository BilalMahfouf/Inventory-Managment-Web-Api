using Application.IntegrationTests.Common;
using Application.Products.DTOs.Request.Categories;
using Domain.Products.Enums;
using Domain.Shared.Errors;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace Application.IntegrationTests.Services;

public sealed class ProductCategoryFeatureTests : ProductFeaturesIntegrationTestBase
{
    public ProductCategoryFeatureTests(IntegrationTestWebAppFactory factory)
        : base(factory)
    {
    }

    [Fact]
    public async Task AddAsync_ValidRootCategory_PersistsCategory()
    {
        var request = new ProductCategoryRequest(
            Name: $"Root-{Guid.NewGuid().ToString("N")[..8]}",
            Description: "Root category",
            ParentId: null);

        var result = await ProductCategoryService.AddAsync(request, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().Be(request.Name);
        result.Value.ParentId.Should().BeNull();

        (await AppDbContext.ProductCategories.AnyAsync(e => e.Id == result.Value.Id)).Should().BeTrue();
    }

    [Fact]
    public async Task AddAsync_InvalidRequest_ReturnsValidationFailure()
    {
        var request = new ProductCategoryRequest(
            Name: string.Empty,
            Description: "Invalid",
            ParentId: 0);

        var result = await ProductCategoryService.AddAsync(request, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorType.Validation);
    }

    [Fact]
    public async Task AddAsync_MissingParent_ReturnsNotFoundFailure()
    {
        var request = new ProductCategoryRequest(
            Name: $"Child-{Guid.NewGuid().ToString("N")[..8]}",
            Description: "Child category",
            ParentId: 999_999);

        var result = await ProductCategoryService.AddAsync(request, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public async Task AddAsync_ValidParent_CreatesChildCategory()
    {
        var parent = await CreateCategoryAsync(type: ProductCategoryType.MainCategory);
        var request = new ProductCategoryRequest(
            Name: $"Child-{Guid.NewGuid().ToString("N")[..8]}",
            Description: "Child category",
            ParentId: parent.Id);

        var result = await ProductCategoryService.AddAsync(request, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.ParentId.Should().Be(parent.Id);
        result.Value.ParentName.Should().Be(parent.Name);
    }

    [Fact]
    public async Task FindAsync_ReturnsExpectedSuccessInvalidAndNotFoundCases()
    {
        var category = await CreateCategoryAsync(type: ProductCategoryType.MainCategory);

        var success = await ProductCategoryService.FindAsync(category.Id);
        var invalid = await ProductCategoryService.FindAsync(0);
        var notFound = await ProductCategoryService.FindAsync(999_999);

        success.IsSuccess.Should().BeTrue();
        success.Value.Id.Should().Be(category.Id);

        invalid.IsSuccess.Should().BeFalse();
        invalid.Error.Type.Should().Be(ErrorType.Validation);

        notFound.IsSuccess.Should().BeFalse();
        notFound.Error.Type.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsCategories()
    {
        await CreateCategoryAsync(name: "All-A", type: ProductCategoryType.MainCategory);
        await CreateCategoryAsync(name: "All-B", type: ProductCategoryType.MainCategory);

        var result = await ProductCategoryService.GetAllAsync();

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Contain(e => e.Name == "All-A");
        result.Value.Should().Contain(e => e.Name == "All-B");
    }

    [Fact]
    public async Task GetAllChildrenAsync_ReturnsExpectedSuccessAndNotFoundCases()
    {
        var parent = await CreateCategoryAsync(name: "Parent-A", type: ProductCategoryType.MainCategory);
        await CreateCategoryAsync(name: "Child-A", parentId: parent.Id, type: ProductCategoryType.SubCategory);

        var success = await ProductCategoryService.GetAllChildrenAsync(parent.Id);
        var notFound = await ProductCategoryService.GetAllChildrenAsync(999_999);

        success.IsSuccess.Should().BeTrue();
        success.Value.Should().ContainSingle(e => e.Name == "Child-A");

        notFound.IsSuccess.Should().BeFalse();
        notFound.Error.Type.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public async Task GetAllTreeAsync_ReturnsParentWithChildren()
    {
        var parent = await CreateCategoryAsync(name: "Tree-Parent", type: ProductCategoryType.MainCategory);
        await CreateCategoryAsync(name: "Tree-Child", parentId: parent.Id, type: ProductCategoryType.SubCategory);

        var result = await ProductCategoryService.GetAllTreeAsync();

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Contain(e => e.Name == "Tree-Parent");

        var parentNode = result.Value.Single(e => e.Name == "Tree-Parent");
        parentNode.Children.Should().NotBeNull();
        parentNode.Children.Should().Contain(e => e.Name == "Tree-Child");
    }

    [Fact]
    public async Task UpdateAsync_ReturnsExpectedSuccessInvalidAndNotFoundCases()
    {
        var oldParent = await CreateCategoryAsync(name: "Update-Old-Parent", type: ProductCategoryType.MainCategory);
        var parent = await CreateCategoryAsync(name: "Update-Parent", type: ProductCategoryType.MainCategory);
        var category = await CreateCategoryAsync(
            name: "Update-Original",
            parentId: oldParent.Id,
            type: ProductCategoryType.SubCategory);

        var invalidRequest = new ProductCategoryRequest(string.Empty, "Invalid", 0);
        var invalid = await ProductCategoryService.UpdateAsync(0, invalidRequest, CancellationToken.None);

        var request = new ProductCategoryRequest("Update-Renamed", "Updated", parent.Id);
        var success = await ProductCategoryService.UpdateAsync(category.Id, request, CancellationToken.None);
        var notFound = await ProductCategoryService.UpdateAsync(999_999, request, CancellationToken.None);

        invalid.IsSuccess.Should().BeFalse();
        invalid.Error.Type.Should().Be(ErrorType.Validation);

        success.IsSuccess.Should().BeTrue();
        success.Value.Name.Should().Be("Update-Renamed");
        success.Value.ParentId.Should().Be(parent.Id);

        notFound.IsSuccess.Should().BeFalse();
        notFound.Error.Type.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public async Task DeleteAsync_ReturnsExpectedSuccessInvalidAndNotFoundCases()
    {
        var category = await CreateCategoryAsync(name: "Delete-Category", type: ProductCategoryType.MainCategory);

        var invalid = await ProductCategoryService.DeleteAsync(0, CancellationToken.None);
        var success = await ProductCategoryService.DeleteAsync(category.Id, CancellationToken.None);
        var notFound = await ProductCategoryService.DeleteAsync(category.Id, CancellationToken.None);

        invalid.IsSuccess.Should().BeFalse();
        invalid.Error.Type.Should().Be(ErrorType.Validation);

        success.IsSuccess.Should().BeTrue();

        notFound.IsSuccess.Should().BeFalse();
        notFound.Error.Type.Should().Be(ErrorType.NotFound);

        var deleted = await AppDbContext.ProductCategories
            .IgnoreQueryFilters()
            .SingleAsync(e => e.Id == category.Id);
        deleted.IsDeleted.Should().BeTrue();
    }

    [Fact]
    public async Task GetCategoriesNamesAsync_ReturnsCategoryLookups()
    {
        await CreateCategoryAsync(name: "Names-A", type: ProductCategoryType.MainCategory);

        var result = await ProductCategoryService.GetCategoriesNamesAsync();

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();
    }

    [Fact]
    public async Task ProductCategoryQueries_GetCategoryByIdAsync_ReturnsMainAndSubCategoryViews()
    {
        var parent = await CreateCategoryAsync(name: "Query-Parent", type: ProductCategoryType.MainCategory);
        var child = await CreateCategoryAsync(name: "Query-Child", parentId: parent.Id, type: ProductCategoryType.SubCategory);

        var mainResult = await ProductCategoryQueries.GetCategoryByIdAsync(parent.Id, CancellationToken.None);
        var subResult = await ProductCategoryQueries.GetCategoryByIdAsync(child.Id, CancellationToken.None);
        var missing = await ProductCategoryQueries.GetCategoryByIdAsync(999_999, CancellationToken.None);

        mainResult.IsSuccess.Should().BeTrue();
        mainResult.Value.Name.Should().Be("Query-Parent");
        mainResult.Value.SubCategories.Should().NotBeNull();

        subResult.IsSuccess.Should().BeTrue();
        subResult.Value.Name.Should().Be("Query-Child");
        subResult.Value.ParentId.Should().Be(parent.Id);
        subResult.Value.ParentName.Should().Be(parent.Name);

        missing.IsSuccess.Should().BeFalse();
        missing.Error.Type.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public async Task ProductCategoryQueries_GetMainCategoriesAsync_ReturnsMainCategories()
    {
        await CreateCategoryAsync(name: "Main-Query", type: ProductCategoryType.MainCategory);
        await CreateCategoryAsync(name: "Sub-Query", parentId: null, type: ProductCategoryType.SubCategory);

        var result = await ProductCategoryQueries.GetMainCategoriesAsync(CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();
    }
}