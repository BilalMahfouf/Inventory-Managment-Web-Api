//using Application.Abstractions.Repositories.Base;
//using Application.Abstractions.Services.User;
//using Application.Abstractions.UnitOfWork;
//using Application.Common.Abstractions;
//using Application.DTOs.Users.Request;
//using Application.DTOs.Users.Response;
//using Application.FluentValidations.User;
//using Application.Results;
//using Domain.Entities;
//using Domain.Enums;
//using Moq;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading;
//using System.Threading.Tasks;
//using Xunit;
//using FluentValidation;
//using System.Linq.Expressions;

//namespace Application.Services.Users.Tests
//{
//    public class UserServiceTests
//    {
//        private readonly Mock<IBaseRepository<User>> _userRepositoryMock = new();
//        private readonly Mock<IUnitOfWork> _uowMock = new();
//        private readonly Mock<ICurrentUserService> _currentUserServiceMock = new();
//        private readonly Mock<IPasswordHasher> _hasherMock = new();
//        private readonly Mock<IValidator<UserCreateRequest>> _userCreateValidatorMock = new();
//        private readonly Mock<IValidator<UserUpdateRequest>> _userUpdateValidatorMock = new();
//        private readonly Mock<IValidator<ChangePasswordRequest>> _changePasswordValidatorMock = new();

//        private UserService CreateService()
//        {
//            var validatorContainer = new UserRequestValidatorContainer(
//                _userCreateValidatorMock.Object,
//                _userUpdateValidatorMock.Object,
//                _changePasswordValidatorMock.Object
//            );
//            return new UserService(
//                _userRepositoryMock.Object,
//                _uowMock.Object,
//                validatorContainer,
//                _currentUserServiceMock.Object,
//                _hasherMock.Object
//            );
//        }

//        [Fact]
//        public async Task ActivateAsync_ReturnsInvalidId_WhenIdIsZero()
//        {
//            var service = CreateService();
//            var result = await service.ActivateAsync(0, CancellationToken.None);
//            Assert.False(result.IsSuccess);
//            Assert.Equal(ErrorType.BadRequest, result.ErrorType);
//        }

//        [Fact]
//        public async Task ActivateAsync_ReturnsNotFound_WhenUserDoesNotExist()
//        {
//            var service = CreateService();
//            _userRepositoryMock.Setup(r => r.FindAsync(
//                It.IsAny<Expression<Func<Users, bool>>>(),
//                It.IsAny<CancellationToken>(),
//                ""
//            )).ReturnsAsync((Users)null);
//            var result = await service.ActivateAsync(1, CancellationToken.None);
//            Assert.False(result.IsSuccess);
//            Assert.Equal(ErrorType.NotFound, result.ErrorType);
//        }

//        [Fact]
//        public async Task ActivateAsync_Success()
//        {
//            var service = CreateService();
//            var user = new User { Id = 1 };
//            _userRepositoryMock.Setup(r => r.FindAsync(
//                It.IsAny<Expression<Func<Users, bool>>>(),
//                It.IsAny<CancellationToken>(),
//                ""
//            )).ReturnsAsync(user);
//            _currentUserServiceMock.SetupGet(c => c.UserId).Returns(2);
//            _uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

//            var result = await service.ActivateAsync(1, CancellationToken.None);

//            Assert.True(result.IsSuccess);
//        }

//        [Fact]
//        public async Task AddAsync_ReturnsFailure_WhenValidationFails()
//        {
//            var service = CreateService();
//            var request = new UserCreateRequest();
//            var validationResult = new FluentValidation.Results.ValidationResult(new[] {
//                new FluentValidation.Results.ValidationFailure("UserName", "Required")
//            });
//            _userCreateValidatorMock.Setup(v => v.Validate(request)).Returns(validationResult);

//            var result = await service.AddAsync(request, CancellationToken.None);

//            Assert.False(result.IsSuccess);
//            Assert.Equal(ErrorType.BadRequest, result.ErrorType);
//        }

//        [Fact]
//        public async Task AddAsync_Success()
//        {
//            var service = CreateService();
//            var request = new UserCreateRequest { UserName = "test", Email = "test@test.com", Password = "pass", FirstName = "A", LastName = "B", RoleId = 1 };
//            var validationResult = new FluentValidation.Results.ValidationResult();
//            _userCreateValidatorMock.Setup(v => v.Validate(request)).Returns(validationResult);
//            _hasherMock.Setup(h => h.HashPassword(request.Password)).Returns("hashed");
//            _currentUserServiceMock.SetupGet(c => c.UserId).Returns(1);
//            _uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
//            _userRepositoryMock.Setup(r => r.Add(It.IsAny<Users>()));
//            _userRepositoryMock.Setup(r => r.FindAsync(
//                It.IsAny<Expression<Func<Users, bool>>>(),
//                It.IsAny<CancellationToken>(),
//                "Role,CreatedByUser,UpdatedByUser,DeletedByUser"
//            )).ReturnsAsync(new User { Id = 1, UserName = "unittest", Email = "test@test.com", FirstName = "A", LastName = "B", Role = new UserRole { Name = "Admin" } });

