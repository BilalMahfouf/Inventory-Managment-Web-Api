using Domain.Shared.Errors;

namespace Domain.Users;

public static class UserErrors
{
    public static Error UserNotFound(string email) =>
        Error.NotFound("User.NotFound", $"User with email '{email}' was not found.");

    public static Error UserNotFound(int id) =>
        Error.NotFound("User.NotFound", $"User with id '{id}' was not found.");

    public static Error InvalidCredentials =>
        Error.Unauthorized("User.InvalidCredentials", "The provided credentials are invalid.");

    public static Error ExpiredRefreshToken =>
        Error.Conflict("User.ExpiredRefreshToken", "Refresh token is expired, please login again.");

    public static Error InvalidPassword =>
        Error.Validation("User.InvalidPassword", "The provided password is invalid.");

    public static Error InvalidPasswordLength =>
        Error.Validation("User.InvalidPasswordLength", "Password must be at least 6 characters long.");

    public static Error EmailAlreadyInUse(string email) =>
        Error.Conflict("User.EmailAlreadyInUse", $"Email '{email}' is already in use.");

    public static Error UsersNotFound =>
        Error.NotFound("User.UsersNotFound", "No users found in the system.");
}
