using Carter;
using Microsoft.AspNetCore.Authorization;
using Presentation.Extensions;

namespace Presentation.Endpoints.Auth;

public sealed record LoginTokenResponse(string Token);

public sealed class AuthenticationEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/auth")
            .WithTags("Auth");

        group.MapPost("/login", async (
                IAuthenticationService service,
                IHttpContextAccessor httpContextAccessor,
                LoginRequest loginRequest,
                CancellationToken cancellationToken) =>
            {
                var response = await service.LoginAsync(loginRequest, cancellationToken);
                if (response.IsSuccess)
                {
                    var refreshToken = response.Value!.RefreshToken;
                    httpContextAccessor.HttpContext!
                    .Response.Cookies
                    .Append("refreshToken", refreshToken, new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.None,
                        Path = "/api/auth",
                        Expires = DateTimeOffset.UtcNow.AddDays(7)
                    });

                    return Results.Ok(new LoginTokenResponse(response.Value.Token));
                }

                return response.Problem();
            })
            .AllowAnonymous()
            .WithSummary("User login")
            .WithDescription("Authenticates a user and returns access token")
            .Produces<LoginTokenResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapPost("/refresh-token", async (
                IAuthenticationService service,
                IHttpContextAccessor httpContextAccessor,
                CancellationToken cancellationToken) =>
            {
                var refreshToken = httpContextAccessor
                .HttpContext!
                .Request
                .Cookies["refreshToken"];
                if (refreshToken is null)
                {
                    return Results.Problem(
                        statusCode: StatusCodes.Status400BadRequest,
                        title: "Missing refresh token",
                        detail: "Refresh token is missing.");
                }
                var command = new RefreshTokenRequest(refreshToken);
                var response = await service
                .RefreshTokenAsync(command, cancellationToken);
                if (response.IsSuccess)
                {
                    httpContextAccessor.HttpContext.Response
                    .Cookies.Append("refreshToken", response.Value!.RefreshToken, new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.None,
                        Path = "/api/auth",
                        Expires = DateTimeOffset.UtcNow.AddDays(7)
                    });
                    return Results.Ok(new LoginTokenResponse(response.Value.Token));
                }

                return response.Problem();
            })
            .WithSummary("Refresh token")
            .WithDescription("Generates a new access token using a valid refresh token.")
            .Produces<LoginTokenResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapPost("/reset-password", async (
                IAuthenticationService service,
                ResetPasswordRequest request,
                CancellationToken cancellationToken) =>
            {
                var response = await service.ResetPasswordAsync(request, cancellationToken);

                if (response.IsSuccess)
                {
                    return Results.Ok(response.Value);
                }

                return response.Problem();
            })
            .WithSummary("Reset password")
            .WithDescription("Resets user password using the provided reset information.")
            .Produces<string>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapPost("/forget-password", async (
                IAuthenticationService service,
                ForgetPasswordRequest request,
                CancellationToken cancellationToken) =>
            {
                var response = await service.ForgetPasswordAsync(request, cancellationToken);

                if (response.IsSuccess)
                {
                    return Results.Ok(response.Value);
                }

                return response.Problem();
            })
            .WithSummary("Request password reset")
            .WithDescription("Starts the forgot password flow and sends reset instructions.")
            .Produces<string>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapPost("/confirm-email", async (
                IAuthenticationService service,
                ConfirmEmailRequest request,
                CancellationToken cancellationToken) =>
            {
                var response = await service.ConfirmEmailAsync(request, cancellationToken);

                if (response.IsSuccess)
                {
                    return Results.Ok(response.Value);
                }

                return response.Problem();
            })
            .WithSummary("Confirm email")
            .WithDescription("Confirms a user email address using a confirmation token.")
            .Produces<string>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapPost("/send-confirm-email", async (
                IAuthenticationService service,
                SendConfirmEmailRequest request,
                CancellationToken cancellationToken) =>
            {
                var response = await service.SendConfirmEmailAsync(request, cancellationToken);

                if (response.IsSuccess)
                {
                    return Results.Ok(response.Value);
                }

                return response.Problem();
            })
            .WithSummary("Send confirmation email")
            .WithDescription("Sends an email confirmation message to the user.")
            .Produces<string>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }
}