//            var result = await service.AddAsync(request, CancellationToken.None);

//            Assert.True(result.IsSuccess);
//            Assert.NotNull(result.Value);
//            Assert.Equal("test", result.Value.UserName);
//        }

//        [Fact]
//        public async Task ChangePasswordAsync_ReturnsFailure_WhenValidationFails()
//        {
//            var service = CreateService();
//            var request = new ChangePasswordRequest("old", "new", "new");
//            var validationResult = new FluentValidation.Results.ValidationResult(new[] {
//                new FluentValidation.Results.ValidationFailure("OldPassword", "Required")
//            });
//            _changePasswordValidatorMock.Setup(v => v.Validate(request)).Returns(validationResult);

//            var result = await service.ChangePasswordAsync(request, CancellationToken.None);

//            Assert.False(result.IsSuccess);
//            Assert.Equal(ErrorType.BadRequest, result.ErrorType);
//        }

//        [Fact]
//        public async Task ChangePasswordAsync_ReturnsNotFound_WhenUserDoesNotExist()
//        {
//            var service = CreateService();
//            var request = new ChangePasswordRequest("old", "new", "new");
//            var validationResult = new FluentValidation.Results.ValidationResult();
//            _changePasswordValidatorMock.Setup(v => v.Validate(request)).Returns(validationResult);
//            _currentUserServiceMock.SetupGet(c => c.UserId).Returns(1);
//            _userRepositoryMock.Setup(r => r.FindAsync(
//                It.IsAny<Expression<Func<Users, bool>>>(),
//                It.IsAny<CancellationToken>(),
//                ""
//            )).ReturnsAsync((Users)null);

//            var result = await service.ChangePasswordAsync(request, CancellationToken.None);

//            Assert.False(result.IsSuccess);
//            Assert.Equal(ErrorType.NotFound, result.ErrorType);
//        }

//        [Fact]
//        public async Task ChangePasswordAsync_ReturnsFailure_WhenOldPasswordWrong()
//        {
//            var service = CreateService();
//            var request = new ChangePasswordRequest("old", "new", "new");
//            var validationResult = new FluentValidation.Results.ValidationResult();
//            _changePasswordValidatorMock.Setup(v => v.Validate(request)).Returns(validationResult);
//            _currentUserServiceMock.SetupGet(c => c.UserId).Returns(1);
//            var user = new User { Id = 1, PasswordHash = "hashed" };
//            _userRepositoryMock.Setup(r => r.FindAsync(
//                It.IsAny<Expression<Func<Users, bool>>>(),
//                It.IsAny<CancellationToken>(),
//                ""
//            )).ReturnsAsync(user);
//            _hasherMock.Setup(h => h.VerifyPassword("hashed", "old")).Returns(false);

//            var result = await service.ChangePasswordAsync(request, CancellationToken.None);

//            Assert.False(result.IsSuccess);
//            Assert.Equal(ErrorType.BadRequest, result.ErrorType);
//        }

//        [Fact]
//        public async Task ChangePasswordAsync_Success()
//        {
//            var service = CreateService();
//            var request = new ChangePasswordRequest("old", "new", "new");
//            var validationResult = new FluentValidation.Results.ValidationResult();
//            _changePasswordValidatorMock.Setup(v => v.Validate(request)).Returns(validationResult);
//            _currentUserServiceMock.SetupGet(c => c.UserId).Returns(1);
//            var user = new User { Id = 1, PasswordHash = "hashed" };
//            _userRepositoryMock.Setup(r => r.FindAsync(
//                It.IsAny<Expression<Func<Users, bool>>>(),
//                It.IsAny<CancellationToken>(),
//                ""
//            )).ReturnsAsync(user);
//            _hasherMock.Setup(h => h.VerifyPassword("hashed", "old")).Returns(true);
//            _hasherMock.Setup(h => h.HashPassword("new")).Returns("newhashed");
//            _uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

//            var result = await service.ChangePasswordAsync(request, CancellationToken.None);

//            Assert.True(result.IsSuccess);
//        }

