using Application.Abstractions.Services.Products;
using Application.DTOs.Products.Request.Categories;
using Application.DTOs.Products.Response.Categories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Extensions;

namespace Presentation.Controllers.Products
{
    [ApiController]
    [Route("api/product-categories")]
    public class ProductCategoryController:ControllerBase
    {
        private readonly IProductCategoryService _service;

        public ProductCategoryController(IProductCategoryService service)
        {
            _service = service;
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet]

        [Authorize]
        public async Task<ActionResult<IReadOnlyCollection<ProductCategoryReadResponse>>>
            GetAllProductCategoriesAsync(CancellationToken cancellationToken)
        {
            var response = await _service.GetAllAsync(cancellationToken);
            return response.HandleResult();
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet("{id}/children")]

        [Authorize]
        public async Task<ActionResult<IReadOnlyCollection
            <ProductCategoryChildrenReadResponse>>>
            GetAllProductCategoriesChildrenAsync(int id
            , CancellationToken cancellationToken)
        {
            var response = await _service.GetAllChildrenAsync(id,cancellationToken);
            return response.HandleResult();
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet("tree")]

        [Authorize]
        public async Task<ActionResult<IReadOnlyCollection<
            ProductCategoryTreeReadResponse>>>
            GetAllProductCategoriesTreeAsync(CancellationToken cancellationToken)
        {
            var response = await _service.GetAllTreeAsync(cancellationToken);
            return response.HandleResult();
        }
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet("{id}", Name = "GetProductCategoryByIdAsync")]

        [Authorize]
        public async Task<ActionResult<ProductCategoryReadResponse>>
            GetProductCategoryByIdAsync(int id, CancellationToken cancellationToken)
        {
            var response = await _service.FindAsync(id, cancellationToken);
            return response.HandleResult();
        }

        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPut("{id}")]

        [Authorize]
        public async Task<IActionResult> UpdateProductCategoryAsync(int id,
            ProductCategoryRequest request, CancellationToken cancellationToken)
        {
            var response = await _service.UpdateAsync(id, request, cancellationToken);
            return response.HandleResult();
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost]

        [Authorize]
        public async Task<ActionResult<ProductCategoryReadResponse>>
            CreateProductCategoryAsync(ProductCategoryRequest request
            , CancellationToken cancellationToken)
        {
            var response = await _service.AddAsync(request, cancellationToken);
            return response.HandleResult(nameof(GetProductCategoryByIdAsync)
                , new { id = response.Value?.Id });
        }

        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpDelete("{id}")]

        [Authorize]
        public async Task<IActionResult> DeleteProductCategoryAsync(int id
            ,CancellationToken cancellationToken)
        {
            var response = await _service.DeleteAsync(id, cancellationToken);
            return response.HandleResult();
        }


    }
}
