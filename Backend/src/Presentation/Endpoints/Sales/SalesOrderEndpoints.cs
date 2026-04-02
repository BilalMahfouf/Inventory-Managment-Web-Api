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
                    return Results.CreatedAtRoute("GetOrderByIdAsync", new { id = response.Value }, response.Value);
                }

                return response.Problem();
            })
            .WithSummary("Create sales order")
            .WithDescription("Creates a sales order and returns the created order id.")
            .Produces<int>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapPut("/{id:int}", async (
                SalesOrderService salesOrderService,
                int id,
                UpdateSalesOrderRequest request,
                CancellationToken cancellationToken = default) =>
            {
                var response = await salesOrderService.UpdateSalesOrderAsync(id, request, cancellationToken);

                if (response.IsSuccess)
                {
                    return Results.NoContent();
                }

                return response.Problem();
            })
            .WithSummary("Update sales order")
            .WithDescription("Updates a pending sales order.")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapPost("/{id:int}/confirm", async (
                SalesOrderService salesOrderService,
                int id,
                CancellationToken cancellationToken = default) =>
            {
                var response = await salesOrderService.ConfirmOrderAsync(id, cancellationToken);

                if (response.IsSuccess)
                {
                    return Results.NoContent();
                }

                return response.Problem();
            })
            .WithSummary("Confirm order")
            .WithDescription("Transitions order from pending to confirmed.")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapPost("/{id:int}/transit", async (
                SalesOrderService salesOrderService,
                int id,
                CancellationToken cancellationToken = default) =>
            {
                var response = await salesOrderService.MarkInTransitAsync(id, cancellationToken);

                if (response.IsSuccess)
                {
                    return Results.NoContent();
                }

                return response.Problem();
            })
            .WithSummary("Mark order in transit")
            .WithDescription("Transitions order from confirmed to in transit.")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapPost("/{id:int}/ship", async (
                SalesOrderService salesOrderService,
                int id,
                ShipOrderRequest request,
                CancellationToken cancellationToken = default) =>
            {
                var response = await salesOrderService.ShipOrderAsync(id, request.TrackingNumber, cancellationToken);

                if (response.IsSuccess)
                {
                    return Results.NoContent();
                }

                return response.Problem();
            })
            .WithSummary("Ship order")
            .WithDescription("Transitions order from in transit to shipped.")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict)
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
            .WithSummary("Complete order")
            .WithDescription("Transitions order from shipped to completed.")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapPost("/{id:int}/cancel", async (
                SalesOrderService salesOrderService,
                int id,
                CancellationToken cancellationToken = default) =>
            {
                var response = await salesOrderService.CancelOrderAsync(id, cancellationToken);

                if (response.IsSuccess)
                {
                    return Results.NoContent();
                }

                return response.Problem();
            })
            .WithSummary("Cancel order")
            .WithDescription("Cancels an order and restores stock.")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapPost("/{id:int}/return", async (
                SalesOrderService salesOrderService,
                int id,
                CancellationToken cancellationToken = default) =>
            {
                var response = await salesOrderService.ReturnOrderAsync(id, cancellationToken);

                if (response.IsSuccess)
                {
                    return Results.NoContent();
                }

                return response.Problem();
            })
            .WithSummary("Return order")
            .WithDescription("Transitions order from completed to returned.")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict)
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
            .WithSummary("Get order by id")
            .WithDescription("Returns order details by id.")
            .Produces<SalesOrderReadResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapGet("/", async (
                ISalesOrderQueries query,
                SalesOrderStatus? status,
                int? customerId,
                DateTime? dateFrom,
                DateTime? dateTo,
                int? pageNumber,
                int? pageSize,
                string? sortColumn,
                string? sortOrder,
                CancellationToken cancellationToken = default) =>
            {
                var request = TableRequest.Create(pageSize, pageNumber, null, sortColumn, sortOrder);

                var response = await query.GetSalesOrdersAsync(
                    request,
                    status,
                    customerId,
                    dateFrom,
                    dateTo,
                    cancellationToken);

                if (response.IsSuccess)
                {
                    return Results.Ok(response.Value);
                }

                return response.Problem();
            })
            .WithSummary("List orders")
            .WithDescription("Returns filtered and paged orders.")
            .Produces<PagedList<SalesOrderTableResponse>>(StatusCodes.Status200OK)
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