//        [Fact]
//        public async Task DeleteAsync_ReturnsInvalidId_WhenIdIsZero()
//        {
//            var service = CreateService();
//            var result = await service.DeleteAsync(0, CancellationToken.None);
//            Assert.False(result.IsSuccess);
//            Assert.Equal(ErrorType.BadRequest, result.ErrorType);
//        }

//        [Fact]
//        public async Task DeleteAsync_ReturnsNotFound_WhenUserDoesNotExist()
//        {
//            var service = CreateService();
//            _userRepositoryMock.Setup(r => r.FindAsync(
//                It.IsAny<Expression<Func<Users, bool>>>(),
//                It.IsAny<CancellationToken>(),
//                ""
//            )).ReturnsAsync((Users)null);

//            var result = await service.DeleteAsync(1, CancellationToken.None);

//            Assert.False(result.IsSuccess);
//            Assert.Equal(ErrorType.NotFound, result.ErrorType);
//        }

//        [Fact]
//        public async Task DeleteAsync_Success()
//        {
//            var service = CreateService();
//            var user = new User { Id = 1 };
//            _userRepositoryMock.Setup(r => r.FindAsync(
//                It.IsAny<Expression<Func<Users, bool>>>(),
//                It.IsAny<CancellationToken>(),
//                ""
//            )).ReturnsAsync(user);
//            _currentUserServiceMock.SetupGet(c => c.UserId).Returns(2);
//            _uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

//            var result = await service.DeleteAsync(1, CancellationToken.None);

//            Assert.True(result.IsSuccess);
//        }

//        [Fact]
//        public async Task DesActivateAsync_ReturnsInvalidId_WhenIdIsZero()
//        {
//            var service = CreateService();
//            var result = await service.DesActivateAsync(0, CancellationToken.None);
//            Assert.False(result.IsSuccess);
//            Assert.Equal(ErrorType.BadRequest, result.ErrorType);
//        }

//        [Fact]
//        public async Task DesActivateAsync_ReturnsNotFound_WhenUserDoesNotExist()
//        {
//            var service = CreateService();
//            _userRepositoryMock.Setup(r => r.FindAsync(
//                It.IsAny<Expression<Func<Users, bool>>>(),
//                It.IsAny<CancellationToken>(),
//                ""
//            )).ReturnsAsync((Users)null);

//            var result = await service.DesActivateAsync(1, CancellationToken.None);

//            Assert.False(result.IsSuccess);
//            Assert.Equal(ErrorType.NotFound, result.ErrorType);
//        }

//        [Fact]
//        public async Task DesActivateAsync_Success()
//        {
//            var service = CreateService();
//            var user = new User { Id = 1 };
//            _userRepositoryMock.Setup(r => r.FindAsync(
//                It.IsAny<Expression<Func<Users, bool>>>(),
//                It.IsAny<CancellationToken>(),
//                ""
//            )).ReturnsAsync(user);
//            _currentUserServiceMock.SetupGet(c => c.UserId).Returns(2);
//            _uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

//            var result = await service.DesActivateAsync(1, CancellationToken.None);

//            Assert.True(result.IsSuccess);
//        }

//        [Fact]
//        public async Task FindByIdAsync_ReturnsInvalidId_WhenIdIsZero()
//        {
//            var service = CreateService();
//            var result = await service.FindByIdAsync(0, CancellationToken.None);
//            Assert.False(result.IsSuccess);
//            Assert.Equal(ErrorType.BadRequest, result.ErrorType);
//        }

//        [Fact]
//        public async Task FindByIdAsync_ReturnsNotFound_WhenUserDoesNotExist()
//        {
//            var service = CreateService();
//            _userRepositoryMock.Setup(r => r.FindAsync(
//                It.IsAny<Expression<Func<Users, bool>>>(),
//                It.IsAny<CancellationToken>(),
//                "Role,CreatedByUser,UpdatedByUser,DeletedByUser"
//            )).ReturnsAsync((Users)null);

//            var result = await service.FindByIdAsync(1, CancellationToken.None);

//            Assert.False(result.IsSuccess);
//            Assert.Equal(ErrorType.NotFound, result.ErrorType);
//        }

//        [Fact]
//        public async Task FindByIdAsync_Success()
//        {
//            var service = CreateService();
//            var user = new User { Id = 1, UserName = "test", Email = "test@test.com", FirstName = "A", LastName = "B", Role = new UserRole { Name = "Admin" } };
//            _userRepositoryMock.Setup(r => r.FindAsync(
//                It.IsAny<Expression<Func<Users, bool>>>(),
//                It.IsAny<CancellationToken>(),
//                "Role,CreatedByUser,UpdatedByUser,DeletedByUser"
//            )).ReturnsAsync(user);

