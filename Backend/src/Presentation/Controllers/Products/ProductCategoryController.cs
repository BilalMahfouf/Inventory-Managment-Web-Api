using Application.Abstractions.Queries;
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
    [Authorize]
    public class ProductCategoryController:ControllerBase
    {
        private readonly IProductCategoryService _service;
        private readonly IProductCategoryQueries _query;
        public ProductCategoryController(IProductCategoryService service, IProductCategoryQueries query)
        {
            _service = service;
            _query = query;
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet]

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

        public async Task<ActionResult<ProductCategoryDetailsResponse>>
            GetProductCategoryByIdAsync(int id, CancellationToken cancellationToken)
        {
            var response = await _query.GetCategoryByIdAsync(id, cancellationToken);
            return response.HandleResult();
        }

        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPut("{id}")]

        public async Task<ActionResult<ProductCategoryDetailsResponse>> UpdateProductCategoryAsync(int id,
            ProductCategoryRequest request, CancellationToken cancellationToken)
        {
            var response = await _service.UpdateAsync(id, request, cancellationToken);
            return response.HandleResult();
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost]

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

        public async Task<IActionResult> DeleteProductCategoryAsync(int id
            ,CancellationToken cancellationToken)
        {
            var response = await _service.DeleteAsync(id, cancellationToken);
            return response.HandleResult();
        }


        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet("names")]

        public async Task<ActionResult<IEnumerable<object>>> 
            GetProductCategoriesNamesAsync(
            CancellationToken cancellationToken)
        {
            var response = await _service.GetCategoriesNamesAsync(cancellationToken);
            return response.HandleResult();
            
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet("main-categories")]

        public async Task<ActionResult<IEnumerable<object>>>
            GetMainCategoriesAsync(CancellationToken cancellationToken)
        {
            var response = await _query.GetMainCategoriesAsync(cancellationToken);
            return response.HandleResult();
        }

    }
}
