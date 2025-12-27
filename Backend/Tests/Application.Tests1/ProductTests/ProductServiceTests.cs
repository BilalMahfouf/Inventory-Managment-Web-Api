using Application.Abstractions.Queries;
using Application.Abstractions.Repositories.Base;
using Application.Abstractions.Repositories.Inventories;
using Application.Abstractions.Repositories.Products;
using Application.Abstractions.Services.User;
using Application.Abstractions.UnitOfWork;
using Application.DTOs.Products.Request.Products;
using Application.FluentValidations.Products;
using Application.Results;
using Application.Services.Products;
using Domain.Entities;
using Domain.Entities.Products;
using Domain.Enums;
using Domain.Exceptions;
using Domain.Inventories;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using System.Linq.Expressions;
using Xunit;

namespace Application.Tests.ProductTests;

public class ProductServiceTests
{
    private readonly Mock<IProductRepository> _productRepositoryMock = new();
    private readonly Mock<ICurrentUserService> _currentUserServiceMock = new();
    private readonly Mock<IUnitOfWork> _uowMock = new();
    private readonly Mock<IValidator<ProductCreateRequest>> _createValidatorMock = new();
    private readonly Mock<IValidator<ProductUpdateRequest>> _updateValidatorMock = new();
    private readonly Mock<IProductQueries> _queryMock = new();
    private readonly Mock<IInventoryRepository> _inventoryRepositoryMock = new();
    private readonly Mock<IBaseRepository<ProductSupplier>> _productSupplierRepositoryMock = new();

    private ProductService CreateService()
    {
        var validatorContainer = new ProductValidatorContainer(
            _createValidatorMock.Object,
            _updateValidatorMock.Object
        );

        _uowMock.SetupGet(u => u.Products).Returns(_productRepositoryMock.Object);
        _uowMock.SetupGet(u => u.Inventories).Returns(_inventoryRepositoryMock.Object);
        _uowMock.SetupGet(u => u.ProductSuppliers).Returns(_productSupplierRepositoryMock.Object);

        return new ProductService(
            _productRepositoryMock.Object,
            _currentUserServiceMock.Object,
            _uowMock.Object,
            validatorContainer,
            _queryMock.Object
        );
    }

    #region ActivateAsync Tests

    [Fact]
    public async Task ActivateAsync_ReturnsInvalidId_WhenIdIsZero()
    {
        // Arrange
        var service = CreateService();

        // Act
        var result = await service.ActivateAsync(0, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.BadRequest, result.ErrorType);
    }

    [Fact]
    public async Task ActivateAsync_ReturnsInvalidId_WhenIdIsNegative()
    {
        // Arrange
        var service = CreateService();

        // Act
        var result = await service.ActivateAsync(-1, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.BadRequest, result.ErrorType);
    }

    [Fact]
    public async Task ActivateAsync_ReturnsNotFound_WhenProductDoesNotExist()
    {
        // Arrange
        _productRepositoryMock.Setup(r => r.FindAsync(
            It.IsAny<Expression<Func<Product, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>()
        )).ReturnsAsync((Product)null!);

        var service = CreateService();

        // Act
        var result = await service.ActivateAsync(1, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.NotFound, result.ErrorType);
    }

    [Fact]
    public async Task ActivateAsync_ReturnsConflict_WhenProductAlreadyActive()
    {
        // Arrange
        var product = new Product
        {
            Id = 1,
            Name = "Test Product",
            IsActive = true
        };

        _productRepositoryMock.Setup(r => r.FindAsync(
            It.IsAny<Expression<Func<Product, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>()
        )).ReturnsAsync(product);

        var service = CreateService();

        // Act
        var result = await service.ActivateAsync(1, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Conflict, result.ErrorType);
        Assert.Contains("already active", result.ErrorMessage);
    }

