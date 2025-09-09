using Application.Abstractions.Services.Products;
using Application.DTOs.Inventories;
using Application.DTOs.Products.Request.Products;
using Application.DTOs.Products.Response.Products;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Extensions;

namespace Presentation.Controllers.Products
{
    [ApiController]
    [Route("api/products")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _service;

        public ProductController(IProductService service)
        {
            _service = service;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        [Authorize]
        public async Task<ActionResult<IReadOnlyCollection<ProductReadResponse>>>
            GetAllProductAsync(CancellationToken cancellationToken = default)
        {
            var response = await _service.GetAllAsync(cancellationToken);
            return response.HandleResult();
        }

        [HttpGet("{id:int}", Name = "GetProductByIdAsync")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        [Authorize]
        public async Task<ActionResult<ProductReadResponse>> GetProductByIdAsync
            (int id, CancellationToken cancellationToken = default)
        {
            var response = await _service.FindAsync(id, cancellationToken);
            return response.HandleResult();
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        [Authorize]
        public async Task<ActionResult<ProductReadResponse>> UpdateProductAsync
            (int id, [FromBody] ProductUpdateRequest request
            , CancellationToken cancellationToken = default)
        {
            var response = await _service.UpdateAsync(id, request, cancellationToken);
            return response.HandleResult();
        }


        [HttpPut("{id:int}/activate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        [Authorize]
        public async Task<IActionResult> ActivateProductAsync(int id
            , CancellationToken cancellationToken = default)
        {
            var response = await _service.ActivateAsync(id, cancellationToken);
            return response.HandleResult();
        }
        [HttpPut("{id:int}/deactivate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        [Authorize]
        public async Task<IActionResult> DeActivateProductAsync(int id
            , CancellationToken cancellationToken = default)
        {
            var response = await _service.DeactivateAsync(id, cancellationToken);
            return response.HandleResult();
        }



        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        [Authorize]
        public async Task<IActionResult> DeleteProductAsync(int id
            , CancellationToken cancellationToken = default)
        {
            var response = await _service.DeleteAsync(id, cancellationToken);
            return response.HandleResult();
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        [Authorize]
        public async Task<ActionResult<ProductReadResponse>> CreateProductAsync(
            [FromBody] ProductCreateRequest request
            , CancellationToken cancellationToken = default)
        {
            var response = await _service.CreateAsync(request, cancellationToken);
            return response.HandleResult(nameof(GetProductByIdAsync),new
            {
                id = response.Value?.Id
            });
        }


        [HttpGet("{id:int}/suppliers")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        [Authorize]
        public async Task<ActionResult<IReadOnlyCollection<ProductSuppliersReadResponse>>>
            GetProductSuppliersAsync(int id
            , CancellationToken cancellationToken = default)
        {
            var response = await _service.FindProductSuppliersAsync(id, cancellationToken);
            return response.HandleResult();
        }

        [HttpGet("low-stock")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        [Authorize]
        public async Task<ActionResult<IReadOnlyCollection<ProductsLowStockReadResponse>>>
           GetProductsWithLowStockAsync( CancellationToken cancellationToken = default)
        {
            var response = await _service.GetProductsWithLowStockAsync(cancellationToken);
            return response.HandleResult();
        }

        [HttpGet("{id:int}/inventory")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        [Authorize]
        public async Task<ActionResult<IReadOnlyCollection<InventoryBaseReadResponse>>>
            GetProductAcrossLocations(int id
            , CancellationToken cancellationToken = default)
        {
            var response = await _service.FindProductInInventoryAsync(id
                , cancellationToken);
            return response.HandleResult();
        }


    }
}
