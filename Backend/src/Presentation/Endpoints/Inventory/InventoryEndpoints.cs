using Carter;
using Presentation.Extensions;

namespace Presentation.Endpoints.Inventory;

public sealed class InventoryEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/inventory")
            .WithTags("Inventory")
            .RequireAuthorization();

        group.MapGet("/valuation", async (
                InventoryService service,
                CancellationToken cancellationToken) =>
            {
                var response = await service.GetInventoryValuationAsync(cancellationToken);

                if (response.IsSuccess)
                {
                    return Results.Ok(response.Value);
                }

                return response.Problem();
            })
            .WithSummary("Get inventory valuation")
            .WithDescription("Returns the current total inventory valuation.")
            .Produces<decimal>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapGet("/cost", async (
                InventoryService service,
                CancellationToken cancellationToken) =>
            {
                var response = await service.GetInventoryCostAsync(cancellationToken);

                if (response.IsSuccess)
                {
                    return Results.Ok(response.Value);
                }

                return response.Problem();
            })
            .WithSummary("Get inventory cost")
            .WithDescription("Returns the current total inventory cost.")
            .Produces<decimal>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapGet("/low-stock", async (
                InventoryService service,
                CancellationToken cancellationToken) =>
            {
                var response = await service.GetInventoryLowStockAsync(cancellationToken);

                if (response.IsSuccess)
                {
                    return Results.Ok(response.Value);
                }

                return response.Problem();
            })
            .WithSummary("Get low stock inventory")
            .WithDescription("Returns low-stock inventory records.")
            .Produces<IReadOnlyCollection<InventoryBaseReadResponse>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapGet("/", async (
                IInventoryQueries query,
                int? page,
                int? pageSize,
                string? search,
                string? sortOrder,
                string? sortColumn,
                CancellationToken cancellationToken) =>
            {
                var request = TableRequest.Create(pageSize, page, search, sortColumn, sortOrder);
                var response = await query.GetInventoryTableAsync(request, cancellationToken);

                if (response.IsSuccess)
                {
                    return Results.Ok(response.Value);
                }

                return response.Problem();
            })
            .WithSummary("List inventory")
            .WithDescription("Returns paged inventory table records.")
            .Produces<PagedList<InventoryTableResponse>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapGet("/summary", async (
                IInventoryQueries query,
                CancellationToken cancellationToken) =>
            {
                var response = await query.GetInventorySummaryAsync(cancellationToken);

                if (response.IsSuccess)
                {
                    return Results.Ok(response.Value);
                }

                return response.Problem();
            })
            .WithSummary("Get inventory summary")
            .WithDescription("Returns inventory summary metrics.")
            .Produces<object>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapGet("/{id:int}", async (
                IInventoryQueries query,
                int id,
                CancellationToken cancellationToken) =>
            {
                var response = await query.GetByIdAsync(id, cancellationToken);

                if (response.IsSuccess)
                {
                    return Results.Ok(response.Value);
                }

                return response.Problem();
            })
            .WithName("GetInventoryByIdAsync")
            .WithSummary("Get inventory by id")
            .WithDescription("Returns inventory details by id.")
            .Produces<object>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapPost("/", async (
                InventoryService service,
                InventoryCreateRequest request,
                CancellationToken cancellationToken) =>
            {
                var response = await service.CreateAsync(request, cancellationToken);

                if (response.IsSuccess)
                {
                    return Results.CreatedAtRoute("GetInventoryByIdAsync", new
                    {
                        id = response.Value.Id
                    }, response.Value);
                }

                return response.Problem();
            })
            .WithSummary("Create inventory")
            .WithDescription("Creates an inventory record and returns the created resource.")
            .Produces<InventoryBaseReadResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapPut("/{id:int}", async (
                InventoryService service,
                int id,
                InventoryUpdateRequest request,
                CancellationToken cancellationToken) =>
            {
                var response = await service.UpdateAsync(id, request, cancellationToken);

                if (response.IsSuccess)
                {
                    return Results.Ok(response.Value);
                }

                return response.Problem();
            })
            .WithSummary("Update inventory")
            .WithDescription("Updates an inventory record.")
            .Produces<InventoryBaseReadResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapDelete("/{id:int}", async (
                InventoryService service,
                int id,
                CancellationToken cancellationToken) =>
            {
                var response = await service.DeleteByIdAsync(id, cancellationToken);

                if (response.IsSuccess)
                {
                    return Results.NoContent();
                }

                return response.Problem();
            })
            .WithSummary("Delete inventory")
            .WithDescription("Deletes an inventory record.")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }
}
