using Carter;
using Presentation.Extensions;

namespace Presentation.Endpoints.Customers;

public sealed class CustomerCategoryEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/customer-categories")
            .WithTags("Customers");

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
