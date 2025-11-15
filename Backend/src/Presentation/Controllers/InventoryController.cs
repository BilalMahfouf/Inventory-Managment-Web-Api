using Application.Abstractions.Queries;
using Application.DTOs.Inventories;
using Application.DTOs.Inventories.Request;
using Application.Inventories;
using Application.PagedLists;
using Application.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Extensions;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/inventory")]

    [Authorize]
    public class InventoryController : ControllerBase
    {
        private readonly InventoryService _service;
        private readonly IInventoryQueries _query;

        public InventoryController(InventoryService service, IInventoryQueries query)
        {
            _service = service;
            _query = query;
        }

        [HttpGet("valuation")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<ActionResult<decimal>> GetInventoryValuationAsync(CancellationToken cancellationToken)
        {
            var response = await _service.GetInventoryValuationAsync(cancellationToken);
            return response.HandleResult();
        }
        [HttpGet("cost")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<ActionResult<decimal>> GetInventoryCostAsync(CancellationToken cancellationToken)
        {
            var response = await _service.GetInventoryCostAsync(cancellationToken);
            return response.HandleResult();
        }

        [HttpGet("low-stock")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<ActionResult<IReadOnlyCollection<InventoryBaseReadResponse>>>
            GetInventoryLowStock(CancellationToken cancellationToken)
        {
            var response = await _service.GetInventoryLowStockAsync(cancellationToken);
            return response.HandleResult();
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<ActionResult<PagedList<InventoryTableResponse>>>
            GetAllInventoriesAsync(
            [FromQuery] int? page,
            [FromQuery] int? pageSize,
            [FromQuery] string? search,
            [FromQuery] string? sortOrder,
            [FromQuery] string? sortColumn,
            CancellationToken cancellationToken)

        {
            var request = TableRequest.Create(
                pageSize,
                page,
                search,
                sortColumn,
                sortOrder);

            var response = await _query.GetInventoryTableAsync(request, cancellationToken);
            return response.HandleResult();
        }

        [HttpGet("summary")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<ActionResult<object>> GetInventorySummaryAsync(CancellationToken cancellationToken)
        {
            var response = await _query.GetInventorySummaryAsync(cancellationToken);
            return response.HandleResult();
        }



        [HttpGet("{id:int}", Name = "GetInventoryByIdAsync")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<ActionResult<object>> GetInventoryByIdAsync
            (int id, CancellationToken cancellationToken)
        {
            var response = await _query.GetByIdAsync(id, cancellationToken);
            return response.HandleResult();
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<ActionResult<InventoryBaseReadResponse>> CreateInventoryAsync
            (InventoryCreateRequest request, CancellationToken cancellationToken)
        {
            var response = await _service.CreateAsync(request, cancellationToken);
            return response.HandleResult(nameof(GetInventoryByIdAsync), new
            {
                id = response.Value?.Id
            });
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<ActionResult<InventoryBaseReadResponse>> UpdateInventoryAsync
            (int id, InventoryUpdateRequest request, CancellationToken cancellationToken)
        {
            var response = await _service.UpdateAsync(id, request, cancellationToken);
            return response.HandleResult();
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> DeleteInventoryAsync
            (int id, CancellationToken cancellationToken)
        {
            var response = await _service.DeleteByIdAsync(id, cancellationToken);
            return response.HandleResult();
        }
     }
}
