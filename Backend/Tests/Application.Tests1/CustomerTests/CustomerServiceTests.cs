using Application.Products.Contracts;
using Application.Shared.Contracts;
using Application.Users.Contracts;
using Application.Customers;
using Application.Customers.Dtos;
using Domain.Shared.Results;
using Domain.Shared.Entities;
using Domain.Shared.Errors;
using Domain.Shared.Exceptions;
using Domain.Shared.ValueObjects;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using System.Linq.Expressions;
using Xunit;

namespace Application.Tests.CustomerTests;

public class CustomerServiceTests
{
    private readonly Mock<ICurrentUserService> _currentUserServiceMock = new();
    private readonly Mock<IUnitOfWork> _uowMock = new();
    private readonly Mock<IValidator<CustomerCreateRequest>> _createValidatorMock = new();
    private readonly Mock<ICustomerQueries> _queryMock = new();
    private readonly Mock<IBaseRepository<Customer>> _customerRepositoryMock = new();
    private readonly Mock<IBaseRepository<CustomerCategory>> _customerCategoryRepositoryMock = new();

    private CustomerService CreateService()
    {
        _uowMock.SetupGet(u => u.Customers).Returns(_customerRepositoryMock.Object);
        _uowMock.SetupGet(u => u.CustomerCategories).Returns(_customerCategoryRepositoryMock.Object);

        return new CustomerService(
            _currentUserServiceMock.Object,
            _uowMock.Object,
            _createValidatorMock.Object,
            _queryMock.Object
        );
    }

    #region AddAsync Tests

    [Fact]
    public async Task AddAsync_ReturnsFailure_WhenValidationFails()
    {
        // Arrange
        var request = new CustomerCreateRequest
        {
            Name = "",
            CustomerCategoryId = 1,
            Email = "invalid-email"
        };

        var validationResult = new ValidationResult(new[]
        {
            new ValidationFailure("Name", "Name is required"),
            new ValidationFailure("Email", "Email is invalid")
        });

        _createValidatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);

        var service = CreateService();

