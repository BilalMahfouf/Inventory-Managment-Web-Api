using Application.DTOs.StockMovements.Request;
using Application.DTOs.StockMovements.Response;
using Application.Services.StockMovements;
using Microsoft.AspNetCore.Mvc;
using Presentation.Extensions;

namespace Presentation.Controllers.StockMovements
{
    [ApiController]
    [Route("api/stock-movement-types")]
    public class StockMovementTypeController : ControllerBase
    {
        private readonly StockMovementTypeService _service;

        public StockMovementTypeController(StockMovementTypeService service)
        {
            _service = service;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<ActionResult<IEnumerable<StockMovementTypeReadResponse>>> 
            GetAllAsync(CancellationToken cancellationToken = default)
        {
            var result = await _service.GetAllAsync(cancellationToken);
            return result.HandleResult();
        }

        [HttpGet("{id:int}",Name = "GetStockMovementTypeByIdAsync")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<ActionResult<StockMovementTypeReadResponse>>
            GetStockMovementTypeByIdAsync(int id
            ,CancellationToken cancellationToken = default)
        {
            var result = await _service.FindAsync(id,cancellationToken);
            return result.HandleResult();
        }


        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<ActionResult<StockMovementTypeReadResponse>>
            UpdateStockMovementTypeAsync(int id
            , [FromBody] StockMovementTypeRequest request
            , CancellationToken cancellationToken = default)
        {
            var result = await _service.UpdateAsync(id, request, cancellationToken);
            return result.HandleResult();
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<ActionResult<StockMovementTypeReadResponse>>
            CreateStockMovementTypeAsync(
              [FromBody] StockMovementTypeRequest request
            , CancellationToken cancellationToken = default)
        {
            var result = await _service.AddAsync(request, cancellationToken);
            return result.HandleResult();
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> DeleteStockMovementTypeAsync(int id
            , CancellationToken cancellationToken = default)
        {
            var result = await _service.SoftDeleteAsync(id, cancellationToken);
            return result.HandleResult();
        }


    }
}
