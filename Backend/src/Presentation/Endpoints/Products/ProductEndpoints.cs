using Carter;
using Presentation.Extensions;

namespace Presentation.Endpoints.Products;

public sealed class ProductEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/products")
            .WithTags("Products")
            .RequireAuthorization();

        group.MapGet("/", async (
                IProductQueries query,
                int page = 1,
                int pageSize = 10,
                string? search = null,
                string? sortColumn = null,
                string? sortOrder = null,
                CancellationToken cancellationToken = default) =>
            {
                var request = new TableRequest
                {
                    Page = page,
                    PageSize = pageSize,
                    search = string.IsNullOrWhiteSpace(search) ? search : search.ToLower(),
                    SortOrder = sortOrder,
                    SortColumn = sortColumn
                };
                var response = await query.GetAllAsync(request, cancellationToken);

                if (response.IsSuccess)
                {
                    return Results.Ok(response.Value);
                }

                return response.Problem();
            })
            .WithSummary("List products")
            .WithDescription("Returns paged products table data.")
            .Produces<PagedList<ProductTableResponse>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapGet("/{id:int}", async (
                IProductQueries query,
                int id,
                CancellationToken cancellationToken = default) =>
            {
                var response = await query.GetByIdAsync(id, cancellationToken);

                if (response.IsSuccess)
                {
                    return Results.Ok(response.Value);
                }

                return response.Problem();
            })
            .WithName("GetProductByIdAsync")
            .WithSummary("Get product by id")
            .WithDescription("Returns a product by id.")
            .Produces<ProductReadResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapPut("/{id:int}", async (
                IProductService service,
                int id,
                ProductUpdateRequest request,
                CancellationToken cancellationToken = default) =>
            {
                var response = await service.UpdateAsync(id, request, cancellationToken);

                if (response.IsSuccess)
                {
                    return Results.Ok(response.Value);
                }

                return response.Problem();
            })
            .WithSummary("Update product")
            .WithDescription("Updates product details.")
            .Produces<ProductReadResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapPut("/{id:int}/activate", async (
                IProductService service,
                int id,
                CancellationToken cancellationToken = default) =>
            {
                var response = await service.ActivateAsync(id, cancellationToken);

                if (response.IsSuccess)
                {
                    return Results.NoContent();
                }

                return response.Problem();
            })
            .WithSummary("Activate product")
            .WithDescription("Activates a product.")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapPut("/{id:int}/deactivate", async (
                IProductService service,
                int id,
                CancellationToken cancellationToken = default) =>
            {
                var response = await service.DeactivateAsync(id, cancellationToken);

                if (response.IsSuccess)
                {
                    return Results.NoContent();
                }

                return response.Problem();
            })
            .WithSummary("Deactivate product")
            .WithDescription("Deactivates a product.")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapDelete("/{id:int}", async (
                IProductService service,
                int id,
                CancellationToken cancellationToken = default) =>
            {
                var response = await service.DeleteAsync(id, cancellationToken);

                if (response.IsSuccess)
                {
                    return Results.NoContent();
                }

                return response.Problem();
            })
            .WithSummary("Delete product")
            .WithDescription("Soft-deletes a product.")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapPost("/", async (
                IProductService service,
                ProductCreateRequest request,
                CancellationToken cancellationToken = default) =>
            {
                var response = await service.CreateAsync(request, cancellationToken);

                if (response.IsSuccess)
                {
                    return Results.CreatedAtRoute("GetProductByIdAsync", new
                    {
                        id = response.Value.Id
                    }, response.Value);
                }

                return response.Problem();
            })
            .WithSummary("Create product")
            .WithDescription("Creates a product and returns the created product resource.")
            .Produces<ProductReadResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapGet("/{id:int}/suppliers", async (
                IProductService service,
                int id,
                CancellationToken cancellationToken = default) =>
            {
                var response = await service.FindProductSuppliersAsync(id, cancellationToken);

                if (response.IsSuccess)
                {
                    return Results.Ok(response.Value);
                }

                return response.Problem();
            })
            .WithSummary("Get product suppliers")
            .WithDescription("Returns suppliers for a product.")
            .Produces<IReadOnlyCollection<ProductSuppliersReadResponse>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapGet("/low-stock", async (
                IProductService service,
                CancellationToken cancellationToken = default) =>
            {
                var response = await service.GetProductsWithLowStockAsync(cancellationToken);

                if (response.IsSuccess)
                {
                    return Results.Ok(response.Value);
                }

                return response.Problem();
            })
            .WithSummary("Get low stock products")
            .WithDescription("Returns products that are currently low in stock.")
            .Produces<IReadOnlyCollection<ProductsLowStockReadResponse>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapGet("/{id:int}/inventory", async (
                IProductService service,
                int id,
                CancellationToken cancellationToken = default) =>
            {
                var response = await service.FindProductInInventoryAsync(id, cancellationToken);

                if (response.IsSuccess)
                {
                    return Results.Ok(response.Value);
                }

                return response.Problem();
            })
            .WithSummary("Get product inventory")
            .WithDescription("Returns inventory placements of a product across locations.")
            .Produces<IReadOnlyCollection<InventoryBaseReadResponse>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapGet("/summary", async (
                IProductQueries query,
                CancellationToken cancellationToken = default) =>
            {
                var response = await query.GetProductDashboardSummaryAsync(cancellationToken);

                if (response.IsSuccess)
                {
                    return Results.Ok(response.Value);
                }

                return response.Problem();
            })
            .WithSummary("Get products summary")
            .WithDescription("Returns product dashboard summary data.")
            .Produces<object>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapGet("/stock-movements-history", async (
                IProductQueries query,
                int page = 1,
                int pageSize = 10,
                string? search = null,
                string? sortColumn = null,
                string? sortOrder = null,
                CancellationToken cancellationToken = default) =>
            {
                var request = new TableRequest
                {
                    Page = page,
                    PageSize = pageSize,
                    search = string.IsNullOrWhiteSpace(search) ? search : search.ToLower(),
                    SortOrder = sortOrder,
                    SortColumn = sortColumn
                };

                var response = await query.GetStockMovementsHistoryAsync(request, cancellationToken);

                if (response.IsSuccess)
                {
                    return Results.Ok(response.Value);
                }

                return response.Problem();
            })
            .WithSummary("Get stock movement history")
            .WithDescription("Returns paged stock movement history for products.")
            .Produces<PagedList<StockMovementsHistoryTableResponse>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }
}