        // Act
        var result = await service.AddAsync(request, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Validation, result.Error.Type);
        Assert.Contains("Name is required", result.Error.Description);
    }

    [Fact]
    public async Task AddAsync_ReturnsNotFound_WhenCustomerCategoryDoesNotExist()
    {
        // Arrange
        var request = new CustomerCreateRequest
        {
            Name = "Test Customer",
            CustomerCategoryId = 999,
            Email = "test@test.com"
        };

        var validationResult = new ValidationResult();

        _createValidatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);

        _customerCategoryRepositoryMock.Setup(r => r.FindAsync(
            It.IsAny<Expression<Func<CustomerCategory, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>()
        )).ReturnsAsync((CustomerCategory)null!);

        var service = CreateService();

        // Act
        var result = await service.AddAsync(request, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.NotFound, result.Error.Type);
        Assert.Contains("CustomerCategory", result.Error.Description);
    }

    [Fact]
    public async Task AddAsync_ReturnsSuccess_WhenValidRequest()
    {
        // Arrange
        var request = new CustomerCreateRequest
        {
            Name = "Test Customer",
            CustomerCategoryId = 1,
            Email = "test@test.com",
            Phone = "1234567890",
            Street = "123 Main St",
            City = "Test City",
            State = "TS",
            ZipCode = "12345"
        };

        var validationResult = new ValidationResult();

        var customerCategory = new CustomerCategory
        {
            Id = 1,
            Name = "Retail",
            DefaultCreditLimit = 5000,
            DefaultPaymentTerms = "Net 30"
        };

        var customerReadResponse = new CustomerReadResponse
        {
            Id = 1,
            Name = "Test Customer",
            Email = "test@test.com"
        };

        _createValidatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);

        _customerCategoryRepositoryMock.Setup(r => r.FindAsync(
            It.IsAny<Expression<Func<CustomerCategory, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>()
        )).ReturnsAsync(customerCategory);

        _customerRepositoryMock.Setup(r => r.Add(It.IsAny<Customer>()));

        _uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        _queryMock.Setup(q => q.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<CustomerReadResponse>.Success(customerReadResponse));

        var service = CreateService();

        // Act
        var result = await service.AddAsync(request, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        _customerRepositoryMock.Verify(r => r.Add(It.IsAny<Customer>()), Times.Once);
        _uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AddAsync_ReturnsConflict_WhenDomainExceptionThrown()
    {
        // Arrange
        var request = new CustomerCreateRequest
        {
            Name = "Test Customer",
            CustomerCategoryId = 1,
            Email = "test@test.com"
        };

        var validationResult = new ValidationResult();

        _createValidatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);

        _customerCategoryRepositoryMock.Setup(r => r.FindAsync(
            It.IsAny<Expression<Func<CustomerCategory, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>()
        )).ThrowsAsync(new DomainException("Domain error"));

        var service = CreateService();

        // Act
        var result = await service.AddAsync(request, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Conflict, result.Error.Type);
    }

    [Fact]
    public async Task AddAsync_ReturnsException_WhenUnexpectedExceptionThrown()
    {
        // Arrange
        var request = new CustomerCreateRequest
        {
            Name = "Test Customer",
            CustomerCategoryId = 1,
            Email = "test@test.com"
        };

        var validationResult = new ValidationResult();

        _createValidatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);

        _customerCategoryRepositoryMock.Setup(r => r.FindAsync(
            It.IsAny<Expression<Func<CustomerCategory, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>()
        )).ThrowsAsync(new Exception("Database error"));

        var service = CreateService();

        // Act
        var result = await service.AddAsync(request, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Failure, result.Error.Type);
    }

    #endregion

    #region UpdateAsync Tests

    [Fact]
    public async Task UpdateAsync_ReturnsInvalidId_WhenIdIsZero()
    {
        // Arrange
        var request = new UpdateCustomerRequest
        {
            Name = "Updated Customer"
        };

        var service = CreateService();

        // Act
        var result = await service.UpdateAsync(0, request, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Validation, result.Error.Type);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsInvalidId_WhenIdIsNegative()
    {
        // Arrange
        var request = new UpdateCustomerRequest
        {
            Name = "Updated Customer"
        };

        var service = CreateService();

        // Act
        var result = await service.UpdateAsync(-1, request, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Validation, result.Error.Type);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsNotFound_WhenCustomerDoesNotExist()
    {
        // Arrange
        var request = new UpdateCustomerRequest
        {
            Name = "Updated Customer"
        };

        _customerRepositoryMock.Setup(r => r.FindAsync(
            It.IsAny<Expression<Func<Customer, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>()
        )).ReturnsAsync((Customer)null!);

        var service = CreateService();

        // Act
        var result = await service.UpdateAsync(1, request, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.NotFound, result.Error.Type);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsConflict_WhenDomainExceptionThrown()
    {
        // Arrange
        var request = new UpdateCustomerRequest
        {
            Name = "Updated Customer"
        };

        _customerRepositoryMock.Setup(r => r.FindAsync(
            It.IsAny<Expression<Func<Customer, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>()
        )).ThrowsAsync(new DomainException("Domain error"));

        var service = CreateService();

        // Act
        var result = await service.UpdateAsync(1, request, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Conflict, result.Error.Type);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsException_WhenUnexpectedExceptionThrown()
    {
        // Arrange
        var request = new UpdateCustomerRequest
        {
            Name = "Updated Customer"
        };

        _customerRepositoryMock.Setup(r => r.FindAsync(
            It.IsAny<Expression<Func<Customer, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>()
        )).ThrowsAsync(new Exception("Database error"));

        var service = CreateService();

        // Act
        var result = await service.UpdateAsync(1, request, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Failure, result.Error.Type);
    }

    #endregion
}
