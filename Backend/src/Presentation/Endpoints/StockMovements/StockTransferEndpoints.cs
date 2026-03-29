using Carter;
using Presentation.Extensions;

namespace Presentation.Endpoints.StockMovements;

public sealed class StockTransferEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/stock-transfers")
            .WithTags("Stock Movements")
            .RequireAuthorization();

        group.MapGet("/", async (
                IInventoryQueries query,
                int page = 1,
                int pageSize = 10,
                string? search = null,
                string? sortColumn = null,
                string? sortOrder = null,
                CancellationToken cancellationToken = default) =>
            {
                var request = TableRequest.Create(pageSize, page, search, sortColumn, sortOrder);
                var response = await query.GetStockTransfersAsync(request, cancellationToken);

                if (response.IsSuccess)
                {
                    return Results.Ok(response.Value);
                }

                return response.Problem();
            })
            .WithSummary("List stock transfers")
            .WithDescription("Returns paged stock transfer records.")
            .Produces<PagedList<StockTransfersReadResponse>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapPost("/", async (
                StockTransferService service,
                StockTransferRequest request,
                CancellationToken cancellationToken = default) =>
            {
                var response = await service.TransferStockAsync(request, cancellationToken);

                if (response.IsSuccess)
                {
                    return Results.Ok(response.Value);
                }

                return response.Problem();
            })
            .WithSummary("Transfer stock")
            .WithDescription("Transfers stock between locations.")
            .Produces<int>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapGet("/{id:int}", async (
                ITransferQueries transferQuery,
                int id,
                CancellationToken cancellationToken = default) =>
            {
                var response = await transferQuery.GetByIdAsync(id, cancellationToken);

                if (response.IsSuccess)
                {
                    return Results.Ok(response.Value);
                }

                return response.Problem();
            })
            .WithSummary("Get stock transfer by id")
            .WithDescription("Returns stock transfer details by id.")
            .Produces<object>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }
}
