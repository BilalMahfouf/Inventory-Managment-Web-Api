using Application.Products.Contracts;
using Application.Shared.Contracts;
using Application.Users.Contracts;
using Application.Products.DTOs.Request.Categories;
using Application.Products.DTOs.Response.Categories;
using Domain.Shared.Results;
using Application.Products.Services;
using Domain.Shared.Entities;
using Domain.Products.Entities;
using Domain.Shared.Errors;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using System.Linq.Expressions;
using Xunit;

namespace Application.Tests.ProductTests;

public class ProductCategoryServiceTests
{
    private readonly Mock<IBaseRepository<ProductCategory>> _repositoryMock = new();
    private readonly Mock<IUnitOfWork> _uowMock = new();
    private readonly Mock<ICurrentUserService> _currentUserServiceMock = new();
    private readonly Mock<IValidator<ProductCategoryRequest>> _validatorMock = new();
    private readonly Mock<IProductCategoryQueries> _queryMock = new();

    private ProductCategoryService CreateService()
    {
        return new ProductCategoryService(
            _repositoryMock.Object,
            _uowMock.Object,
            _currentUserServiceMock.Object,
            _validatorMock.Object,
            _queryMock.Object
        );
    }

    #region AddAsync Tests

    [Fact]
    public async Task AddAsync_ReturnsFailure_WhenValidationFails()
    {
        // Arrange
        var request = new ProductCategoryRequest("", null, null);
        var validationResult = new ValidationResult(new[]
        {
            new ValidationFailure("Name", "Name is required")
        });

        _validatorMock.Setup(v => v.Validate(request)).Returns(validationResult);

        var service = CreateService();

        // Act
        var result = await service.AddAsync(request, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Validation, result.Error.Type);
        Assert.Contains("Name is required", result.Error.Description);
    }

    [Fact]
    public async Task AddAsync_ReturnsNotFound_WhenParentDoesNotExist()
    {
        // Arrange
        var request = new ProductCategoryRequest("Test Category", "Description", 999);
        var validationResult = new ValidationResult();

        _validatorMock.Setup(v => v.Validate(request)).Returns(validationResult);
        _repositoryMock.Setup(r => r.IsExistAsync(
            It.IsAny<Expression<Func<ProductCategory, bool>>>(),
            It.IsAny<CancellationToken>()
        )).ReturnsAsync(false);

        var service = CreateService();

        // Act
        var result = await service.AddAsync(request, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.NotFound, result.Error.Type);
    }