    [Fact]
    public async Task ActivateAsync_ReturnsSuccess_WhenProductCanBeActivated()
    {
        // Arrange
        var product = new Product
        {
            Id = 1,
            Name = "Test Product",
            IsActive = false
        };

        _productRepositoryMock.Setup(r => r.FindAsync(
            It.IsAny<Expression<Func<Product, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>()
        )).ReturnsAsync(product);

        _currentUserServiceMock.SetupGet(c => c.UserId).Returns(1);
        _uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var service = CreateService();

        // Act
        var result = await service.ActivateAsync(1, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.True(product.IsActive);
    }

    #endregion

    #region DeactivateAsync Tests

    [Fact]
    public async Task DeactivateAsync_ReturnsInvalidId_WhenIdIsZero()
    {
        // Arrange
        var service = CreateService();

        // Act
        var result = await service.DeactivateAsync(0, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.BadRequest, result.ErrorType);
    }

    [Fact]
    public async Task DeactivateAsync_ReturnsConflict_WhenProductAlreadyInactive()
    {
        // Arrange
        var product = new Product
        {
            Id = 1,
            Name = "Test Product",
            IsActive = false
        };

        _productRepositoryMock.Setup(r => r.FindAsync(
            It.IsAny<Expression<Func<Product, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>()
        )).ReturnsAsync(product);

        var service = CreateService();

        // Act
        var result = await service.DeactivateAsync(1, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Conflict, result.ErrorType);
        Assert.Contains("already inactive", result.ErrorMessage);
    }

    [Fact]
    public async Task DeactivateAsync_ReturnsSuccess_WhenProductCanBeDeactivated()
    {
        // Arrange
        var product = new Product
        {
            Id = 1,
            Name = "Test Product",
            IsActive = true
        };

        _productRepositoryMock.Setup(r => r.FindAsync(
            It.IsAny<Expression<Func<Product, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>()
        )).ReturnsAsync(product);

        _currentUserServiceMock.SetupGet(c => c.UserId).Returns(1);
        _uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var service = CreateService();

        // Act
        var result = await service.DeactivateAsync(1, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.False(product.IsActive);
    }

    #endregion

    #region CreateAsync Tests

    [Fact]
    public async Task CreateAsync_ReturnsFailure_WhenValidationFails()
    {
        // Arrange
        var request = new ProductCreateRequest();
        var validationResult = new ValidationResult(new[]
        {
            new ValidationFailure("Name", "Name is required")
        });

        _createValidatorMock.Setup(v => v.Validate(request)).Returns(validationResult);

        var service = CreateService();

        // Act
        var result = await service.CreateAsync(request, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.BadRequest, result.ErrorType);
    }

    [Fact]
    public async Task CreateAsync_ReturnsConflict_WhenSkuAlreadyExists()
    {
        // Arrange
        var request = new ProductCreateRequest
        {
            SKU = "EXISTING-SKU",
            Name = "Test Product",
            CategoryId = 1,
            UnitOfMeasureId = 1,
            UnitPrice = 10.00m,
            CostPrice = 5.00m,
            LocationId = 1,
            QuantityOnHand = 100,
            ReorderLevel = 10,
            MaxLevel = 500
        };

        var validationResult = new ValidationResult();

        _createValidatorMock.Setup(v => v.Validate(request)).Returns(validationResult);
        _productRepositoryMock.Setup(r => r.IsExistAsync(
            It.IsAny<Expression<Func<Product, bool>>>(),
            It.IsAny<CancellationToken>()
        )).ReturnsAsync(true);

        var service = CreateService();

        // Act
        var result = await service.CreateAsync(request, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Conflict, result.ErrorType);
        Assert.Contains("SKU already exists", result.ErrorMessage);
    }

    [Fact]
    public async Task CreateAsync_ReturnsException_WhenExceptionThrown()
    {
        // Arrange
        var request = new ProductCreateRequest
        {
            SKU = "TEST-SKU",
            Name = "Test Product"
        };

        var validationResult = new ValidationResult();

        _createValidatorMock.Setup(v => v.Validate(request)).Returns(validationResult);
        _productRepositoryMock.Setup(r => r.IsExistAsync(
            It.IsAny<Expression<Func<Product, bool>>>(),
            It.IsAny<CancellationToken>()
        )).ThrowsAsync(new Exception("Database error"));

        var service = CreateService();

        // Act
        var result = await service.CreateAsync(request, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.InternalServerError, result.ErrorType);
    }

    [Fact]
    public async Task CreateAsync_ReturnsConflict_WhenDomainExceptionThrown()
    {
        // Arrange
        var request = new ProductCreateRequest
        {
            SKU = "TEST-SKU",
            Name = "Test Product"
        };

        var validationResult = new ValidationResult();

        _createValidatorMock.Setup(v => v.Validate(request)).Returns(validationResult);
        _productRepositoryMock.Setup(r => r.IsExistAsync(
            It.IsAny<Expression<Func<Product, bool>>>(),
            It.IsAny<CancellationToken>()
        )).ThrowsAsync(new DomainException("Domain error"));

        var service = CreateService();

        // Act
        var result = await service.CreateAsync(request, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Conflict, result.ErrorType);
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
        Assert.Equal(ErrorType.BadRequest, result.ErrorType);
    }

    [Fact]
    public async Task DeleteAsync_ReturnsNotFound_WhenProductDoesNotExist()
    {
        // Arrange
        _productRepositoryMock.Setup(r => r.FindAsync(
            It.IsAny<Expression<Func<Product, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>()
        )).ReturnsAsync((Product)null!);

        var service = CreateService();

        // Act
        var result = await service.DeleteAsync(1, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.NotFound, result.ErrorType);
    }

    [Fact]
    public async Task DeleteAsync_ReturnsSuccess_WhenProductExists()
    {
        // Arrange
        var product = new Product
        {
            Id = 1,
            Name = "Test Product",
            IsDeleted = false
        };

        _productRepositoryMock.Setup(r => r.FindAsync(
            It.IsAny<Expression<Func<Product, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>()
        )).ReturnsAsync(product);

        _currentUserServiceMock.SetupGet(c => c.UserId).Returns(1);
        _uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var service = CreateService();

        // Act
        var result = await service.DeleteAsync(1, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
    }

    #endregion

    #region UpdateAsync Tests

    [Fact]
    public async Task UpdateAsync_ReturnsInvalidId_WhenIdIsZero()
    {
        // Arrange
        var request = new ProductUpdateRequest();
        var service = CreateService();

        // Act
        var result = await service.UpdateAsync(0, request, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.BadRequest, result.ErrorType);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsFailure_WhenValidationFails()
    {
        // Arrange
        var request = new ProductUpdateRequest();
        var validationResult = new ValidationResult(new[]
        {
            new ValidationFailure("Name", "Name is required")
        });

        _updateValidatorMock.Setup(v => v.Validate(request)).Returns(validationResult);

        var service = CreateService();

        // Act
        var result = await service.UpdateAsync(1, request, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.BadRequest, result.ErrorType);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsNotFound_WhenProductDoesNotExist()
    {
        // Arrange
        var request = new ProductUpdateRequest
        {
            Name = "Updated Product",
            LocationId = 1
        };

        var validationResult = new ValidationResult();

        _updateValidatorMock.Setup(v => v.Validate(request)).Returns(validationResult);
        _productRepositoryMock.Setup(r => r.FindAsync(
            It.IsAny<Expression<Func<Product, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>()
        )).ReturnsAsync((Product)null!);

        var service = CreateService();

        // Act
        var result = await service.UpdateAsync(1, request, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.NotFound, result.ErrorType);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsNotFound_WhenInventoryDoesNotExist()
    {
        // Arrange
        var request = new ProductUpdateRequest
        {
            Name = "Updated Product",
            LocationId = 1
        };

        var product = new Product
        {
            Id = 1,
            Name = "Test Product",
            IsDeleted = false
        };

        var validationResult = new ValidationResult();

        _updateValidatorMock.Setup(v => v.Validate(request)).Returns(validationResult);
        _productRepositoryMock.Setup(r => r.FindAsync(
            It.IsAny<Expression<Func<Product, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>()
        )).ReturnsAsync(product);

        _inventoryRepositoryMock.Setup(r => r.FindAsync(
            It.IsAny<Expression<Func<Inventory, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>()
        )).ReturnsAsync((Inventory)null!);

        var service = CreateService();

        // Act
        var result = await service.UpdateAsync(1, request, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.NotFound, result.ErrorType);
    }

    #endregion

    #region FindProductSuppliersAsync Tests

    [Fact]
    public async Task FindProductSuppliersAsync_ReturnsInvalidId_WhenIdIsZero()
    {
        // Arrange
        var service = CreateService();

        // Act
        var result = await service.FindProductSuppliersAsync(0, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.BadRequest, result.ErrorType);
    }

    [Fact]
    public async Task FindProductSuppliersAsync_ReturnsNotFound_WhenNoSuppliersExist()
    {
        // Arrange
        _productSupplierRepositoryMock.Setup(r => r.GetAllAsync(
            It.IsAny<Expression<Func<ProductSupplier, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>()
        )).ReturnsAsync(new List<ProductSupplier>());

        var service = CreateService();

        // Act
        var result = await service.FindProductSuppliersAsync(1, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.NotFound, result.ErrorType);
    }

    [Fact]
    public async Task FindProductSuppliersAsync_ReturnsSuccess_WhenSuppliersExist()
    {
        // Arrange
        var suppliers = new List<ProductSupplier>
        {
            new ProductSupplier
            {
                Id = 1,
                ProductId = 1,
                SupplierId = 1,
                Product = new Product { Name = "Test Product" },
                Supplier = new Supplier { Name = "Test Supplier" },
                SupplierProductCode = "SUP-001",
                LeadTimeDays = 7,
                MinOrderQuantity = 10
            }
        };

        _productSupplierRepositoryMock.Setup(r => r.GetAllAsync(
            It.IsAny<Expression<Func<ProductSupplier, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>()
        )).ReturnsAsync(suppliers);

        var service = CreateService();

        // Act
        var result = await service.FindProductSuppliersAsync(1, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Single(result.Value);
    }

    #endregion

    #region GetProductsWithLowStockAsync Tests

    [Fact]
    public async Task GetProductsWithLowStockAsync_ReturnsNotFound_WhenNoLowStockProducts()
    {
        // Arrange
        _inventoryRepositoryMock.Setup(r => r.GetAllAsync(
            It.IsAny<Expression<Func<Inventory, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>()
        )).ReturnsAsync(new List<Inventory>());

        var service = CreateService();

        // Act
        var result = await service.GetProductsWithLowStockAsync(CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.NotFound, result.ErrorType);
    }

    [Fact]
    public async Task GetProductsWithLowStockAsync_ReturnsSuccess_WhenLowStockProductsExist()
    {
        // Arrange
        var lowStockInventories = new List<Inventory>
        {
            new Inventory
            {
                Id = 1,
                ProductId = 1,
                LocationId = 1,
                QuantityOnHand = 5,
                ReorderLevel = 10,
                Product = new Product { Name = "Low Stock Product" },
                Location = new Location { Name = "Warehouse A" }
            }
        };

        _inventoryRepositoryMock.Setup(r => r.GetAllAsync(
            It.IsAny<Expression<Func<Inventory, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>()
        )).ReturnsAsync(lowStockInventories);

        var service = CreateService();

        // Act
        var result = await service.GetProductsWithLowStockAsync(CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Single(result.Value);
    }

    #endregion

    #region FindProductInInventoryAsync Tests

    [Fact]
    public async Task FindProductInInventoryAsync_ReturnsInvalidId_WhenIdIsZero()
    {
        // Arrange
        var service = CreateService();

        // Act
        var result = await service.FindProductInInventoryAsync(0, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.BadRequest, result.ErrorType);
    }

    [Fact]
    public async Task FindProductInInventoryAsync_ReturnsNotFound_WhenNoInventoriesExist()
    {
        // Arrange
        _inventoryRepositoryMock.Setup(r => r.GetAllAsync(
            It.IsAny<Expression<Func<Inventory, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>()
        )).ReturnsAsync(new List<Inventory>());

        var service = CreateService();

        // Act
        var result = await service.FindProductInInventoryAsync(1, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.NotFound, result.ErrorType);
    }

    [Fact]
    public async Task FindProductInInventoryAsync_ReturnsSuccess_WhenInventoriesExist()
    {
        // Arrange
        var inventories = new List<Inventory>
        {
            new Inventory
            {
                Id = 1,
                ProductId = 1,
                LocationId = 1,
                QuantityOnHand = 100,
                ReorderLevel = 10,
                MaxLevel = 500,
                Product = new Product { Name = "Test Product" },
                Location = new Location { Name = "Warehouse A" }
            }
        };

        _inventoryRepositoryMock.Setup(r => r.GetAllAsync(
            It.IsAny<Expression<Func<Inventory, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>()
        )).ReturnsAsync(inventories);

        var service = CreateService();

        // Act
        var result = await service.FindProductInInventoryAsync(1, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Single(result.Value);
    }

    #endregion

    #region GetAllAsync Tests

    [Fact]
    public async Task GetAllAsync_ReturnsSuccess_WhenProductsExist()
    {
        // Arrange
        var products = new List<Product>
        {
            new Product
            {
                Id = 1,
                Sku = "TEST-001",
                Name = "Test Product",
                Category = new ProductCategory { Name = "Category" },
                UnitOfMeasure = new UnitOfMeasure { Name = "Unit" }
            }
        };

        _productRepositoryMock.Setup(r => r.GetAllWithPaginationAsync(
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>()
        )).ReturnsAsync(products);

        _productRepositoryMock.Setup(r => r.GetCountAsync(
            It.IsAny<Expression<Func<Product, bool>>>(),
            It.IsAny<CancellationToken>()
        )).ReturnsAsync(1);

        var service = CreateService();

        // Act
        var result = await service.GetAllAsync(1, 10, null, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsException_WhenExceptionThrown()
    {
        // Arrange
        _productRepositoryMock.Setup(r => r.GetAllWithPaginationAsync(
            It.IsAny<int>(),
            It.IsAny<int>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>()
        )).ThrowsAsync(new Exception("Database error"));

        var service = CreateService();

        // Act
        var result = await service.GetAllAsync(1, 10, null, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.InternalServerError, result.ErrorType);
    }

    #endregion
}
