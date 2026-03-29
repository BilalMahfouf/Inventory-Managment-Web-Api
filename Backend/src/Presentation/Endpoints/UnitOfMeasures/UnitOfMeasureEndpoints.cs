using Carter;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Extensions;

namespace Presentation.Endpoints.UnitOfMeasures;

public sealed class UnitOfMeasureEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/unit-of-measures")
            .WithTags("Unit Of Measures")
            .RequireAuthorization();

        group.MapGet("/", async (
                UnitOfMeasureService service,
                CancellationToken cancellationToken) =>
            {
                var response = await service.GetAllAsync(cancellationToken);

                if (response.IsSuccess)
                {
                    return Results.Ok(response.Value);
                }

                return response.Problem();
            })
            .WithSummary("List unit of measures")
            .WithDescription("Returns all unit of measure records.")
            .Produces<IReadOnlyCollection<UnitOfMeasureReadResponse>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapGet("/{id}", async (
                UnitOfMeasureService service,
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
            .WithName("GetUnitOfMeasureByIdAsync")
            .WithSummary("Get unit by id")
            .WithDescription("Returns a unit of measure by id.")
            .Produces<UnitOfMeasureReadResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapPost("/", async (
                UnitOfMeasureService service,
                [FromBody] UnitOfMeasureRequest request,
                CancellationToken cancellationToken) =>
            {
                var response = await service.AddAsync(request, cancellationToken);

                if (response.IsSuccess)
                {
                    return Results.CreatedAtRoute("GetUnitOfMeasureByIdAsync", new
                    {
                        id = response.Value.Id
                    }, response.Value);
                }

                return response.Problem();
            })
            .WithSummary("Create unit")
            .WithDescription("Creates a unit of measure and returns the created resource.")
            .Produces<UnitOfMeasureReadResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapPut("/{id}", async (
                UnitOfMeasureService service,
                int id,
                [FromBody] UnitOfMeasureRequest request,
                CancellationToken cancellationToken) =>
            {
                var response = await service.UpdateAsync(id, request, cancellationToken);

                if (response.IsSuccess)
                {
                    return Results.Ok(response.Value);
                }

                return response.Problem();
            })
            .WithSummary("Update unit")
            .WithDescription("Updates a unit of measure.")
            .Produces<UnitOfMeasureReadResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapDelete("/{id}", async (
                UnitOfMeasureService service,
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
            .WithSummary("Delete unit")
            .WithDescription("Soft-deletes a unit of measure.")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapGet("/names", async (
                UnitOfMeasureService service,
                CancellationToken cancellationToken) =>
            {
                var response = await service.GetUnitsNamesAsync(cancellationToken);

                if (response.IsSuccess)
                {
                    return Results.Ok(response.Value);
                }

                return response.Problem();
            })
            .WithSummary("Get unit names")
            .WithDescription("Returns unit names for lookups.")
            .Produces<IEnumerable<object>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }
}
