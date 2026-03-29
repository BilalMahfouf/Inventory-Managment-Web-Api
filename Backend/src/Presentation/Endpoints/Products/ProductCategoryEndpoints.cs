using Carter;
using Presentation.Extensions;

namespace Presentation.Endpoints.Products;

public sealed class ProductCategoryEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/product-categories")
            .WithTags("Products")
            .RequireAuthorization();

        group.MapGet("/", async (
                IProductCategoryService service,
                CancellationToken cancellationToken) =>
            {
                var response = await service.GetAllAsync(cancellationToken);

                if (response.IsSuccess)
                {
                    return Results.Ok(response.Value);
                }

                return response.Problem();
            })
            .WithSummary("List product categories")
            .WithDescription("Returns all product categories.")
            .Produces<IReadOnlyCollection<ProductCategoryReadResponse>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapGet("/{id}/children", async (
                IProductCategoryService service,
                int id,
                CancellationToken cancellationToken) =>
            {
                var response = await service.GetAllChildrenAsync(id, cancellationToken);

                if (response.IsSuccess)
                {
                    return Results.Ok(response.Value);
                }

                return response.Problem();
            })
            .WithSummary("Get category children")
            .WithDescription("Returns direct children of a product category.")
            .Produces<IReadOnlyCollection<ProductCategoryChildrenReadResponse>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapGet("/tree", async (
                IProductCategoryService service,
                CancellationToken cancellationToken) =>
            {
                var response = await service.GetAllTreeAsync(cancellationToken);

                if (response.IsSuccess)
                {
                    return Results.Ok(response.Value);
                }

                return response.Problem();
            })
            .WithSummary("Get categories tree")
            .WithDescription("Returns product categories in tree shape.")
            .Produces<IReadOnlyCollection<ProductCategoryTreeReadResponse>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapGet("/{id}", async (
                IProductCategoryQueries query,
                int id,
                CancellationToken cancellationToken) =>
            {
                var response = await query.GetCategoryByIdAsync(id, cancellationToken);

                if (response.IsSuccess)
                {
                    return Results.Ok(response.Value);
                }

                return response.Problem();
            })
            .WithName("GetProductCategoryByIdAsync")
            .WithSummary("Get category by id")
            .WithDescription("Returns product category details by id.")
            .Produces<ProductCategoryDetailsResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapPut("/{id}", async (
                IProductCategoryService service,
                int id,
                ProductCategoryRequest request,
                CancellationToken cancellationToken) =>
            {
                var response = await service.UpdateAsync(id, request, cancellationToken);

                if (response.IsSuccess)
                {
                    return Results.Ok(response.Value);
                }

                return response.Problem();
            })
            .WithSummary("Update category")
            .WithDescription("Updates a product category.")
            .Produces<ProductCategoryDetailsResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapPost("/", async (
                IProductCategoryService service,
                ProductCategoryRequest request,
                CancellationToken cancellationToken) =>
            {
                var response = await service.AddAsync(request, cancellationToken);

                if (response.IsSuccess)
                {
                    return Results.CreatedAtRoute("GetProductCategoryByIdAsync", new
                    {
                        id = response.Value.Id
                    }, response.Value);
                }

                return response.Problem();
            })
            .WithSummary("Create category")
            .WithDescription("Creates a product category and returns the created resource.")
            .Produces<ProductCategoryReadResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapDelete("/{id}", async (
                IProductCategoryService service,
                int id,
                CancellationToken cancellationToken) =>
            {
                var response = await service.DeleteAsync(id, cancellationToken);

                if (response.IsSuccess)
                {
                    return Results.NoContent();
                }

                return response.Problem();
            })
            .WithSummary("Delete category")
            .WithDescription("Deletes a product category.")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapGet("/names", async (
                IProductCategoryService service,
                CancellationToken cancellationToken) =>
            {
                var response = await service.GetCategoriesNamesAsync(cancellationToken);

                if (response.IsSuccess)
                {
                    return Results.Ok(response.Value);
                }

                return response.Problem();
            })
            .WithSummary("Get category names")
            .WithDescription("Returns category names for lookups.")
            .Produces<IEnumerable<object>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapGet("/main-categories", async (
                IProductCategoryQueries query,
                CancellationToken cancellationToken) =>
            {
                var response = await query.GetMainCategoriesAsync(cancellationToken);

                if (response.IsSuccess)
                {
                    return Results.Ok(response.Value);
                }

                return response.Problem();
            })
            .WithSummary("Get main categories")
            .WithDescription("Returns root-level product categories.")
            .Produces<IEnumerable<object>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }
}
