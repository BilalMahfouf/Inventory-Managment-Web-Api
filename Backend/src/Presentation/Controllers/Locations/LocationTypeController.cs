using Application.DTOs.Locations.Request;
using Application.DTOs.Locations.Response;
using Application.Services.Locations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Extensions;

namespace Presentation.Controllers.Locations
{
    [ApiController]
    [Route("api/location-type")]
    public class LocationTypeController : ControllerBase
    {
        private readonly LocationTypeService _service;
        public LocationTypeController(LocationTypeService locationTypeService)
        {
            _service= locationTypeService;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        //[Authorize]
        public async Task<ActionResult<LocationTypeReadResponse>> AddLocationTypeAsync(
            [FromBody] LocationTypeCreateRequest request
            , CancellationToken cancellationToken = default)
        {
            var response = await _service.AddLocationTypeAsync(request);
            return response.HandleResult(nameof(GetLocationTypeByIdAsync), new
            {
                id = response.Value?.Id
            });
        }

        [HttpGet("{id:int}",Name = "GetLocationTypeByIdAsync")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        //[Authorize]
        public async Task<ActionResult<LocationTypeReadResponse>> GetLocationTypeByIdAsync(
            int id
            , CancellationToken cancellationToken = default)
        {
            var response = await _service.FindAsync(id, cancellationToken);
            return response.HandleResult();
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        //[Authorize]
        public async Task<ActionResult<IReadOnlyCollection<LocationTypeReadResponse>>>
            GetLocationTypesAsync(
            int id
            , CancellationToken cancellationToken = default)
        {
            var response = await _service.GetAllLocationTypesAsync(cancellationToken);
            return response.HandleResult();
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        //[Authorize]
        public async Task<IActionResult> DeleteLocationTypeAsync(
           int id
           , CancellationToken cancellationToken = default)
        {
            var response = await _service.SoftDeleteAsync(id, cancellationToken);
            return response.HandleResult();
        }





    }
}
