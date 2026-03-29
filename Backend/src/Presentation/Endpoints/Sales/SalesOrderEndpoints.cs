using Carter;
using Presentation.Extensions;

namespace Presentation.Endpoints.Sales;

public sealed class SalesOrderEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/sales-orders")
            .WithTags("Sales")
            .RequireAuthorization();

        group.MapPost("/", async (
                SalesOrderService salesOrderService,
                CreateSalesOrderRequest request,
                CancellationToken cancellationToken = default) =>
            {
                var response = await salesOrderService.CreateSalesOrderAsync(request, cancellationToken);

                if (response.IsSuccess)
                {
                    return Results.CreatedAtRoute("GetOrderByIdAsync", new
                    {
                        id = response.Value
                    }, response.Value);
                }

                return response.Problem();
            })
            .WithSummary("Place sales order")
            .WithDescription("Creates a sales order and returns the created resource location.")
            .Produces<int>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapGet("/", async (
                ISalesOrderQueries query,
                int? page,
                int? pageSize,
                string? search,
                string? sortColumn,
                string? sortOrder,
                CancellationToken cancellationToken = default) =>
            {
                var request = TableRequest.Create(pageSize, page, search, sortColumn, sortOrder);
                var response = await query.GetOrdersTableAsync(request, cancellationToken);

                if (response.IsSuccess)
                {
                    return Results.Ok(response.Value);
                }

                return response.Problem();
            })
            .WithSummary("List sales orders")
            .WithDescription("Returns paged sales orders.")
            .Produces<PagedList<SalesOrderTableResponse>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapGet("/{id:int}", async (
                ISalesOrderQueries query,
                int id,
                CancellationToken cancellationToken = default) =>
            {
                var response = await query.GetSalesOrderByIdAsync(id, cancellationToken);

                if (response.IsSuccess)
                {
                    return Results.Ok(response.Value);
                }

                return response.Problem();
            })
            .WithName("GetOrderByIdAsync")
            .WithSummary("Get sales order by id")
            .WithDescription("Returns a sales order by id.")
            .Produces<SalesOrderReadResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapPost("/{id:int}/complete", async (
                SalesOrderService salesOrderService,
                int id,
                CancellationToken cancellationToken = default) =>
            {
                var response = await salesOrderService.CompleteOrderAsync(id, cancellationToken);

                if (response.IsSuccess)
                {
                    return Results.NoContent();
                }

                return response.Problem();
            })
            .WithSummary("Complete sales order")
            .WithDescription("Transitions a sales order to completed state.")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapGet("/summary", async (
                ISalesOrderQueries query,
                CancellationToken cancellationToken = default) =>
            {
                var response = await query.GetDahsboardSummaryAsync(cancellationToken);

                if (response.IsSuccess)
                {
                    return Results.Ok(response.Value);
                }

                return response.Problem();
            })
            .WithSummary("Get sales summary")
            .WithDescription("Returns sales summary metrics.")
            .Produces<object>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }
}
