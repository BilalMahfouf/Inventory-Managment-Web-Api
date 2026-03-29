using Carter;
using Microsoft.AspNetCore.Mvc;
using Presentation.Extensions;

namespace Presentation.Endpoints.StockMovements;

public sealed class StockMovementTypeEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/stock-movement-types")
            .WithTags("Stock Movements");

        group.MapGet("/", async (
                StockMovementTypeService service,
                CancellationToken cancellationToken = default) =>
            {
                var result = await service.GetAllAsync(cancellationToken);

                if (result.IsSuccess)
                {
                    return Results.Ok(result.Value);
                }

                return result.Problem();
            })
            .WithSummary("List stock movement types")
            .WithDescription("Returns all stock movement types.")
            .Produces<IEnumerable<StockMovementTypeReadResponse>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapGet("/{id:int}", async (
                StockMovementTypeService service,
                int id,
                CancellationToken cancellationToken = default) =>
            {
                var result = await service.FindAsync(id, cancellationToken);

                if (result.IsSuccess)
                {
                    return Results.Ok(result.Value);
                }

                return result.Problem();
            })
            .WithName("GetStockMovementTypeByIdAsync")
            .WithSummary("Get movement type by id")
            .WithDescription("Returns stock movement type by id.")
            .Produces<StockMovementTypeReadResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapPut("/{id:int}", async (
                StockMovementTypeService service,
                int id,
                [FromBody] StockMovementTypeRequest request,
                CancellationToken cancellationToken = default) =>
            {
                var result = await service.UpdateAsync(id, request, cancellationToken);

                if (result.IsSuccess)
                {
                    return Results.Ok(result.Value);
                }

                return result.Problem();
            })
            .WithSummary("Update movement type")
            .WithDescription("Updates stock movement type.")
            .Produces<StockMovementTypeReadResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapPost("/", async (
                StockMovementTypeService service,
                [FromBody] StockMovementTypeRequest request,
                CancellationToken cancellationToken = default) =>
            {
                var result = await service.AddAsync(request, cancellationToken);

                if (result.IsSuccess)
                {
                    return Results.Ok(result.Value);
                }

                return result.Problem();
            })
            .WithSummary("Create movement type")
            .WithDescription("Creates a stock movement type.")
            .Produces<StockMovementTypeReadResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapDelete("/{id:int}", async (
                StockMovementTypeService service,
                int id,
                CancellationToken cancellationToken = default) =>
            {
                var result = await service.SoftDeleteAsync(id, cancellationToken);

                if (result.IsSuccess)
                {
                    return Results.NoContent();
                }

                return result.Problem();
            })
            .WithSummary("Delete movement type")
            .WithDescription("Soft-deletes a stock movement type.")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }
}
