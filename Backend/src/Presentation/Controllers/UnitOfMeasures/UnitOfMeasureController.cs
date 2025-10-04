using Application.DTOs.UnitOfMeasure;
using Application.Services.UnitOfMeasures;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Extensions;

namespace Presentation.Controllers.UnitOfMeasures
{
    [ApiController]
    [Route("api/unit-of-measures")]
    public class UnitOfMeasureController : ControllerBase
    {
        private readonly UnitOfMeasureService _service;

        public UnitOfMeasureController(UnitOfMeasureService service)
        {
            _service = service;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        [Authorize]
        public async Task<ActionResult<IReadOnlyCollection<UnitOfMeasureReadResponse>>>
            GetAllUnitOfMeasuresAsync(CancellationToken cancellationToken)
        {
            var response = await _service.GetAllAsync(cancellationToken);
            return response.HandleResult();
        }

        [HttpGet("{id}", Name = "GetUnitOfMeasureByIdAsync")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        [Authorize]
        public async Task<ActionResult<UnitOfMeasureReadResponse>>
            GetUnitOfMeasureByIdAsync(int id, CancellationToken cancellationToken)
        {
            var response = await _service.FindAsync(id, cancellationToken);
            return response.HandleResult();
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        [Authorize]
        public async Task<ActionResult<UnitOfMeasureReadResponse>> CreateUnitOfMeasureAsync
            ([FromBody] UnitOfMeasureRequest request
            , CancellationToken cancellationToken)
        {
            var response = await _service.AddAsync(request, cancellationToken);
            return response.HandleResult(nameof(GetUnitOfMeasureByIdAsync), new
            {
                id = response.Value?.Id
            });
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        [Authorize]
        public async Task<ActionResult<UnitOfMeasureReadResponse>> UpdateUnitOfMeasureAsync
            (int id, [FromBody] UnitOfMeasureRequest request
            , CancellationToken cancellationToken)
        {
            var response = await _service.UpdateAsync(id, request, cancellationToken);
            return response.HandleResult();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        [Authorize]
        public async Task<IActionResult> DeleteUnitOfMeasureAsync(int id
            ,CancellationToken cancellationToken)
        {
            var response = await _service.SoftDeleteAsync(id, cancellationToken);
            return response.HandleResult();
        }

        [HttpGet("names")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        [Authorize]
        public async Task<ActionResult<IEnumerable<object>>>
            GetUnitOfMeasureNamesAsync(CancellationToken cancellationToken)
        {
            var response = await _service.GetUnitsNamesAsync(cancellationToken);
            return response.HandleResult();
        }






    }
}
