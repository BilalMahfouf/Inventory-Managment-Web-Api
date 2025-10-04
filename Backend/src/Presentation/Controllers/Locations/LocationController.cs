using Application.DTOs.Inventories;
using Application.DTOs.Locations.Request;
using Application.DTOs.Locations.Response;
using Application.Services.Locations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Extensions;

namespace Presentation.Controllers.Locations
{
    [ApiController]
    [Route("api/locations")]
    public class LocationController : ControllerBase
    {
        private readonly LocationService _service;

        public LocationController(LocationService service)
        {
            _service = service;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        //[Authorize]
        public async Task<ActionResult<IReadOnlyCollection<LocationReadResponse>>>
            GetAllLocation(CancellationToken cancellationToken = default)
        {
            var response = await _service.GetAllAsync(cancellationToken);
            return response.HandleResult();
        }

        [HttpGet("{id:int}",Name ="GetLocationByIdAsync")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]


        //        [Authorize]
        public async Task<ActionResult<LocationReadResponse>> GetLocationByIdAsync(int id
            , CancellationToken cancellationToken = default)
        {
            var response = await _service.FindAsync(id, cancellationToken);
            return response.HandleResult();
        }
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        //[Authorize]
        public async Task<ActionResult<LocationReadResponse>> CreateLocationAsync(
            LocationCreateRequest request
            , CancellationToken cancellationToken = default)
        {
            var response = await _service.CreateAsync(request,cancellationToken);
            return response.HandleResult(nameof(GetLocationByIdAsync), new
            {
                id=response.Value?.Id
            });
        }
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        //[Authorize]
        
        public async Task<ActionResult<LocationReadResponse>> UpdateLocationAsync(int id
            , LocationUpdateRequest request
            , CancellationToken cancellationToken = default)
        {
            request.Id = id;
            var response = await _service.UpdateAsync(id, request, cancellationToken);
            return response.HandleResult();
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        //[Authorize]
        public async Task<IActionResult> DeleteLocationAsync(int id
            , CancellationToken cancellationToken = default)
        {
            var response = await _service.SoftDeleteAsync(id, cancellationToken);
            return response.HandleResult();
        }

        [HttpPut("{id:int}/activate")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        //[Authorize]
        public async Task<IActionResult> ActivateLocationAsync(int id
            , CancellationToken cancellationToken = default)
        {
            var response = await _service.ActivateAsync(id, cancellationToken);
            return response.HandleResult();
        }

        [HttpPut("{id:int}/deactivate")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        //[Authorize]
        public async Task<IActionResult> DeActivateLocationAsync(int id
           , CancellationToken cancellationToken = default)
        {
            var response = await _service.DeactivateAsync(id, cancellationToken);
            return response.HandleResult();
        }

        [HttpGet("{id:int}/inventories")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        [Authorize]
        public async Task<ActionResult<IReadOnlyCollection<InventoryBaseReadResponse>>>
            GetLocationInventoriesByIdAsync(int id
            , CancellationToken cancellationToken = default)
        {
            var response = await _service.GetLocationInventoriesAsync(id, cancellationToken);
            return response.HandleResult();
        }

        [HttpGet("names")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        [Authorize]
        public async Task<ActionResult<IEnumerable<object>>> GetLocationsNamesAsync
            (CancellationToken cancellationToken = default)
        {
            var response = await _service.GetLocationsNamesAsync(cancellationToken);
            return response.HandleResult();
        }

    }
}