//            var result = await service.FindByIdAsync(1, CancellationToken.None);

//            Assert.True(result.IsSuccess);
//            Assert.NotNull(result.Value);
//            Assert.Equal("test", result.Value.UserName);
//        }

//        [Fact]
//        public async Task GetAllAsync_ReturnsNotFound_WhenNoUsers()
//        {
//            var service = CreateService();
//            _userRepositoryMock.Setup(r => r.GetAllAsync(
//                null,
//                It.IsAny<CancellationToken>(),
//                "Role,CreatedByUser,UpdatedByUser,DeletedByUser"
//            )).ReturnsAsync(new List<Users>());

//            var result = await service.GetAllAsync(CancellationToken.None);

//            Assert.False(result.IsSuccess);
//            Assert.Equal(ErrorType.NotFound, result.ErrorType);
//        }

//        [Fact]
//        public async Task GetAllAsync_Success()
//        {
//            var service = CreateService();
//            var users = new List<Users>
//            {
//                new User { Id = 1, UserName = "test1", Email = "test1@test.com", FirstName = "A", LastName = "B", Role = new UserRole { Name = "Admin" } },
//                new User { Id = 2, UserName = "test2", Email = "test2@test.com", FirstName = "C", LastName = "D", Role = new UserRole { Name = "Users" } }
//            };
//            _userRepositoryMock.Setup(r => r.GetAllAsync(
//                null,
//                It.IsAny<CancellationToken>(),
//                "Role,CreatedByUser,UpdatedByUser,DeletedByUser"
//            )).ReturnsAsync(users);

//            var result = await service.GetAllAsync(CancellationToken.None);

//            Assert.True(result.IsSuccess);
//            Assert.NotNull(result.Value);
//            Assert.Equal(2, result.Value.Count());
//        }

//        [Fact]
//        public async Task UpdateAsync_ReturnsInvalidId_WhenIdIsZero()
//        {
//            var service = CreateService();
//            var request = new UserUpdateRequest();
//            var result = await service.UpdateAsync(0, request, CancellationToken.None);
//            Assert.False(result.IsSuccess);
//            Assert.Equal(ErrorType.BadRequest, result.ErrorType);
//        }

//        [Fact]
//        public async Task UpdateAsync_ReturnsFailure_WhenValidationFails()
//        {
//            var service = CreateService();
//            var request = new UserUpdateRequest();
//            var validationResult = new FluentValidation.Results.ValidationResult(new[] {
//                new FluentValidation.Results.ValidationFailure("UserName", "Required")
//            });
//            _userUpdateValidatorMock.Setup(v => v.Validate(request)).Returns(validationResult);

//            var result = await service.UpdateAsync(1, request, CancellationToken.None);

//            Assert.False(result.IsSuccess);
//            Assert.Equal(ErrorType.BadRequest, result.ErrorType);
//        }

//        [Fact]
//        public async Task UpdateAsync_ReturnsNotFound_WhenUserDoesNotExist()
//        {
//            var service = CreateService();
//            var request = new UserUpdateRequest();
//            var validationResult = new FluentValidation.Results.ValidationResult();
//            _userUpdateValidatorMock.Setup(v => v.Validate(request)).Returns(validationResult);
//            _userRepositoryMock.Setup(r => r.FindAsync(
//                It.IsAny<Expression<Func<Users, bool>>>(),
//                It.IsAny<CancellationToken>(),
//                ""
//            )).ReturnsAsync((Users)null);

//            var result = await service.UpdateAsync(1, request, CancellationToken.None);

//            Assert.False(result.IsSuccess);
//            Assert.Equal(ErrorType.NotFound, result.ErrorType);
//        }

//        [Fact]
//        public async Task UpdateAsync_Success()
//        {
//            var service = CreateService();
//            var request = new UserUpdateRequest { UserName = "updated" };
//            var validationResult = new FluentValidation.Results.ValidationResult();
//            _userUpdateValidatorMock.Setup(v => v.Validate(request)).Returns(validationResult);
//            var user = new User { Id = 1, UserName = "old" };
//            _userRepositoryMock.Setup(r => r.FindAsync(
//                It.IsAny<Expression<Func<Users, bool>>>(),
//                It.IsAny<CancellationToken>(),
//                ""
//            )).ReturnsAsync(user);
//            _currentUserServiceMock.SetupGet(c => c.UserId).Returns(2);
//            _uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

//            var result = await service.UpdateAsync(1, request, CancellationToken.None);

//            Assert.True(result.IsSuccess);
//        }
//    }
//}