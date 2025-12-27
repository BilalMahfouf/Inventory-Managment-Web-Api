using Application.Abstractions.Repositories.Base;
using Application.Abstractions.Services.User;
using Application.Abstractions.UnitOfWork;
using Application.DTOs.Users.Request;
using Application.DTOs.Users.Response;
using Application.Results;
using Application.Services.Users;
using Domain.Entities;
using Domain.Enums;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using System.Linq.Expressions;
using Xunit;

namespace Application.Tests.UserTests;

public class UserRoleServiceTests
{
    private readonly Mock<IBaseRepository<UserRole>> _repositoryMock = new();
    private readonly Mock<IUnitOfWork> _uowMock = new();
    private readonly Mock<ICurrentUserService> _currentUserServiceMock = new();
    private readonly Mock<IValidator<UserRoleRequest>> _validatorMock = new();

    private UserRoleService CreateService()
    {
        return new UserRoleService(
            _repositoryMock.Object,
            _uowMock.Object,
            _currentUserServiceMock.Object,
            _validatorMock.Object
        );
    }

    #region GetAllAsync Tests

    [Fact]
    public async Task GetAllAsync_ReturnsNotFound_WhenNoRolesExist()
    {
        // Arrange
        _repositoryMock.Setup(r => r.GetAllAsync(
            null!,
            It.IsAny<CancellationToken>(),
            It.IsAny<string>()
        )).ReturnsAsync(new List<UserRole>());

        var service = CreateService();

        // Act
        var result = await service.GetAllAsync(CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.NotFound, result.ErrorType);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsSuccess_WhenRolesExist()
    {
        // Arrange
        var roles = new List<UserRole>
        {
            new UserRole { Id = 1, Name = "Admin", Description = "Administrator" },
            new UserRole { Id = 2, Name = "User", Description = "Regular User" }
        };

        _repositoryMock.Setup(r => r.GetAllAsync(
            null!,
            It.IsAny<CancellationToken>(),
            It.IsAny<string>()
        )).ReturnsAsync(roles);

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
            null!,
            It.IsAny<CancellationToken>(),
            It.IsAny<string>()
        )).ThrowsAsync(new Exception("Database error"));

        var service = CreateService();

