using Application.Abstractions.Services;
using Application.DTOs.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers.Auth
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService _service;

        public AuthenticationController(IAuthenticationService service)
        {
            _service = service;
        }

        [HttpGet("login")]
        public async Task<ActionResult<LoginResponse>> LoginAsync(
            LoginRequest loginRequest,CancellationToken cancellationToken)
        {
            var response= await _service.LoginAsync(loginRequest,cancellationToken);
            return Ok(response);
        }
    }
}
