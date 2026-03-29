using Carter;
using Microsoft.AspNetCore.Mvc;
using Presentation.Extensions;

namespace Presentation.Endpoints.Products;

public sealed class ProductImageEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/products/{productId}/images")
            .WithTags("Products")
            .RequireAuthorization();

        group.MapPost("/", async (
                IProductImageService service,
                int productId,
                [FromForm] bool isPrimary,
                [FromForm] IFormFile file,
                CancellationToken cancellationToken) =>
            {
                if (file is null || file.Length == 0)
                {
                    return Results.BadRequest("No file uploaded.");
                }

                await using var stream = file.OpenReadStream();

                var request = new ProductImageUploadRequest
                {
                    ProductId = productId,
                    FileName = file.FileName,
                    FileSize = file.Length,
                    MimeType = file.ContentType,
                    IsPrimary = isPrimary,
                    FileStream = stream
                };

                var response = await service.AddProductImageAsync(request, cancellationToken);

                if (response.IsSuccess)
                {
                    return Results.Ok(response.Value);
                }

                return response.Problem();
            })
            .Accepts<IFormFile>("multipart/form-data")
            .WithSummary("Upload product image")
            .WithDescription("Uploads an image for a product and optionally sets it as primary.")
            .Produces<ProductImageReadResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapGet("/", async (
                IProductImageService service,
                int productId,
                CancellationToken cancellationToken = default) =>
            {
                var response = await service.GetProductImages(productId, cancellationToken);

                if (response.IsSuccess)
                {
                    return Results.Ok(response.Value);
                }

                return response.Problem();
            })
            .WithSummary("List product images")
            .WithDescription("Returns all images assigned to a product.")
            .Produces<IReadOnlyCollection<ProductImageReadResponse>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapDelete("/{id:int}", async (
                IProductImageService service,
                int id,
                CancellationToken cancellationToken = default) =>
            {
                var response = await service.DeleteProductImageAsync(id, cancellationToken);

                if (response.IsSuccess)
                {
                    return Results.NoContent();
                }

                return response.Problem();
            })
            .WithSummary("Delete product image")
            .WithDescription("Deletes a product image.")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapPut("/{id:int}", async (
                IProductImageService service,
                int id,
                CancellationToken cancellationToken = default) =>
            {
                var response = await service.SetProductImagePrimaryAsync(id, cancellationToken);

                if (response.IsSuccess)
                {
                    return Results.NoContent();
                }

                return response.Problem();
            })
            .WithSummary("Set primary image")
            .WithDescription("Sets the selected image as primary for the product.")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }
}
