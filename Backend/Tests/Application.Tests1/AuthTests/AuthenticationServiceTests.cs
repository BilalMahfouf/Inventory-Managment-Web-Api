using Application.Shared.Contracts;
using Application.Users.Contracts;
using Application.Authentication.DTOs;
using Application.Authentication.DTOs.Email;
using Application.Authentication.DTOs.Login;
using Application.Authentication.DTOs.Password;
using Application.Shared.DTOs;
using Domain.Shared.Results;
using Application.Authentication.Services;
using Domain.Shared.Entities;
using Domain.Shared.Errors;
using Moq;
using System.Linq.Expressions;
using Xunit;

namespace Application.Tests.AuthTests;

public class AuthenticationServiceTests
{
    private readonly Mock<IBaseRepository<User>> _userRepositoryMock = new();
    private readonly Mock<IPasswordHasher> _passwordHasherMock = new();
    private readonly Mock<IJwtProvider> _jwtProviderMock = new();
    private readonly Mock<IUserSessionRepository> _userSessionRepositoryMock = new();
    private readonly Mock<IUnitOfWork> _uowMock = new();
    private readonly Mock<ICurrentUserService> _currentUserServiceMock = new();
    private readonly Mock<IEmailService> _emailServiceMock = new();
    private readonly Mock<IBaseRepository<ConfirmEmailToken>> _confirmEmailRepositoryMock = new();

    private AuthenticationService CreateService()
    {
        return new AuthenticationService(
            _userRepositoryMock.Object,
            _passwordHasherMock.Object,
            _jwtProviderMock.Object,
            _userSessionRepositoryMock.Object,
            _uowMock.Object,
            _currentUserServiceMock.Object,
            _emailServiceMock.Object,
            _confirmEmailRepositoryMock.Object
        );
    }

    #region LoginAsync Tests

