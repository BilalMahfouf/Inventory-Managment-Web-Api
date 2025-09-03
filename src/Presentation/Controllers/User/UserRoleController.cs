using Application.DTOs.Users.Request;
using Application.DTOs.Users.Response;
using Application.Services.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Extensions;

namespace Presentation.Controllers.User
{
    [ApiController]
    [Route("api/user-roles")]
    public class UserRoleController:ControllerBase
    {
        private readonly UserRoleService _service;

        public UserRoleController(UserRoleService service)
        {
            _service = service;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        [Authorize]
        public async Task<ActionResult<IReadOnlyCollection<UserRoleReadResponse>>>
            GetAllUserRolesAsync(CancellationToken cancellationToken)
        {
            var response= await _service.GetAllAsync(cancellationToken);
            return response.HandleResult();
        }

        [HttpGet("{id}",Name ="GetUserRoleById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        [Authorize]
        public async Task<ActionResult<UserRoleReadResponse>>
            GetUserRoleById(int id,CancellationToken cancellationToken)
        {
            var response = await _service.FindAsync(id, cancellationToken);
            return response.HandleResult();
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        [Authorize]
        public async Task<ActionResult<UserRoleReadResponse>>
            UpdateUserRoleAsync(int id, UserRoleRequest request
            , CancellationToken cancellationToken)
        {
            var response = await _service.UpdateAsync(id, request, cancellationToken);
            return response.HandleResult();
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        [Authorize]
        public async Task<ActionResult<UserRoleReadResponse>>
            DeleteUserRoleAsync(int id, CancellationToken cancellationToken)
        {
            var response = await _service.SoftDeleteAsync(id, cancellationToken);
            return response.HandleResult();
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        [Authorize]
        public async Task<ActionResult<UserRoleReadResponse>>
            CreateUserRoleAsync(UserRoleRequest request
            , CancellationToken cancellationToken)
        {
            var response = await _service.AddAsync(request, cancellationToken);
            return response.HandleResult(nameof(GetUserRoleById), new
            {
                id = response.Value?.Id
            });
        }
    }
}
