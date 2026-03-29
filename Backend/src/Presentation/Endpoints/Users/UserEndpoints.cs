using Carter;
using Microsoft.AspNetCore.Authorization;
using Presentation.Extensions;

namespace Presentation.Endpoints.Users;

public sealed class UserEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/users")
            .WithTags("Users");

        group.MapGet("/", async (
                IUserService userService,
                CancellationToken cancellationToken) =>
            {
                var response = await userService.GetAllAsync(cancellationToken);

                if (response.IsSuccess)
                {
                    return Results.Ok(response.Value);
                }

                return response.Problem();
            })
            .WithSummary("List users")
            .WithDescription("Returns all users.")
            .Produces<IEnumerable<UserReadResponse>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapGet("/{id}", async (
                IUserService userService,
                int id,
                CancellationToken cancellationToken) =>
            {
                var response = await userService.FindByIdAsync(id, cancellationToken);

                if (response.IsSuccess)
                {
                    return Results.Ok(response.Value);
                }

                return response.Problem();
            })
            .RequireAuthorization()
            .WithName("GetUserByIdAsync")
            .WithSummary("Get user by id")
            .WithDescription("Returns a user by id.")
            .Produces<UserReadResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapPost("/", async (
                IUserService userService,
                UserCreateRequest request,
                CancellationToken cancellationToken) =>
            {
                var response = await userService.AddAsync(request, cancellationToken);

                if (response.IsSuccess)
                {
                    return Results.CreatedAtRoute("GetUserByIdAsync", new
                    {
                        id = response.Value.Id
                    }, response.Value);
                }

                return response.Problem();
            })
            .RequireAuthorization()
            .WithSummary("Create user")
            .WithDescription("Creates a user and returns the created user resource.")
            .Produces<UserReadResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapPut("/{id}", async (
                IUserService userService,
                int id,
                UserUpdateRequest request,
                CancellationToken cancellationToken) =>
            {
                var response = await userService.UpdateAsync(id, request, cancellationToken);

                if (response.IsSuccess)
                {
                    return Results.NoContent();
                }

                return response.Problem();
            })
            .RequireAuthorization()
            .WithSummary("Update user")
            .WithDescription("Updates user details.")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapPut("/{id}/activate", async (
                IUserService userService,
                int id,
                CancellationToken cancellationToken) =>
            {
                var response = await userService.ActivateAsync(id, cancellationToken);

                if (response.IsSuccess)
                {
                    return Results.NoContent();
                }

                return response.Problem();
            })
            .RequireAuthorization()
            .WithSummary("Activate user")
            .WithDescription("Activates a user account.")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapPut("/{id}/deactivate", async (
                IUserService userService,
                int id,
                CancellationToken cancellationToken) =>
            {
                var response = await userService.DesActivateAsync(id, cancellationToken);

                if (response.IsSuccess)
                {
                    return Results.NoContent();
                }

                return response.Problem();
            })
            .RequireAuthorization()
            .WithSummary("Deactivate user")
            .WithDescription("Deactivates a user account.")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapDelete("/{id}", async (
                IUserService userService,
                int id,
                CancellationToken cancellationToken) =>
            {
                var response = await userService.DeleteAsync(id, cancellationToken);

                if (response.IsSuccess)
                {
                    return Results.NoContent();
                }

                return response.Problem();
            })
            .RequireAuthorization()
            .WithSummary("Delete user")
            .WithDescription("Soft-deletes a user.")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapPut("/{id}/password", async (
                IUserService userService,
                int id,
                ChangePasswordRequest request,
                CancellationToken cancellationToken) =>
            {
                var response = await userService.ChangePasswordAsync(id, request, cancellationToken);

                if (response.IsSuccess)
                {
                    return Results.NoContent();
                }

                return response.Problem();
            })
            .RequireAuthorization()
            .WithSummary("Change password")
            .WithDescription("Changes the password for a user.")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }
}