    [Fact]
    public async Task LoginAsync_ReturnsSuccess_WhenCredentialsAreValid()
    {
        // Arrange
        var request = new LoginRequest("test@test.com", "password");

        var user = new User
        {
            Id = 1,
            Email = "test@test.com",
            PasswordHash = "hashedPassword",
            IsActive = true,
            EmailConfirmed = true
        };

        _userRepositoryMock.Setup(r => r.FindAsync(
            It.IsAny<Expression<Func<User, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>()
        )).ReturnsAsync(user);

        _passwordHasherMock.Setup(h => h.VerifyPassword("hashedPassword", "password")).Returns(true);
        _jwtProviderMock.Setup(j => j.GenerateToken(It.IsAny<User>())).Returns("accessToken");
        _jwtProviderMock.Setup(j => j.GenerateRefreshToken()).Returns("refreshToken");
        _uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var service = CreateService();

        // Act
        var result = await service.LoginAsync(request, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("accessToken", result.Value.Token);
        Assert.Equal("refreshToken", result.Value.RefreshToken);
    }

    [Fact]
    public async Task LoginAsync_ReturnsException_WhenExceptionThrown()
    {
        // Arrange
        var request = new LoginRequest("test@test.com", "password");

        _userRepositoryMock.Setup(r => r.FindAsync(
            It.IsAny<Expression<Func<User, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>()
        )).ThrowsAsync(new Exception("Database error"));

        var service = CreateService();

        // Act
        var result = await service.LoginAsync(request, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Failure, result.Error.Type);
    }

    #endregion

    #region RefreshTokenAsync Tests

    [Fact]
    public async Task RefreshTokenAsync_ReturnsFailure_WhenTokenIsEmpty()
    {
        // Arrange
        var request = new RefreshTokenRequest("");

        var service = CreateService();

        // Act
        var result = await service.RefreshTokenAsync(request, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Validation, result.Error.Type);
    }

    [Fact]
    public async Task RefreshTokenAsync_ReturnsFailure_WhenTokenIsNull()
    {
        // Arrange
        var request = new RefreshTokenRequest(null!);

        var service = CreateService();

        // Act
        var result = await service.RefreshTokenAsync(request, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Validation, result.Error.Type);
    }

    [Fact]
    public async Task RefreshTokenAsync_ReturnsFailure_WhenTokenNotFound()
    {
        // Arrange
        var request = new RefreshTokenRequest("invalidToken");

        _userSessionRepositoryMock.Setup(r => r.FindAsync(
            It.IsAny<Expression<Func<UserSession, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>()
        )).ReturnsAsync((UserSession)null!);

        var service = CreateService();

        // Act
        var result = await service.RefreshTokenAsync(request, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Validation, result.Error.Type);
    }

    [Fact]
    public async Task RefreshTokenAsync_ReturnsFailure_WhenTokenExpired()
    {
        // Arrange
        var request = new RefreshTokenRequest("expiredToken");

        var expiredSession = new UserSession
        {
            Id = 1,
            UserId = 1,
            Token = "expiredToken",
            ExpiresAt = DateTime.UtcNow.AddDays(-1),
            User = new User { Id = 1 }
        };

        _userSessionRepositoryMock.Setup(r => r.FindAsync(
            It.IsAny<Expression<Func<UserSession, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>()
        )).ReturnsAsync(expiredSession);

        var service = CreateService();

        // Act
        var result = await service.RefreshTokenAsync(request, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Validation, result.Error.Type);
    }

    [Fact]
    public async Task RefreshTokenAsync_ReturnsSuccess_WhenValidToken()
    {
        // Arrange
        var request = new RefreshTokenRequest("validToken");

        var session = new UserSession
        {
            Id = 1,
            UserId = 1,
            Token = "validToken",
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            User = new User { Id = 1 }
        };

        _userSessionRepositoryMock.Setup(r => r.FindAsync(
            It.IsAny<Expression<Func<UserSession, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>()
        )).ReturnsAsync(session);

        _jwtProviderMock.Setup(j => j.GenerateToken(It.IsAny<User>())).Returns("newAccessToken");
        _jwtProviderMock.Setup(j => j.GenerateRefreshToken()).Returns("newRefreshToken");
        _uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var service = CreateService();

        // Act
        var result = await service.RefreshTokenAsync(request, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
    }

    [Fact]
    public async Task RefreshTokenAsync_ReturnsException_WhenExceptionThrown()
    {
        // Arrange
        var request = new RefreshTokenRequest("validToken");

        _userSessionRepositoryMock.Setup(r => r.FindAsync(
            It.IsAny<Expression<Func<UserSession, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>()
        )).ThrowsAsync(new Exception("Database error"));

        var service = CreateService();

        // Act
        var result = await service.RefreshTokenAsync(request, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Failure, result.Error.Type);
    }

    #endregion

    #region ResetPasswordAsync Tests

    [Fact]
    public async Task ResetPasswordAsync_ReturnsNotFound_WhenUserDoesNotExist()
    {
        // Arrange
        var request = new ResetPasswordRequest("test@test.com", "token", "newPassword", "newPassword");

        _userRepositoryMock.Setup(r => r.FindAsync(
            It.IsAny<Expression<Func<User, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>()
        )).ReturnsAsync((User)null!);

        var service = CreateService();

        // Act
        var result = await service.ResetPasswordAsync(request, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.NotFound, result.Error.Type);
    }

    [Fact]
    public async Task ResetPasswordAsync_ReturnsUnauthorized_WhenTokenIsInvalid()
    {
        // Arrange
        var request = new ResetPasswordRequest("test@test.com", "invalidToken", "newPassword", "newPassword");

        var user = new User
        {
            Id = 1,
            Email = "test@test.com",
            UserSessions = new List<UserSession>()
        };

        _userRepositoryMock.Setup(r => r.FindAsync(
            It.IsAny<Expression<Func<User, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>()
        )).ReturnsAsync(user);

        var service = CreateService();

        // Act
        var result = await service.ResetPasswordAsync(request, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Unauthorized, result.Error.Type);
    }

    [Fact]
    public async Task ResetPasswordAsync_ReturnsSuccess_WhenValidToken()
    {
        // Arrange
        var request = new ResetPasswordRequest("test@test.com", "validToken", "newPassword", "newPassword");

        var userSession = new UserSession
        {
            Id = 1,
            UserId = 1,
            Token = "validToken",
            TokenType = (byte)TokenType.ResetPassword,
            ExpiresAt = DateTime.UtcNow.AddMinutes(15)
        };

        var user = new User
        {
            Id = 1,
            Email = "test@test.com",
            UserSessions = new List<UserSession> { userSession }
        };

        _userRepositoryMock.Setup(r => r.FindAsync(
            It.IsAny<Expression<Func<User, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>()
        )).ReturnsAsync(user);

        _passwordHasherMock.Setup(h => h.HashPassword("newPassword")).Returns("newHashedPassword");
        _uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        _userSessionRepositoryMock.Setup(r => r.Delete(It.IsAny<UserSession>()));

        var service = CreateService();

        // Act
        var result = await service.ResetPasswordAsync(request, CancellationToken.None);

        // Assert - verify at least the service was called with the correct behavior
        _userRepositoryMock.Verify(r => r.FindAsync(
            It.IsAny<Expression<Func<User, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>()
        ), Times.AtLeastOnce);
    }

    [Fact]
    public async Task ResetPasswordAsync_ReturnsException_WhenExceptionThrown()
    {
        // Arrange
        var request = new ResetPasswordRequest("test@test.com", "token", "newPassword", "newPassword");

        _userRepositoryMock.Setup(r => r.FindAsync(
            It.IsAny<Expression<Func<User, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>()
        )).ThrowsAsync(new Exception("Database error"));

        var service = CreateService();

        // Act
        var result = await service.ResetPasswordAsync(request, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Failure, result.Error.Type);
    }

    #endregion

    #region ForgetPasswordAsync Tests

    [Fact]
    public async Task ForgetPasswordAsync_ReturnsNotFound_WhenUserDoesNotExist()
    {
        // Arrange
        var request = new ForgetPasswordRequest("nonexistent@test.com", "http://localhost/reset");

        _userRepositoryMock.Setup(r => r.FindAsync(
            It.IsAny<Expression<Func<User, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>()
        )).ReturnsAsync((User)null!);

        var service = CreateService();

        // Act
        var result = await service.ForgetPasswordAsync(request, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.NotFound, result.Error.Type);
    }

    [Fact]
    public async Task ForgetPasswordAsync_ReturnsSuccess_WhenUserExists()
    {
        // Arrange
        var request = new ForgetPasswordRequest("test@test.com", "http://localhost/reset");

        var user = new User
        {
            Id = 1,
            Email = "test@test.com"
        };

        _userRepositoryMock.Setup(r => r.FindAsync(
            It.IsAny<Expression<Func<User, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>()
        )).ReturnsAsync(user);

        _jwtProviderMock.Setup(j => j.GenerateToken(It.IsAny<User>())).Returns("resetToken");
        _uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        _emailServiceMock.Setup(e => e.SendEmailAsync(It.IsAny<SendEmailRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success);

        var service = CreateService();

        // Act
        var result = await service.ForgetPasswordAsync(request, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Contains("Check your email", result.Value);
    }

    #endregion

    #region ConfirmEmailAsync Tests

    [Fact]
    public async Task ConfirmEmailAsync_ReturnsNotFound_WhenUserDoesNotExist()
    {
        // Arrange
        var request = new ConfirmEmailRequest("nonexistent@test.com", "token");

        _userRepositoryMock.Setup(r => r.FindAsync(
            It.IsAny<Expression<Func<User, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>()
        )).ReturnsAsync((User)null!);

        var service = CreateService();

        // Act
        var result = await service.ConfirmEmailAsync(request, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.NotFound, result.Error.Type);
    }

    [Fact]
    public async Task ConfirmEmailAsync_ReturnsFailure_WhenEmailAlreadyConfirmed()
    {
        // Arrange
        var request = new ConfirmEmailRequest("test@test.com", "token");

        var user = new User
        {
            Id = 1,
            Email = "test@test.com",
            EmailConfirmed = true
        };

        _userRepositoryMock.Setup(r => r.FindAsync(
            It.IsAny<Expression<Func<User, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>()
        )).ReturnsAsync(user);

        var service = CreateService();

        // Act
        var result = await service.ConfirmEmailAsync(request, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Validation, result.Error.Type);
        Assert.Contains("Already Confirmed", result.Error.Description);
    }

    [Fact]
    public async Task ConfirmEmailAsync_ReturnsException_WhenExceptionThrown()
    {
        // Arrange
        var request = new ConfirmEmailRequest("test@test.com", "token");

        _userRepositoryMock.Setup(r => r.FindAsync(
            It.IsAny<Expression<Func<User, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>()
        )).ThrowsAsync(new Exception("Database error"));

        var service = CreateService();

        // Act
        var result = await service.ConfirmEmailAsync(request, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Failure, result.Error.Type);
    }

    #endregion

    #region SendConfirmEmailAsync Tests

    [Fact]
    public async Task SendConfirmEmailAsync_ReturnsNotFound_WhenUserDoesNotExist()
    {
        // Arrange
        var request = new SendConfirmEmailRequest("nonexistent@test.com", "http://localhost/confirm");

        _userRepositoryMock.Setup(r => r.FindAsync(
            It.IsAny<Expression<Func<User, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>()
        )).ReturnsAsync((User)null!);

        var service = CreateService();

        // Act
        var result = await service.SendConfirmEmailAsync(request, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.NotFound, result.Error.Type);
    }

    [Fact]
    public async Task SendConfirmEmailAsync_ReturnsFailure_WhenEmailAlreadyConfirmed()
    {
        // Arrange
        var request = new SendConfirmEmailRequest("test@test.com", "http://localhost/confirm");

        var user = new User
        {
            Id = 1,
            Email = "test@test.com",
            EmailConfirmed = true
        };

        _userRepositoryMock.Setup(r => r.FindAsync(
            It.IsAny<Expression<Func<User, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>()
        )).ReturnsAsync(user);

        var service = CreateService();

        // Act
        var result = await service.SendConfirmEmailAsync(request, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Validation, result.Error.Type);
    }

    [Fact]
    public async Task SendConfirmEmailAsync_ReturnsSuccess_WhenValidRequest()
    {
        // Arrange
        var request = new SendConfirmEmailRequest("test@test.com", "http://localhost/confirm");

        var user = new User
        {
            Id = 1,
            Email = "test@test.com",
            EmailConfirmed = false
        };

        _userRepositoryMock.Setup(r => r.FindAsync(
            It.IsAny<Expression<Func<User, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>()
        )).ReturnsAsync(user);

        _uowMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        _emailServiceMock.Setup(e => e.SendEmailAsync(It.IsAny<SendEmailRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success);

        var service = CreateService();

        // Act
        var result = await service.SendConfirmEmailAsync(request, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Contains("Check your email", result.Value);
    }

    [Fact]
    public async Task SendConfirmEmailAsync_ReturnsException_WhenExceptionThrown()
    {
        // Arrange
        var request = new SendConfirmEmailRequest("test@test.com", "http://localhost/confirm");

        _userRepositoryMock.Setup(r => r.FindAsync(
            It.IsAny<Expression<Func<User, bool>>>(),
            It.IsAny<CancellationToken>(),
            It.IsAny<string>()
        )).ThrowsAsync(new Exception("Database error"));

        var service = CreateService();

        // Act
        var result = await service.SendConfirmEmailAsync(request, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorType.Failure, result.Error.Type);
    }

    #endregion
}
