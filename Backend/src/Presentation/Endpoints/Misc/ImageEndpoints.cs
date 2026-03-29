using Carter;
using Presentation.Extensions;

namespace Presentation.Endpoints.Misc;

public sealed class ImageEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/images")
            .WithTags("Images")
            .RequireAuthorization();

        group.MapGet("/{id:int}", async (
                ImageService service,
                int id,
                CancellationToken cancellationToken = default) =>
            {
                var response = await service.GetImageAsync(id, cancellationToken);

                if (response.Value is not null && response.IsSuccess)
                {
                    using var memory = new MemoryStream();
                    await response.Value.ImageStream.CopyToAsync(memory, cancellationToken);

                    var payload = new
                    {
                        response.Value.FileName,
                        response.Value.MimeType,
                        ContentBase64 = Convert.ToBase64String(memory.ToArray())
                    };

                    return Results.Ok(payload);
                }

                return response.Problem();
            })
            .WithSummary("Download image")
            .WithDescription("Returns image metadata and content as base64 JSON.")
            .Produces<object>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }
}
