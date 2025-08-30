using Application.Abstractions.Services.User;
using Application.DTOs.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Presentation.Extensions;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;

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

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<LoginResponse>> LoginAsync([FromBody]
            LoginRequest loginRequest,CancellationToken cancellationToken)
        {
            var response= await _service.LoginAsync(loginRequest,cancellationToken);
            return response.HandleResult();
        }

        [HttpPost("refresh-token")]

        public async Task<ActionResult<LoginResponse>> RefreshTokenAsync(
           [FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
        {
            var response = await _service.RefreshTokenAsync(request, cancellationToken);
            return response.HandleResult();
        }

        [HttpPut("reset-password")]
        [Authorize]
        public async Task<ActionResult<string>> ResetPasswordAsync(
            ResetPasswordRequest request
            ,CancellationToken cancellationToken)
        {
            var response =await _service.ResetPasswordAsync(request, cancellationToken);
            return response.HandleResult();
        }
        
    }
}