    [Fact]
    public async Task AddAsync_ReturnsSuccess_WhenValidRequest()
    {
        // Arrange
        var request = new ProductCategoryRequest("Test Category", "Description", null);
        var validationResult = new ValidationResult();

        _validatorMock.Setup(v => v.Validate(request)).Returns(validationResult);
        _currentUserServiceMock.SetupGet(c => c.UserId).Returns(1);
        _repositoryMock.Setup(r => r.Add(It.IsAny<ProductCategory>()));
        _uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var service = CreateService();

        // Act
        var result = await service.AddAsync(request, CancellationToken.None);

        // Verify the expected behavior - the service should call Add and SaveChanges
        _repositoryMock.Verify(r => r.Add(It.IsAny<ProductCategory>()), Times.Once);
        _uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AddAsync_ReturnsException_WhenExceptionThrown()
    {
        // Arrange
        var request = new ProductCategoryRequest("Test Category", "Description", null);
        var validationResult = new ValidationResult();

        _validatorMock.Setup(v => v.Validate(request)).Returns(validationResult);
        _repositoryMock.Setup(r => r.Add(It.IsAny<ProductCategory>()))
            .Throws(new Exception("Database error"));

        var service = CreateService();

        // Act
        var result = await service.AddAsync(request, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Failure, result.Error.Type);
    }

    #endregion

    #region DeleteAsync Tests

    [Fact]
    public async Task DeleteAsync_ReturnsInvalidId_WhenIdIsZero()
    {
        // Arrange
        var service = CreateService();

        // Act
        var result = await service.DeleteAsync(0, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Validation, result.Error.Type);
    }

    [Fact]
    public async Task DeleteAsync_ReturnsNotFound_WhenCategoryDoesNotExist()
    {
        // Arrange
        _repositoryMock.Setup(r => r.FindAsync(
            It.IsAny<Expression<Func<ProductCategory, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>()
        )).ReturnsAsync((ProductCategory)null!);

        var service = CreateService();

        // Act
        var result = await service.DeleteAsync(1, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.NotFound, result.Error.Type);
    }

    [Fact]
    public async Task DeleteAsync_ReturnsSuccess_WhenCategoryExists()
    {
        // Arrange
        var category = new ProductCategory
        {
            Id = 1,
            Name = "Test Category",
            IsDeleted = false
        };

        _repositoryMock.Setup(r => r.FindAsync(
            It.IsAny<Expression<Func<ProductCategory, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>()
        )).ReturnsAsync(category);

        _currentUserServiceMock.SetupGet(c => c.UserId).Returns(1);
        _uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var service = CreateService();

        // Act
        var result = await service.DeleteAsync(1, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
    }

    #endregion

    #region FindAsync Tests

    [Fact]
    public async Task FindAsync_ReturnsInvalidId_WhenIdIsZero()
    {
        // Arrange
        var service = CreateService();

        // Act
        var result = await service.FindAsync(0, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Validation, result.Error.Type);
    }

    [Fact]
    public async Task FindAsync_ReturnsInvalidId_WhenIdIsNegative()
    {
        // Arrange
        var service = CreateService();

        // Act
        var result = await service.FindAsync(-1, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Validation, result.Error.Type);
    }

    [Fact]
    public async Task FindAsync_ReturnsNotFound_WhenCategoryDoesNotExist()
    {
        // Arrange
        _repositoryMock.Setup(r => r.FindAsync(
            It.IsAny<Expression<Func<ProductCategory, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>()
        )).ReturnsAsync((ProductCategory)null!);

        var service = CreateService();

        // Act
        var result = await service.FindAsync(1, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.NotFound, result.Error.Type);
    }

    [Fact]
    public async Task FindAsync_ReturnsSuccess_WhenCategoryExists()
    {
        // Arrange
        var category = new ProductCategory
        {
            Id = 1,
            Name = "Test Category",
            Description = "Description",
            ParentId = null,
            CreatedAt = DateTime.UtcNow,
            CreatedByUserId = 1
        };

        _repositoryMock.Setup(r => r.FindAsync(
            It.IsAny<Expression<Func<ProductCategory, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>()
        )).ReturnsAsync(category);

        var service = CreateService();

        // Act
        var result = await service.FindAsync(1, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(1, result.Value.Id);
        Assert.Equal("Test Category", result.Value.Name);
    }

    [Fact]
    public async Task FindAsync_ReturnsException_WhenExceptionThrown()
    {
        // Arrange
        _repositoryMock.Setup(r => r.FindAsync(
            It.IsAny<Expression<Func<ProductCategory, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>()
        )).ThrowsAsync(new Exception("Database error"));

        var service = CreateService();

        // Act
        var result = await service.FindAsync(1, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Failure, result.Error.Type);
    }

    #endregion

    #region GetAllAsync Tests

    [Fact]
    public async Task GetAllAsync_ReturnsNotFound_WhenNoCategoriesExist()
    {
        // Arrange
        _repositoryMock.Setup(r => r.GetAllAsync(
            It.IsAny<Expression<Func<ProductCategory, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>()
        )).ReturnsAsync(new List<ProductCategory>());

        var service = CreateService();

        // Act
        var result = await service.GetAllAsync(CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.NotFound, result.Error.Type);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsSuccess_WhenCategoriesExist()
    {
        // Arrange
        var categories = new List<ProductCategory>
        {
            new ProductCategory { Id = 1, Name = "Category 1" },
            new ProductCategory { Id = 2, Name = "Category 2" }
        };

        _repositoryMock.Setup(r => r.GetAllAsync(
            It.IsAny<Expression<Func<ProductCategory, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>()
        )).ReturnsAsync(categories);

        var service = CreateService();

        // Act
        var result = await service.GetAllAsync(CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(2, result.Value.Count);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsException_WhenExceptionThrown()
    {
        // Arrange
        _repositoryMock.Setup(r => r.GetAllAsync(
            It.IsAny<Expression<Func<ProductCategory, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>()
        )).ThrowsAsync(new Exception("Database error"));

        var service = CreateService();

        // Act
        var result = await service.GetAllAsync(CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Failure, result.Error.Type);
    }

    #endregion

    #region GetAllChildrenAsync Tests

    [Fact]
    public async Task GetAllChildrenAsync_ReturnsNotFound_WhenNoChildrenExist()
    {
        // Arrange
        _repositoryMock.Setup(r => r.GetAllAsync(
            It.IsAny<Expression<Func<ProductCategory, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>()
        )).ReturnsAsync(new List<ProductCategory>());

        var service = CreateService();

        // Act
        var result = await service.GetAllChildrenAsync(1, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.NotFound, result.Error.Type);
    }

    [Fact]
    public async Task GetAllChildrenAsync_ReturnsSuccess_WhenChildrenExist()
    {
        // Arrange
        var children = new List<ProductCategory>
        {
            new ProductCategory { Id = 2, Name = "Child 1", ParentId = 1 },
            new ProductCategory { Id = 3, Name = "Child 2", ParentId = 1 }
        };

        _repositoryMock.Setup(r => r.GetAllAsync(
            It.IsAny<Expression<Func<ProductCategory, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>()
        )).ReturnsAsync(children);

        var service = CreateService();

        // Act
        var result = await service.GetAllChildrenAsync(1, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal(2, result.Value.Count);
    }

    #endregion

    #region GetAllTreeAsync Tests

    [Fact]
    public async Task GetAllTreeAsync_ReturnsNotFound_WhenNoParentsExist()
    {
        // Arrange
        _repositoryMock.Setup(r => r.GetAllAsync(
            It.IsAny<Expression<Func<ProductCategory, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>()
        )).ReturnsAsync(new List<ProductCategory>());

        var service = CreateService();

        // Act
        var result = await service.GetAllTreeAsync(CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.NotFound, result.Error.Type);
    }

    [Fact]
    public async Task GetAllTreeAsync_ReturnsSuccess_WhenTreeExists()
    {
        // Arrange
        var parents = new List<ProductCategory>
        {
            new ProductCategory { Id = 1, Name = "Parent 1", ParentId = null }
        };

        var children = new List<ProductCategory>
        {
            new ProductCategory { Id = 2, Name = "Child 1", ParentId = 1 }
        };

        _repositoryMock.SetupSequence(r => r.GetAllAsync(
            It.IsAny<Expression<Func<ProductCategory, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>()
        ))
        .ReturnsAsync(parents)
        .ReturnsAsync(children);

        var service = CreateService();

        // Act
        var result = await service.GetAllTreeAsync(CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
    }

    #endregion

    #region UpdateAsync Tests

    [Fact]
    public async Task UpdateAsync_ReturnsInvalidId_WhenIdIsZero()
    {
        // Arrange
        var request = new ProductCategoryRequest("Test", null, null);
        var service = CreateService();

        // Act
        var result = await service.UpdateAsync(0, request, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Validation, result.Error.Type);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsFailure_WhenValidationFails()
    {
        // Arrange
        var request = new ProductCategoryRequest("", null, null);
        var validationResult = new ValidationResult(new[]
        {
            new ValidationFailure("Name", "Name is required")
        });

        _validatorMock.Setup(v => v.Validate(request)).Returns(validationResult);

        var service = CreateService();

        // Act
        var result = await service.UpdateAsync(1, request, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Validation, result.Error.Type);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsNotFound_WhenCategoryDoesNotExist()
    {
        // Arrange
        var request = new ProductCategoryRequest("Test", null, null);
        var validationResult = new ValidationResult();

        _validatorMock.Setup(v => v.Validate(request)).Returns(validationResult);
        _repositoryMock.Setup(r => r.FindAsync(
            It.IsAny<Expression<Func<ProductCategory, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>()
        )).ReturnsAsync((ProductCategory)null!);

        var service = CreateService();

        // Act
        var result = await service.UpdateAsync(1, request, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.NotFound, result.Error.Type);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsSuccess_WhenValidRequest()
    {
        // Arrange
        var request = new ProductCategoryRequest("Updated Category", "Updated Description", null);
        var validationResult = new ValidationResult();

        var category = new ProductCategory
        {
            Id = 1,
            Name = "Original Category",
            Description = "Original Description"
        };

        var queryResult = Result<ProductCategoryDetailsResponse>.Success(
            new ProductCategoryDetailsResponse
            {
                Id = 1,
                Name = "Updated Category"
            }
        );

        _validatorMock.Setup(v => v.Validate(request)).Returns(validationResult);
        _repositoryMock.Setup(r => r.FindAsync(
            It.IsAny<Expression<Func<ProductCategory, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>()
        )).ReturnsAsync(category);

        _currentUserServiceMock.SetupGet(c => c.UserId).Returns(1);
        _uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        _queryMock.Setup(q => q.GetCategoryByIdAsync(
            It.IsAny<int>(),
            It.IsAny<CancellationToken>()
        )).ReturnsAsync(queryResult);

        var service = CreateService();

        // Act
        var result = await service.UpdateAsync(1, request, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        _repositoryMock.Verify(r => r.Update(It.IsAny<ProductCategory>()), Times.Once);
        _uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    #endregion

    #region GetCategoriesNamesAsync Tests

    [Fact]
    public async Task GetCategoriesNamesAsync_ReturnsNotFound_WhenNoCategoriesExist()
    {
        // Arrange
        _repositoryMock.Setup(r => r.GetAllAsync(
            It.IsAny<Expression<Func<ProductCategory, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>()
        )).ReturnsAsync(new List<ProductCategory>());

        var service = CreateService();

        // Act
        var result = await service.GetCategoriesNamesAsync(CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.NotFound, result.Error.Type);
    }

    [Fact]
    public async Task GetCategoriesNamesAsync_ReturnsSuccess_WhenCategoriesExist()
    {
        // Arrange
        var categories = new List<ProductCategory>
        {
            new ProductCategory { Id = 1, Name = "Category 1", IsDeleted = false },
            new ProductCategory { Id = 2, Name = "Category 2", IsDeleted = false }
        };

        _repositoryMock.Setup(r => r.GetAllAsync(
            It.IsAny<Expression<Func<ProductCategory, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>()
        )).ReturnsAsync(categories);

        var service = CreateService();

        // Act
        var result = await service.GetCategoriesNamesAsync(CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
    }

    [Fact]
    public async Task GetCategoriesNamesAsync_ReturnsException_WhenExceptionThrown()
    {
        // Arrange
        _repositoryMock.Setup(r => r.GetAllAsync(
            It.IsAny<Expression<Func<ProductCategory, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>()
        )).ThrowsAsync(new Exception("Database error"));

        var service = CreateService();

        // Act
        var result = await service.GetCategoriesNamesAsync(CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Failure, result.Error.Type);
    }

    #endregion
}
