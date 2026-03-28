using Application.Shared.Contracts;
using Application.Customers;
using Domain.Shared.Results;
using Domain.Shared.Entities;
using Domain.Shared.Enums;
using Moq;
using Xunit;

namespace Application.Tests.CustomerTests;

public class CustomerCategoryServiceTests
{
    private readonly Mock<IUnitOfWork> _uowMock = new();
    private readonly Mock<IBaseRepository<CustomerCategory>> _customerCategoryRepositoryMock = new();

    private CustomerCategoryService CreateService()
    {
        _uowMock.SetupGet(u => u.CustomerCategories).Returns(_customerCategoryRepositoryMock.Object);

        return new CustomerCategoryService(_uowMock.Object);
    }

    #region GetCategoriesNamesAsync Tests

    [Fact]
    public async Task GetCategoriesNamesAsync_ReturnsNotFound_WhenNoCategoriesExist()
    {
        // Arrange
        _customerCategoryRepositoryMock.Setup(r => r.GetAllAsync(
            null!,
            It.IsAny<CancellationToken>(),
            It.IsAny<string>()
        )).ReturnsAsync(new List<CustomerCategory>());

        var service = CreateService();

        // Act
        var result = await service.GetCategoriesNamesAsync(CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.NotFound, result.ErrorType);
    }

    [Fact]
    public async Task GetCategoriesNamesAsync_ReturnsSuccess_WhenCategoriesExist()
    {
        // Arrange
        var categories = new List<CustomerCategory>
        {
            new CustomerCategory { Id = 1, Name = "Retail" },
            new CustomerCategory { Id = 2, Name = "Wholesale" },
            new CustomerCategory { Id = 3, Name = "Enterprise" }
        };

        _customerCategoryRepositoryMock.Setup(r => r.GetAllAsync(
            null!,
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
        _customerCategoryRepositoryMock.Setup(r => r.GetAllAsync(
            null!,
            It.IsAny<CancellationToken>(),
            It.IsAny<string>()
        )).ThrowsAsync(new Exception("Database error"));

        var service = CreateService();

        // Act
        var result = await service.GetCategoriesNamesAsync(CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.InternalServerError, result.ErrorType);
    }

    [Fact]
    public async Task GetCategoriesNamesAsync_ReturnsCorrectData()
    {
        // Arrange
        var categories = new List<CustomerCategory>
        {
            new CustomerCategory { Id = 1, Name = "Retail" },
            new CustomerCategory { Id = 2, Name = "Wholesale" }
        };

        _customerCategoryRepositoryMock.Setup(r => r.GetAllAsync(
            null!,
            It.IsAny<CancellationToken>(),
            It.IsAny<string>()
        )).ReturnsAsync(categories);

        var service = CreateService();

        // Act
        var result = await service.GetCategoriesNamesAsync(CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        // The result should contain objects with Id and Name properties
    }

    #endregion
}
