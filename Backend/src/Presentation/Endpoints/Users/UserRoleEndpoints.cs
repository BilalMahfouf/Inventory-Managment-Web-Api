using Carter;
using Microsoft.AspNetCore.Authorization;
using Presentation.Extensions;

namespace Presentation.Endpoints.Users;

public sealed class UserRoleEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/user-roles")
            .WithTags("Users");

        group.MapGet("/", async (
                UserRoleService service,
                CancellationToken cancellationToken) =>
            {
                var response = await service.GetAllAsync(cancellationToken);

                if (response.IsSuccess)
                {
                    return Results.Ok(response.Value);
                }

                return response.Problem();
            })
            .RequireAuthorization()
            .WithSummary("List user roles")
            .WithDescription("Returns all user roles.")
            .Produces<IReadOnlyCollection<UserRoleReadResponse>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapGet("/{id}", async (
                UserRoleService service,
                int id,
                CancellationToken cancellationToken) =>
            {
                var response = await service.FindAsync(id, cancellationToken);

                if (response.IsSuccess)
                {
                    return Results.Ok(response.Value);
                }

                return response.Problem();
            })
            .RequireAuthorization()
            .WithName("GetUserRoleById")
            .WithSummary("Get role by id")
            .WithDescription("Returns a single user role by id.")
            .Produces<UserRoleReadResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapPut("/{id}", async (
                UserRoleService service,
                int id,
                UserRoleRequest request,
                CancellationToken cancellationToken) =>
            {
                var response = await service.UpdateAsync(id, request, cancellationToken);

                if (response.IsSuccess)
                {
                    return Results.NoContent();
                }

                return response.Problem();
            })
            .RequireAuthorization()
            .WithSummary("Update role")
            .WithDescription("Updates a user role.")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapDelete("/{id}", async (
                UserRoleService service,
                int id,
                CancellationToken cancellationToken) =>
            {
                var response = await service.SoftDeleteAsync(id, cancellationToken);

                if (response.IsSuccess)
                {
                    return Results.NoContent();
                }

                return response.Problem();
            })
            .RequireAuthorization()
            .WithSummary("Delete role")
            .WithDescription("Soft-deletes a user role.")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapPost("/", async (
                UserRoleService service,
                UserRoleRequest request,
                CancellationToken cancellationToken) =>
            {
                var response = await service.AddAsync(request, cancellationToken);

                if (response.IsSuccess)
                {
                    return Results.CreatedAtRoute("GetUserRoleById", new
                    {
                        id = response.Value.Id
                    }, response.Value);
                }

                return response.Problem();
            })
            .RequireAuthorization()
            .WithSummary("Create role")
            .WithDescription("Creates a user role and returns the created role.")
            .Produces<UserRoleReadResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }
}
