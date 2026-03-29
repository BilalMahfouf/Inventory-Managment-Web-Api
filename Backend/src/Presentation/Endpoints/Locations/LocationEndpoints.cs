using Carter;
using Presentation.Extensions;

namespace Presentation.Endpoints.Locations;

public sealed class LocationEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/locations")
            .WithTags("Locations")
            .RequireAuthorization();

        group.MapGet("/", async (
                LocationService service,
                CancellationToken cancellationToken = default) =>
            {
                var response = await service.GetAllAsync(cancellationToken);

                if (response.IsSuccess)
                {
                    return Results.Ok(response.Value);
                }

                return response.Problem();
            })
            .WithSummary("List locations")
            .WithDescription("Returns all locations.")
            .Produces<IReadOnlyCollection<LocationReadResponse>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapGet("/{id:int}", async (
                LocationService service,
                int id,
                CancellationToken cancellationToken = default) =>
            {
                var response = await service.FindAsync(id, cancellationToken);

                if (response.IsSuccess)
                {
                    return Results.Ok(response.Value);
                }

                return response.Problem();
            })
            .WithName("GetLocationByIdAsync")
            .WithSummary("Get location by id")
            .WithDescription("Returns a location by id.")
            .Produces<LocationReadResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapPost("/", async (
                LocationService service,
                LocationCreateRequest request,
                CancellationToken cancellationToken = default) =>
            {
                var response = await service.CreateAsync(request, cancellationToken);

                if (response.IsSuccess)
                {
                    return Results.CreatedAtRoute("GetLocationByIdAsync", new
                    {
                        id = response.Value.Id
                    }, response.Value);
                }

                return response.Problem();
            })
            .WithSummary("Create location")
            .WithDescription("Creates a location and returns the created location resource.")
            .Produces<LocationReadResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapPut("/{id:int}", async (
                LocationService service,
                int id,
                LocationUpdateRequest request,
                CancellationToken cancellationToken = default) =>
            {
                request.Id = id;
                var response = await service.UpdateAsync(id, request, cancellationToken);

                if (response.IsSuccess)
                {
                    return Results.Ok(response.Value);
                }

                return response.Problem();
            })
            .WithSummary("Update location")
            .WithDescription("Updates a location.")
            .Produces<LocationReadResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapDelete("/{id:int}", async (
                LocationService service,
                int id,
                CancellationToken cancellationToken = default) =>
            {
                var response = await service.SoftDeleteAsync(id, cancellationToken);

                if (response.IsSuccess)
                {
                    return Results.NoContent();
                }

                return response.Problem();
            })
            .WithSummary("Delete location")
            .WithDescription("Soft-deletes a location.")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapPut("/{id:int}/activate", async (
                LocationService service,
                int id,
                CancellationToken cancellationToken = default) =>
            {
                var response = await service.ActivateAsync(id, cancellationToken);

                if (response.IsSuccess)
                {
                    return Results.NoContent();
                }

                return response.Problem();
            })
            .WithSummary("Activate location")
            .WithDescription("Activates a location.")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapPut("/{id:int}/deactivate", async (
                LocationService service,
                int id,
                CancellationToken cancellationToken = default) =>
            {
                var response = await service.DeactivateAsync(id, cancellationToken);

                if (response.IsSuccess)
                {
                    return Results.NoContent();
                }

                return response.Problem();
            })
            .WithSummary("Deactivate location")
            .WithDescription("Deactivates a location.")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapGet("/{id:int}/inventories", async (
                LocationService service,
                int id,
                CancellationToken cancellationToken = default) =>
            {
                var response = await service.GetLocationInventoriesAsync(id, cancellationToken);

                if (response.IsSuccess)
                {
                    return Results.Ok(response.Value);
                }

                return response.Problem();
            })
            .WithSummary("Get location inventories")
            .WithDescription("Returns inventories in a location.")
            .Produces<IReadOnlyCollection<InventoryBaseReadResponse>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapGet("/names", async (
                LocationService service,
                CancellationToken cancellationToken = default) =>
            {
                var response = await service.GetLocationsNamesAsync(cancellationToken);

                if (response.IsSuccess)
                {
                    return Results.Ok(response.Value);
                }

                return response.Problem();
            })
            .WithSummary("Get location names")
            .WithDescription("Returns location names for lookups.")
            .Produces<IEnumerable<object>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }
}
