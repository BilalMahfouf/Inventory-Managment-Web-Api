using System.Net;
using Application.IntegrationTests.Common;
using Application.Shared.Contracts;
using Application.Users.Contracts;
using Application.Users.DTOs.Request;
using Domain.Shared.Errors;
using Domain.Users.Entities;
using FluentAssertions;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Application.IntegrationTests.Services;

public sealed class UserServiceTests : IClassFixture<IntegrationTestWebAppFactory>, IAsyncLifetime
{
    private const int InvalidId = 0;
    private const int MissingId = 999_999;
    private const string UserNamePrefix = "UserService-IT-";
    private const string EmailPrefix = "userservice-it-";
    private const string RolePrefix = "UserServiceRole-IT-";
    private const string IntegrationSeedPasswordHash = "integration-tests-password-hash";

    private readonly AsyncServiceScope _scope;

    public UserServiceTests(IntegrationTestWebAppFactory factory)
    {
        _scope = factory.Services.CreateAsyncScope();
        UserService = _scope.ServiceProvider.GetRequiredService<IUserService>();
        PasswordHasher = _scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
        AppDbContext = _scope.ServiceProvider.GetRequiredService<InventoryManagmentDBContext>();
        HttpClient = factory.CreateClient();
    }

    private IUserService UserService { get; }

    private IPasswordHasher PasswordHasher { get; }

    private InventoryManagmentDBContext AppDbContext { get; }

    private HttpClient HttpClient { get; }

    private static int TestUserId => IntegrationTestWebAppFactory.SeedUserId;

    public async Task InitializeAsync()
    {
        await CleanupUserFeatureDataAsync();
    }

    public async Task DisposeAsync()
    {
        await CleanupUserFeatureDataAsync();
        await _scope.DisposeAsync();
    }