        // Act
        var result = await service.GetAllAsync(CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.InternalServerError, result.ErrorType);
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
        Assert.Equal(ErrorType.BadRequest, result.ErrorType);
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
        Assert.Equal(ErrorType.BadRequest, result.ErrorType);
    }

    [Fact]
    public async Task FindAsync_ReturnsNotFound_WhenRoleDoesNotExist()
    {
        // Arrange
        _repositoryMock.Setup(r => r.FindAsync(
            It.IsAny<Expression<Func<UserRole, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>()
        )).ReturnsAsync((UserRole)null!);

        var service = CreateService();

        // Act
        var result = await service.FindAsync(1, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.NotFound, result.ErrorType);
    }

    [Fact]
    public async Task FindAsync_ReturnsSuccess_WhenRoleExists()
    {
        // Arrange
        var role = new UserRole
        {
            Id = 1,
            Name = "Admin",
            Description = "Administrator role"
        };

        _repositoryMock.Setup(r => r.FindAsync(
            It.IsAny<Expression<Func<UserRole, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>()
        )).ReturnsAsync(role);

        var service = CreateService();

        // Act
        var result = await service.FindAsync(1, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("Admin", result.Value.Name);
    }

    [Fact]
    public async Task FindAsync_ReturnsException_WhenExceptionThrown()
    {
        // Arrange
        _repositoryMock.Setup(r => r.FindAsync(
            It.IsAny<Expression<Func<UserRole, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>()
        )).ThrowsAsync(new Exception("Database error"));

        var service = CreateService();

        // Act
        var result = await service.FindAsync(1, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.InternalServerError, result.ErrorType);
    }

    #endregion

    #region AddAsync Tests

    [Fact]
    public async Task AddAsync_ReturnsFailure_WhenValidationFails()
    {
        // Arrange
        var request = new UserRoleRequest("", null);
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
        Assert.Equal(ErrorType.BadRequest, result.ErrorType);
    }

    [Fact]
    public async Task AddAsync_ReturnsSuccess_WhenValidRequest()
    {
        // Arrange
        var request = new UserRoleRequest("New Role", "New role description");
        var validationResult = new ValidationResult();

        var role = new UserRole
        {
            Id = 1,
            Name = "New Role",
            Description = "New role description",
            IsDeleted = false
        };

        _validatorMock.Setup(v => v.Validate(request)).Returns(validationResult);
        _currentUserServiceMock.SetupGet(c => c.UserId).Returns(1);
        
        // Use callback to capture the added entity and set its ID as EF would
        _repositoryMock.Setup(r => r.Add(It.IsAny<UserRole>()))
            .Callback<UserRole>(r => r.Id = 1);
        
        _uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        
        // FindAsync is called after AddAsync internally to return the created entity
        _repositoryMock.Setup(r => r.FindAsync(
            It.IsAny<Expression<Func<UserRole, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>()
        )).ReturnsAsync(role);

        var service = CreateService();

        // Act
        var result = await service.AddAsync(request, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("New Role", result.Value.Name);
        _repositoryMock.Verify(r => r.Add(It.IsAny<UserRole>()), Times.Once);
        _uowMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AddAsync_ReturnsException_WhenExceptionThrown()
    {
        // Arrange
        var request = new UserRoleRequest("New Role", "Description");
        var validationResult = new ValidationResult();

        _validatorMock.Setup(v => v.Validate(request)).Returns(validationResult);
        _repositoryMock.Setup(r => r.Add(It.IsAny<UserRole>()))
            .Throws(new Exception("Database error"));

        var service = CreateService();

        // Act
        var result = await service.AddAsync(request, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.InternalServerError, result.ErrorType);
    }

    #endregion

    #region UpdateAsync Tests

    [Fact]
    public async Task UpdateAsync_ReturnsFailure_WhenValidationFails()
    {
        // Arrange
        var request = new UserRoleRequest("", null);
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
        Assert.Equal(ErrorType.BadRequest, result.ErrorType);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsNotFound_WhenRoleDoesNotExist()
    {
        // Arrange
        var request = new UserRoleRequest("Updated Role", "Updated description");
        var validationResult = new ValidationResult();

        _validatorMock.Setup(v => v.Validate(request)).Returns(validationResult);
        _repositoryMock.Setup(r => r.FindAsync(
            It.IsAny<Expression<Func<UserRole, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>()
        )).ReturnsAsync((UserRole)null!);

        var service = CreateService();

        // Act
        var result = await service.UpdateAsync(1, request, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.NotFound, result.ErrorType);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsSuccess_WhenValidRequest()
    {
        // Arrange
        var request = new UserRoleRequest("Updated Role", "Updated description");
        var validationResult = new ValidationResult();

        var role = new UserRole
        {
            Id = 1,
            Name = "Original Role",
            Description = "Original description"
        };

        _validatorMock.Setup(v => v.Validate(request)).Returns(validationResult);
        _repositoryMock.Setup(r => r.FindAsync(
            It.IsAny<Expression<Func<UserRole, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>()
        )).ReturnsAsync(role);

        _currentUserServiceMock.SetupGet(c => c.UserId).Returns(1);
        _uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var service = CreateService();

        // Act
        var result = await service.UpdateAsync(1, request, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        _repositoryMock.Verify(r => r.Update(It.IsAny<UserRole>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsException_WhenExceptionThrown()
    {
        // Arrange
        var request = new UserRoleRequest("Updated Role", "Description");
        var validationResult = new ValidationResult();

        _validatorMock.Setup(v => v.Validate(request)).Returns(validationResult);
        _repositoryMock.Setup(r => r.FindAsync(
            It.IsAny<Expression<Func<UserRole, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>()
        )).ThrowsAsync(new Exception("Database error"));

        var service = CreateService();

        // Act
        var result = await service.UpdateAsync(1, request, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.InternalServerError, result.ErrorType);
    }

    #endregion
}
