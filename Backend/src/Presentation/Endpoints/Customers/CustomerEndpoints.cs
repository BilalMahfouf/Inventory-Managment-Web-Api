using Carter;
using Presentation.Extensions;

namespace Presentation.Endpoints.Customers;

public sealed class CustomerEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/customers")
            .WithTags("Customers")
            .RequireAuthorization();

        group.MapGet("/summary", async (
                ICustomerQueries query,
                CancellationToken cancellationToken) =>
            {
                var response = await query.GetCustomerSummary(cancellationToken);

                if (response.IsSuccess)
                {
                    return Results.Ok(response.Value);
                }

                return response.Problem();
            })
            .WithSummary("Get customers summary")
            .WithDescription("Returns dashboard summary metrics for customers.")
            .Produces<object>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapGet("/", async (
                ICustomerQueries query,
                int? page,
                int? pageSize,
                string? search,
                string? sortColumn,
                string? sortOrder,
                CancellationToken cancellationToken) =>
            {
                var request = TableRequest.Create(pageSize, page, search, sortColumn, sortOrder);
                var response = await query.GetAllAsync(request, cancellationToken);

                if (response.IsSuccess)
                {
                    return Results.Ok(response.Value);
                }

                return response.Problem();
            })
            .WithSummary("List customers")
            .WithDescription("Returns paged customers table data.")
            .Produces<PagedList<CustomerTableReadResponse>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapGet("/{id:int}", async (
                ICustomerQueries query,
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
            .WithName("GetCustomerByIdAsync")
            .WithSummary("Get customer by id")
            .WithDescription("Returns a customer by id.")
            .Produces<CustomerReadResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapPost("/", async (
                CustomerService service,
                CustomerCreateRequest request,
                CancellationToken cancellationToken) =>
            {
                var response = await service.AddAsync(request, cancellationToken);

                if (response.IsSuccess)
                {
                    return Results.CreatedAtRoute("GetCustomerByIdAsync", new
                    {
                        id = response.Value.Id
                    }, response.Value);
                }

                return response.Problem();
            })
            .WithSummary("Create customer")
            .WithDescription("Creates a customer and returns the created customer resource.")
            .Produces<CustomerReadResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapPut("/{id:int}", async (
                CustomerService service,
                int id,
                UpdateCustomerRequest request,
                CancellationToken cancellationToken) =>
            {
                var response = await service.UpdateAsync(id, request, cancellationToken);

                if (response.IsSuccess)
                {
                    return Results.Ok(response.Value);
                }

                return response.Problem();
            })
            .WithSummary("Update customer")
            .WithDescription("Updates customer details.")
            .Produces<int>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapDelete("/{id:int}", async (
                CustomerService service,
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
            .WithSummary("Delete customer")
            .WithDescription("Soft-deletes a customer.")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }
}
