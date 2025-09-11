using Application.Abstractions.Services.User;
using Application.DTOs.Users.Request;
using Application.DTOs.Users.Response;
using Application.FluentValidations.User.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Extensions;

namespace Presentation.Controllers.User
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]


       
        public async Task<ActionResult<IEnumerable<UserReadResponse>>> GetAllUsersAsync(
            CancellationToken cancellationToken)
        {
            var response = await _userService.GetAllAsync(cancellationToken);
            return response.HandleResult();
        }

        [HttpGet("{id}",Name = "GetUserByIdAsync")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        [Authorize]
        public async Task<ActionResult<UserReadResponse>> GetUserByIdAsync(
            int id,CancellationToken cancellationToken)
        {
            var response = await _userService.FindByIdAsync(id, cancellationToken);
            return response.HandleResult();
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        [Authorize]
        public async Task<ActionResult<UserReadResponse>> CreateUserAsync(
            UserCreateRequest request,CancellationToken cancellationToken)
        {
            var response = await _userService.AddAsync(request, cancellationToken);
            return response.HandleResult(nameof(GetUserByIdAsync), new
            {
                id = response.Value.Id
            });
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        [Authorize]
        public async Task<IActionResult> UpdateUserAsync(int id,UserUpdateRequest request
            ,CancellationToken cancellationToken)
        {
            var response = await _userService.UpdateAsync(id, request, cancellationToken);
            return response.HandleResult();
        }
        [HttpPut("{id}/activate")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        [Authorize]
        public async Task<IActionResult> ActivateUserAsync(int id
            , CancellationToken cancellationToken)
        {
            var response = await _userService.ActivateAsync(id,cancellationToken);
            return response.HandleResult();
        }
        [HttpPut("{id}/deactivate")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        [Authorize]
        public async Task<IActionResult> DeActivateUserAsync(int id
            , CancellationToken cancellationToken)
        {
            var response = await _userService.DesActivateAsync(id, cancellationToken);
            return response.HandleResult();
        }
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        [Authorize]
        public async Task<IActionResult> DeleteUserAsync(int id
            , CancellationToken cancellationToken)
        {
            var response = await _userService.DeleteAsync(id, cancellationToken);
            return response.HandleResult();
        }


        [HttpPut("{id}/password")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        [Authorize]
        public async Task<IActionResult> ChangePasswordAsync(int id
            , ChangePasswordRequest request, CancellationToken cancellationToken)
        {
            var response = await _userService.ChangePasswordAsync(id, request
                , cancellationToken);
            return response.HandleResult();
        }

    }
}
