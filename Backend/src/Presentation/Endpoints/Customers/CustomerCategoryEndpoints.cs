using Carter;
using Presentation.Extensions;

namespace Presentation.Endpoints.Customers;

public sealed class CustomerCategoryEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/customer-categories")
            .WithTags("Customers")
            .RequireAuthorization();

        group.MapGet("/", async (
                CustomerCategoryService service,
                CancellationToken cancellationToken) =>
            {
                var response = await service.GetAllAsync(cancellationToken);

                if (response.IsSuccess)
                {
                    return Results.Ok(response.Value);
                }

                return response.Problem();
            })
            .WithSummary("List customer categories")
            .WithDescription("Returns all customer categories.")
            .Produces<IEnumerable<GetByIdResponse>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapGet("/{id:int}", async (
                CustomerCategoryService service,
                int id,
                CancellationToken cancellationToken) =>
            {
                var response = await service.GetByIdAsync(id, cancellationToken);

                if (response.IsSuccess)
                {
                    return Results.Ok(response.Value);
                }

                return response.Problem();
            })
            .WithName("GetCustomerCategoryByIdAsync")
            .WithSummary("Get customer category by id")
            .WithDescription("Returns a customer category by id.")
            .Produces<GetByIdResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapPost("/", async (
                CustomerCategoryService service,
                CreateCustomerCategoryRequest request,
                CancellationToken cancellationToken) =>
            {
                var response = await service.CreateCustomerAsync(request, cancellationToken);

                if (response.IsSuccess)
                {
                    return Results.CreatedAtRoute("GetCustomerCategoryByIdAsync", new
                    {
                        id = response.Value.Id
                    }, response.Value);
                }

                return response.Problem();
            })
            .WithSummary("Create customer category")
            .WithDescription("Creates a customer category and returns the created resource.")
            .Produces<CreateCustomerCategoryResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapPut("/{id:int}", async (
                CustomerCategoryService service,
                int id,
                UpdateCustomerCategoryCommand request,
                CancellationToken cancellationToken) =>
            {
                var command = request with { Id = id };
                var response = await service.UpdateCustomerAsync(command, cancellationToken);

                if (response.IsSuccess)
                {
                    return Results.Ok();
                }

                return response.Problem();
            })
            .WithSummary("Update customer category")
            .WithDescription("Updates customer category details.")
            .Produces(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapDelete("/{id:int}", async (
                CustomerCategoryService service,
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
            .WithSummary("Delete customer category")
            .WithDescription("Soft-deletes a customer category.")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapGet("/names", async (
                CustomerCategoryService service,
                CancellationToken cancellationToken) =>
            {
                var response = await service.GetCategoriesNamesAsync(cancellationToken);

                if (response.IsSuccess)
                {
                    return Results.Ok(response.Value);
                }

                return response.Problem();
            })
            .WithSummary("Get customer category names")
            .WithDescription("Returns customer category names for lookups.")
            .Produces<IEnumerable<object>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }
}
