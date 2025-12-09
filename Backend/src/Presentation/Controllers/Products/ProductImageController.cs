using Application.Abstractions.Services.Product;
using Application.DTOs.Products.Request.ProductImages;
using Application.DTOs.Products.Response.ProductImages;
using Application.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Extensions;

namespace Presentation.Controllers.Products
{
    [ApiController]
    [Route("api/products/{productId}/images")]
    public class ProductImageController : ControllerBase
    {
        private readonly IProductImageService _service;

        public ProductImageController(IProductImageService service)
        {
            _service = service;
        }

        [HttpPost]
        [Consumes("multipart/form-data")] // 
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

       [Authorize]
        public async Task<ActionResult<ProductImageReadResponse>>
            UploadProductImageAsync(
            [FromRoute] int productId, [FromForm] bool isPrimary
        ,[FromForm] IFormFile file, CancellationToken cancellationToken)
        {
            if (file is null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
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
                var response = await _service.AddProductImageAsync(request
                    ,cancellationToken);

            return response.HandleResult();
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

       [Authorize]
        public async Task<ActionResult<IReadOnlyCollection<ProductImageReadResponse>>>
            GetProductImagesAsync([FromRoute] int productId
            , CancellationToken cancellationToken = default)
        {
            var response = await _service.GetProductImages(productId
                , cancellationToken);
            return response.HandleResult();
        }
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        [Authorize]
        public async Task<IActionResult>
            DeleteProductImageAsync([FromRoute] int id
            , CancellationToken cancellationToken = default)
        {
            var response = await _service.DeleteProductImageAsync(id
                , cancellationToken);
            return response.HandleResult();
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        [Authorize]
        public async Task<IActionResult> SetProductImagePrimary(
            [FromRoute] int id
            , CancellationToken cancellationToken = default)
        {
            var response = await _service.SetProductImagePrimaryAsync(id
                , cancellationToken);
            return response.HandleResult();
        }




    }
}