    [Fact]
    public async Task AddAsync_ValidRequest_ReturnsCreatedUser()
    {
        // Arrange
        var request = await BuildValidCreateRequestAsync();

        // Act
        var result = await UserService.AddAsync(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.UserName.Should().Be(request.UserName);
        result.Value.Email.Should().Be(request.Email);
    }

    [Fact]
    public async Task AddAsync_ValidRequest_PersistsHashedPassword()
    {
        // Arrange
        const string plainPassword = "Pass#123";
        var request = await BuildValidCreateRequestAsync(password: plainPassword);

        // Act
        var result = await UserService.AddAsync(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        AppDbContext.ChangeTracker.Clear();
        var persistedUser = await AppDbContext.Users
            .IgnoreQueryFilters()
            .SingleAsync(e => e.Id == result.Value.Id);

        persistedUser.PasswordHash.Should().NotBe(plainPassword);
        PasswordHasher.VerifyPassword(persistedUser.PasswordHash, plainPassword).Should().BeTrue();
    }

    [Fact]
    public async Task AddAsync_InvalidRequest_ReturnsValidationFailure()
    {
        // Arrange
        var request = await BuildValidCreateRequestAsync();
        request = request with
        {
            UserName = string.Empty,
            Email = "not-an-email",
            Password = "123",
            FirstName = string.Empty,
            LastName = string.Empty,
            RoleId = InvalidId,
        };

        // Act
        var result = await UserService.AddAsync(request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorType.Validation);
    }

    [Fact]
    public async Task AddAsync_DuplicateEmail_ReturnsFailure()
    {
        // Arrange
        var duplicateEmail = $"{EmailPrefix}{Guid.NewGuid():N}@ims.local";
        var firstRequest = await BuildValidCreateRequestAsync(email: duplicateEmail);
        var secondRequest = await BuildValidCreateRequestAsync(email: duplicateEmail);

        // Act
        var first = await UserService.AddAsync(firstRequest, CancellationToken.None);
        var second = await UserService.AddAsync(secondRequest, CancellationToken.None);

        // Assert
        first.IsSuccess.Should().BeTrue();
        second.IsSuccess.Should().BeFalse();
    }

    [Fact]
    public async Task AddAsync_DuplicateUserName_ReturnsConflictFailure()
    {
        // Arrange
        var duplicateUserName = $"{UserNamePrefix}{Guid.NewGuid():N}";
        var firstRequest = await BuildValidCreateRequestAsync(userName: duplicateUserName);
        var secondRequest = await BuildValidCreateRequestAsync(userName: duplicateUserName);

        // Act
        var first = await UserService.AddAsync(firstRequest, CancellationToken.None);
        var second = await UserService.AddAsync(secondRequest, CancellationToken.None);

        // Assert
        first.IsSuccess.Should().BeTrue();
        second.IsSuccess.Should().BeFalse();
        second.Error.Type.Should().Be(ErrorType.Conflict);
    }

    [Fact]
    public async Task FindByIdAsync_InvalidId_ReturnsValidationFailure()
    {
        // Arrange
        // Act
        var result = await UserService.FindByIdAsync(InvalidId, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorType.Validation);
    }

    [Fact]
    public async Task FindByIdAsync_MissingUser_ReturnsNotFoundFailure()
    {
        // Arrange
        // Act
        var result = await UserService.FindByIdAsync(MissingId, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public async Task GetAllAsync_AfterAddingUser_ContainsCreatedUser()
    {
        // Arrange
        var request = await BuildValidCreateRequestAsync();
        var created = await UserService.AddAsync(request, CancellationToken.None);

        // Act
        var result = await UserService.GetAllAsync(CancellationToken.None);

        // Assert
        created.IsSuccess.Should().BeTrue();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Contain(e => e.Id == created.Value.Id);
    }

    [Fact]
    public async Task UpdateAsync_ValidRequest_UpdatesUserAndAssignedRole()
    {
        // Arrange
        var createRequest = await BuildValidCreateRequestAsync();
        var created = await UserService.AddAsync(createRequest, CancellationToken.None);
        var newRoleId = await CreateRoleAsync();

        var updateRequest = new UserUpdateRequest
        {
            UserName = $"{UserNamePrefix}updated-{Guid.NewGuid():N}",
            FirstName = "Updated",
            LastName = "User",
            RoleId = newRoleId,
        };

        // Act
        var result = await UserService.UpdateAsync(created.Value.Id, updateRequest, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        AppDbContext.ChangeTracker.Clear();
        var persistedUser = await AppDbContext.Users
            .IgnoreQueryFilters()
            .SingleAsync(e => e.Id == created.Value.Id);

        persistedUser.UserName.Should().Be(updateRequest.UserName);
        persistedUser.FirstName.Should().Be(updateRequest.FirstName);
        persistedUser.LastName.Should().Be(updateRequest.LastName);
        persistedUser.RoleId.Should().Be(newRoleId);
    }

    [Fact]
    public async Task UpdateAsync_InvalidRequest_ReturnsValidationFailure()
    {
        // Arrange
        var request = new UserUpdateRequest
        {
            UserName = string.Empty,
            FirstName = string.Empty,
            LastName = string.Empty,
            RoleId = InvalidId,
        };

        // Act
        var result = await UserService.UpdateAsync(TestUserId, request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorType.Validation);
    }

    [Fact]
    public async Task UpdateAsync_MissingUser_ReturnsNotFoundFailure()
    {
        // Arrange
        var request = new UserUpdateRequest
        {
            UserName = $"{UserNamePrefix}missing-{Guid.NewGuid():N}",
            FirstName = "Missing",
            LastName = "User",
            RoleId = await GetDefaultRoleIdAsync(),
        };

        // Act
        var result = await UserService.UpdateAsync(MissingId, request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public async Task DeleteAsync_ExistingUser_SetsSoftDeleteAuditFields()
    {
        // Arrange
        var createdUser = await CreateUserAsync();

        // Act
        var result = await UserService.DeleteAsync(createdUser.Id, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        AppDbContext.ChangeTracker.Clear();
        var deletedUser = await AppDbContext.Users
            .IgnoreQueryFilters()
            .SingleAsync(e => e.Id == createdUser.Id);

        deletedUser.IsDeleted.Should().BeTrue();
        deletedUser.DeletedAt.Should().NotBeNull();
        deletedUser.DeletedByUserId.Should().Be(TestUserId);
    }

    [Fact]
    public async Task DeleteAsync_SoftDeletedUser_IsNotReturnedByFind()
    {
        // Arrange
        var createdUser = await CreateUserAsync();
        var deleted = await UserService.DeleteAsync(createdUser.Id, CancellationToken.None);

        // Act
        var result = await UserService.FindByIdAsync(createdUser.Id, CancellationToken.None);

        // Assert
        deleted.IsSuccess.Should().BeTrue();
        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public async Task ActivateAsync_AfterDeactivation_ReactivatesUser()
    {
        // Arrange
        var createdUser = await CreateUserAsync();
        var deactivated = await UserService.DesActivateAsync(createdUser.Id, CancellationToken.None);

        // Act
        var activated = await UserService.ActivateAsync(createdUser.Id, CancellationToken.None);

        // Assert
        deactivated.IsSuccess.Should().BeTrue();
        activated.IsSuccess.Should().BeTrue();

        AppDbContext.ChangeTracker.Clear();
        var persistedUser = await AppDbContext.Users
            .IgnoreQueryFilters()
            .SingleAsync(e => e.Id == createdUser.Id);

        persistedUser.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task DesActivateAsync_MissingUser_ReturnsNotFoundFailure()
    {
        // Arrange
        // Act
        var result = await UserService.DesActivateAsync(MissingId, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public async Task ChangePasswordAsync_WrongOldPassword_ReturnsValidationFailure()
    {
        // Arrange
        const string currentPassword = "Current#123";
        await SetSeedUserPasswordHashAsync(currentPassword);

        var request = new ChangePasswordRequest(
            OldPassword: "Wrong#123",
            NewPassword: "New#1234",
            ConfirmNewPassword: "New#1234");

        // Act
        var result = await UserService.ChangePasswordAsync(TestUserId, request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorType.Validation);
    }

    [Fact]
    public async Task ChangePasswordAsync_ValidRequest_UpdatesPasswordHash()
    {
        // Arrange
        const string oldPassword = "OldPass#123";
        const string newPassword = "NewPass#123";
        await SetSeedUserPasswordHashAsync(oldPassword);

        var request = new ChangePasswordRequest(
            OldPassword: oldPassword,
            NewPassword: newPassword,
            ConfirmNewPassword: newPassword);

        // Act
        var result = await UserService.ChangePasswordAsync(TestUserId, request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();

        AppDbContext.ChangeTracker.Clear();
        var seedUser = await AppDbContext.Users
            .IgnoreQueryFilters()
            .SingleAsync(e => e.Id == TestUserId);

        PasswordHasher.VerifyPassword(seedUser.PasswordHash, newPassword).Should().BeTrue();
    }

    [Fact]
    public async Task ChangePasswordAsync_InvalidRequest_ReturnsValidationFailure()
    {
        // Arrange
        var request = new ChangePasswordRequest(
            OldPassword: string.Empty,
            NewPassword: "short",
            ConfirmNewPassword: "mismatch");

        // Act
        var result = await UserService.ChangePasswordAsync(TestUserId, request, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorType.Validation);
    }

    [Fact]
    public async Task GetByIdEndpoint_WithoutAuthorization_ReturnsUnauthorized()
    {
        // Arrange
        // Act
        var response = await HttpClient.GetAsync($"/api/users/{TestUserId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    private async Task<User> CreateUserAsync()
    {
        var request = await BuildValidCreateRequestAsync();
        var result = await UserService.AddAsync(request, CancellationToken.None);
        result.IsSuccess.Should().BeTrue();

        AppDbContext.ChangeTracker.Clear();
        return await AppDbContext.Users
            .IgnoreQueryFilters()
            .SingleAsync(e => e.Id == result.Value.Id);
    }

    private async Task<UserCreateRequest> BuildValidCreateRequestAsync(
        string? userName = null,
        string? email = null,
        string? password = null,
        int? roleId = null)
    {
        var token = Guid.NewGuid().ToString("N");

        return new UserCreateRequest
        {
            UserName = userName ?? $"{UserNamePrefix}{token}",
            Email = email ?? $"{EmailPrefix}{token}@ims.local",
            Password = password ?? "Pass#123",
            FirstName = "Integration",
            LastName = "User",
            RoleId = roleId ?? await GetDefaultRoleIdAsync(),
        };
    }

    private async Task<int> GetDefaultRoleIdAsync()
    {
        return await AppDbContext.UserRoles
            .Where(e => e.Name == IntegrationTestWebAppFactory.DefaultUserRoleName)
            .Select(e => e.Id)
            .SingleAsync();
    }

    private async Task<int> CreateRoleAsync()
    {
        var role = new UserRole
        {
            Name = $"{RolePrefix}{Guid.NewGuid():N}",
            Description = "User role for integration tests",
        };

        AppDbContext.UserRoles.Add(role);
        await AppDbContext.SaveChangesAsync();

        return role.Id;
    }

    private async Task SetSeedUserPasswordHashAsync(string plainPassword)
    {
        var seedUser = await AppDbContext.Users
            .IgnoreQueryFilters()
            .SingleAsync(e => e.Id == TestUserId);

        seedUser.PasswordHash = PasswordHasher.HashPassword(plainPassword);
        seedUser.UpdatedAt = DateTime.UtcNow;
        seedUser.UpdatedByUserId = TestUserId;

        await AppDbContext.SaveChangesAsync();
    }

    private async Task CleanupUserFeatureDataAsync()
    {
        AppDbContext.ChangeTracker.Clear();

        var userIds = await AppDbContext.Users
            .IgnoreQueryFilters()
            .Where(e => EF.Functions.Like(e.UserName, $"{UserNamePrefix}%")
                || EF.Functions.Like(e.Email, $"{EmailPrefix}%"))
            .Select(e => e.Id)
            .ToListAsync();

        if (userIds.Count > 0)
        {
            await AppDbContext.UserSessions
                .IgnoreQueryFilters()
                .Where(e => userIds.Contains(e.UserId))
                .ExecuteDeleteAsync();

            await AppDbContext.ConfirmEmailTokens
                .IgnoreQueryFilters()
                .Where(e => userIds.Contains(e.UserId))
                .ExecuteDeleteAsync();

            await AppDbContext.Users
                .IgnoreQueryFilters()
                .Where(e => userIds.Contains(e.Id))
                .ExecuteDeleteAsync();
        }

        await AppDbContext.UserRoles
            .IgnoreQueryFilters()
            .Where(e => EF.Functions.Like(e.Name, $"{RolePrefix}%"))
            .ExecuteDeleteAsync();

        var seedUser = await AppDbContext.Users
            .IgnoreQueryFilters()
            .SingleAsync(e => e.Id == TestUserId);

        if (!string.Equals(seedUser.PasswordHash, IntegrationSeedPasswordHash, StringComparison.Ordinal))
        {
            seedUser.PasswordHash = IntegrationSeedPasswordHash;
            seedUser.UpdatedAt = null;
            seedUser.UpdatedByUserId = null;
        }

        await AppDbContext.SaveChangesAsync();
    }
}