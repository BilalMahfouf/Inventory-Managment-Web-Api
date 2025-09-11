using Application.Abstractions.Services.Email;
using Application.Abstractions.Services.User;
using Application.DTOs.Authentication;
using Application.DTOs.Authentication.Email;
using Application.DTOs.Authentication.Login;
using Application.DTOs.Authentication.Password;
using Application.DTOs.Email;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Presentation.Extensions;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;

namespace Presentation.Controllers.Auth
{
    [Route("api/auth")]
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
            LoginRequest loginRequest, CancellationToken cancellationToken)
        {
            var response = await _service.LoginAsync(loginRequest, cancellationToken);
            return response.HandleResult();
        }

        [HttpPost("refresh-token")]

        public async Task<ActionResult<LoginResponse>> RefreshTokenAsync(
           [FromBody] RefreshTokenRequest request, CancellationToken cancellationToken)
        {
            var response = await _service.RefreshTokenAsync(request, cancellationToken);
            return response.HandleResult();
        }

        [HttpPost("reset-password")]

        public async Task<ActionResult<string>> ResetPasswordAsync(
            ResetPasswordRequest request
            , CancellationToken cancellationToken)
        {
            var response = await _service.ResetPasswordAsync(request, cancellationToken);
            return response.HandleResult();
        }

        [HttpPost("forget-password")]
        public async Task<ActionResult<string>> ForgetPasswordAsync(
            ForgetPasswordRequest request, CancellationToken cancellationToken)
        {
            var response = await _service.ForgetPasswordAsync(request, cancellationToken);
            return response.HandleResult();
        }

        [HttpPost("confirm-email")]
        public async Task<ActionResult<string>> ConfirmEmailAsync(ConfirmEmailRequest request
            , CancellationToken cancellationToken)
        {
            var response = await _service.ConfirmEmailAsync(request, cancellationToken);
            return response.HandleResult();
        }

        [HttpPost("send-confirm-email")]
        public async Task<ActionResult<string>> SendConfirmEmailAsync(
            SendConfirmEmailRequest request, CancellationToken cancellationToken)
        {
            var response = await _service.SendConfirmEmailAsync(request
                , cancellationToken);
            return response.HandleResult();
        }

    }
}
