using Carter;
using Microsoft.AspNetCore.Mvc;
using Presentation.Extensions;

namespace Presentation.Endpoints.Locations;

public sealed class LocationTypeEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/location-type")
            .WithTags("Locations");

        group.MapPost("/", async (
                LocationTypeService service,
                [FromBody] LocationTypeCreateRequest request,
                CancellationToken cancellationToken = default) =>
            {
                var response = await service.AddLocationTypeAsync(request);

                if (response.IsSuccess)
                {
                    return Results.CreatedAtRoute("GetLocationTypeByIdAsync", new
                    {
                        id = response.Value.Id
                    }, response.Value);
                }

                return response.Problem();
            })
            .WithSummary("Create location type")
            .WithDescription("Creates a location type and returns the created resource.")
            .Produces<LocationTypeReadResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapGet("/{id:int}", async (
                LocationTypeService service,
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
            .WithName("GetLocationTypeByIdAsync")
            .WithSummary("Get location type by id")
            .WithDescription("Returns a location type by id.")
            .Produces<LocationTypeReadResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapGet("/", async (
                LocationTypeService service,
                int id = 0,
                CancellationToken cancellationToken = default) =>
            {
                var response = await service.GetAllLocationTypesAsync(cancellationToken);

                if (response.IsSuccess)
                {
                    return Results.Ok(response.Value);
                }

                return response.Problem();
            })
            .WithSummary("List location types")
            .WithDescription("Returns all location types.")
            .Produces<IReadOnlyCollection<LocationTypeReadResponse>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapDelete("/{id:int}", async (
                LocationTypeService service,
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
            .WithSummary("Delete location type")
            .WithDescription("Soft-deletes a location type.")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }
}
