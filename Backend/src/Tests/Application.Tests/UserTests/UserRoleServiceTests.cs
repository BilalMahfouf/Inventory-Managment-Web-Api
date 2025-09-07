using Application.Abstractions.Repositories.Base;
using Application.Abstractions.Services.User;
using Application.Abstractions.UnitOfWork;
using Application.Common.Abstractions;
using Application.DTOs.Users.Request;
using Application.DTOs.Users.Response;
using Application.Results;
using Application.Services.Shared;
using Application.Services.Users;
using Domain.Entities;
using Domain.Enums;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Application.Tests.UserTests
{
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

        [Fact]
        public async Task GetAllAsync_ReturnsNotFound_WhenNoRoles()
        {
            // Arrange
            var service = CreateService();
            _repositoryMock.Setup(r => r.GetAllAsync(
                null,
                It.IsAny<CancellationToken>(),
                It.IsAny<string>()
            )).ReturnsAsync(new List<UserRole>());

            // Act
            var result = await service.GetAllAsync(CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorType.NotFound, result.ErrorType);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsSuccess_WithRoles()
        {
            // Arrange
            var service = CreateService();
            var roles = new List<UserRole>
            {
                new UserRole { Id = 1, Name = "Admin", Description = "Administrator role" },
                new UserRole { Id = 2, Name = "User", Description = "Regular user" }
            };
            _repositoryMock.Setup(r => r.GetAllAsync(
                null,
                It.IsAny<CancellationToken>(),
                It.IsAny<string>()
            )).ReturnsAsync(roles);

            // Act
            var result = await service.GetAllAsync(CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal(2, result.Value.Count);
            Assert.Contains(result.Value, r => r.Name == "Admin");
            Assert.Contains(result.Value, r => r.Name == "User");
        }

        [Fact]
        public async Task GetAllAsync_ReturnsFailure_OnException()
        {
            // Arrange
            var service = CreateService();
            _repositoryMock.Setup(r => r.GetAllAsync(
                null,
                It.IsAny<CancellationToken>(),
                It.IsAny<string>()
            )).ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await service.GetAllAsync(CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorType.InternalServerError, result.ErrorType);
            Assert.Contains("Test exception", result.ErrorMessage);
        }

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
        public async Task FindAsync_ReturnsNotFound_WhenRoleDoesNotExist()
        {
            // Arrange
            var service = CreateService();
            _repositoryMock.Setup(r => r.FindAsync(
                It.IsAny<Expression<Func<UserRole, bool>>>(),
                It.IsAny<CancellationToken>(),
                "CreatedByUser,UpdatedByUser,DeletedByUser"
            )).ReturnsAsync((UserRole)null);

            // Act
            var result = await service.FindAsync(1, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorType.NotFound, result.ErrorType);
        }

        [Fact]
        public async Task FindAsync_ReturnsSuccess_WithRole()
        {
            // Arrange
            var service = CreateService();
            var role = new UserRole
            {
                Id = 1,
                Name = "Admin",
                Description = "Administrator role",
                CreatedByUser = new User { UserName = "creator" }
            };

            _repositoryMock.Setup(r => r.FindAsync(
                It.IsAny<Expression<Func<UserRole, bool>>>(),
                It.IsAny<CancellationToken>(),
                "CreatedByUser,UpdatedByUser,DeletedByUser"
            )).ReturnsAsync(role);

            // Act
            var result = await service.FindAsync(1, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal("Admin", result.Value.Name);
            Assert.Equal("Administrator role", result.Value.Description);
            Assert.Equal("creator", result.Value.CreatedByUserName);
        }

        [Fact]
        public async Task FindAsync_ReturnsFailure_OnException()
        {
            // Arrange
            var service = CreateService();
            _repositoryMock.Setup(r => r.FindAsync(
                It.IsAny<Expression<Func<UserRole, bool>>>(),
                It.IsAny<CancellationToken>(),
                "CreatedByUser,UpdatedByUser,DeletedByUser"
            )).ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await service.FindAsync(1, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorType.InternalServerError, result.ErrorType);
            Assert.Contains("Test exception", result.ErrorMessage);
        }

        [Fact]
        public async Task AddAsync_ReturnsFailure_WhenValidationFails()
        {
            // Arrange
            var service = CreateService();
            var request = new UserRoleRequest ("", "");
            var validationResult = new ValidationResult(new[] {
                new ValidationFailure("Name", "Name is required")
            });
            _validatorMock.Setup(v => v.Validate(request)).Returns(validationResult);

            // Act
            var result = await service.AddAsync(request, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorType.BadRequest, result.ErrorType);
            Assert.Contains("Name is required", result.ErrorMessage);
        }

        [Fact]
        public async Task AddAsync_ReturnsSuccess_WhenValid()
        {
            // Arrange
            var service = CreateService();
            var request = new UserRoleRequest("Admin", "Administrator role");
            var validationResult = new ValidationResult();
            _validatorMock.Setup(v => v.Validate(request)).Returns(validationResult);

            _currentUserServiceMock.SetupGet(c => c.UserId).Returns(1);
            _uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            var createdRole = new UserRole
            {
                Id = 1,
                Name = "Admin",
                Description = "Administrator role",
                CreatedByUser = new User { UserName = "creator" }
            };

            _repositoryMock.Setup(r => r.FindAsync(
                It.IsAny<Expression<Func<UserRole, bool>>>(),
                It.IsAny<CancellationToken>(),
                "CreatedByUser,UpdatedByUser,DeletedByUser"
            )).ReturnsAsync(createdRole);

            // Act
            var result = await service.AddAsync(request, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Value);
            Assert.Equal("Admin", result.Value.Name);
            _repositoryMock.Verify(r => r.Add(It.Is<UserRole>(role =>
                role.Name == "Admin" &&
                role.Description == "Administrator role" &&
                role.CreatedByUserId == 1
            )), Times.Once);
        }

        [Fact]
        public async Task AddAsync_ReturnsFailure_OnException()
        {
            // Arrange
            var service = CreateService();
            var request = new UserRoleRequest( "Admin", "Administrator role");
            var validationResult = new ValidationResult();
            _validatorMock.Setup(v => v.Validate(request)).Returns(validationResult);

            _repositoryMock.Setup(r => r.Add(It.IsAny<UserRole>()))
                .Throws(new Exception("Test exception"));

            // Act
            var result = await service.AddAsync(request, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorType.InternalServerError, result.ErrorType);
            Assert.Contains("Test exception", result.ErrorMessage);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsFailure_WhenValidationFails()
        {
            // Arrange
            var service = CreateService();
            var request = new UserRoleRequest("", "");
            var validationResult = new ValidationResult(new[] {
                new ValidationFailure("Name", "Name is required")
            });
            _validatorMock.Setup(v => v.Validate(request)).Returns(validationResult);

            // Act
            var result = await service.UpdateAsync(1, request, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorType.BadRequest, result.ErrorType);
            Assert.Contains("Name is required", result.ErrorMessage);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsNotFound_WhenRoleDoesNotExist()
        {
            // Arrange
            var service = CreateService();
            var request = new UserRoleRequest ("Updated Role", "Updated description");
            var validationResult = new ValidationResult();
            _validatorMock.Setup(v => v.Validate(request)).Returns(validationResult);

            _repositoryMock.Setup(r => r.FindAsync(
                It.IsAny<Expression<Func<UserRole, bool>>>(),
                It.IsAny<CancellationToken>(),
                ""
            )).ReturnsAsync((UserRole)null);

            // Act
            var result = await service.UpdateAsync(1, request, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorType.NotFound, result.ErrorType);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsSuccess_WhenValid()
        {
            // Arrange
            var service = CreateService();
            var request = new UserRoleRequest ("Updated Role", "Updated description");
            var validationResult = new ValidationResult();
            _validatorMock.Setup(v => v.Validate(request)).Returns(validationResult);

            var existingRole = new UserRole { Id = 1, Name = "Old Role", Description = "Old description" };
            _repositoryMock.Setup(r => r.FindAsync(
                It.IsAny<Expression<Func<UserRole, bool>>>(),
                It.IsAny<CancellationToken>(),
                ""
            )).ReturnsAsync(existingRole);

            _currentUserServiceMock.SetupGet(c => c.UserId).Returns(2);
            _uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            // Act
            var result = await service.UpdateAsync(1, request, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            _repositoryMock.Verify(r => r.Update(It.Is<UserRole>(role =>
                role.Name == "Updated Role" &&
                role.Description == "Updated description" &&
                role.UpdatedByUserId == 2
            )), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsFailure_OnException()
        {
            // Arrange
            var service = CreateService();
            var request = new UserRoleRequest ("Updated Role", "Updated description");
            var validationResult = new ValidationResult();
            _validatorMock.Setup(v => v.Validate(request)).Returns(validationResult);

            var existingRole = new UserRole { Id = 1, Name = "Old Role", Description = "Old description" };
            _repositoryMock.Setup(r => r.FindAsync(
                It.IsAny<Expression<Func<UserRole, bool>>>(),
                It.IsAny<CancellationToken>(),
                ""
            )).ReturnsAsync(existingRole);

            _repositoryMock.Setup(r => r.Update(It.IsAny<UserRole>()))
                .Throws(new Exception("Test exception"));

            // Act
            var result = await service.UpdateAsync(1, request, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorType.InternalServerError, result.ErrorType);
            Assert.Contains("Test exception", result.ErrorMessage);
        }

        [Fact]
        public async Task SoftDeleteAsync_ReturnsInvalidId_WhenIdIsZero()
        {
            // Arrange
            var service = CreateService();

            // Act
            var result = await service.SoftDeleteAsync(0, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorType.BadRequest, result.ErrorType);
        }

        [Fact]
        public async Task SoftDeleteAsync_ReturnsNotFound_WhenRoleDoesNotExist()
        {
            // Arrange
            var service = CreateService();
            _repositoryMock.Setup(r => r.FindAsync(
                It.IsAny<Expression<Func<UserRole, bool>>>(),
                It.IsAny<CancellationToken>(),
                "Users"
            )).ReturnsAsync((UserRole)null);

            // Act
            var result = await service.SoftDeleteAsync(1, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ErrorType.NotFound, result.ErrorType);
        }

        [Fact]
        public async Task SoftDeleteAsync_Success()
        {
            // Arrange
            var service = CreateService();
            var role = new UserRole { Id = 1, Name = "Admin" };
            _repositoryMock.Setup(r => r.FindAsync(
                It.IsAny<Expression<Func<UserRole, bool>>>(),
                It.IsAny<CancellationToken>(),
                "Users"
            )).ReturnsAsync(role);

            _currentUserServiceMock.SetupGet(c => c.UserId).Returns(2);
            _uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            // Act
            var result = await service.SoftDeleteAsync(1, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            _repositoryMock.Verify(r => r.Update(It.Is<UserRole>(r =>
                r.IsDeleted == true &&
                r.DeletedByUserId == 2 &&
                r.DeletedAt != null
            )), Times.Once);
        }
    }
}