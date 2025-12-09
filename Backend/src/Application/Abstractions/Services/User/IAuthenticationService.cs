using Application.DTOs.Authentication;
using Application.DTOs.Authentication.Email;
using Application.DTOs.Authentication.Login;
using Application.DTOs.Authentication.Password;
using Application.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Abstractions.Services.User
{
    public interface IAuthenticationService
    {
        Task<Result<LoginResponse>> LoginAsync
            (LoginRequest request, CancellationToken cancellationToken = default);

        Task<Result<LoginResponse>> RefreshTokenAsync(RefreshTokenRequest request
            ,CancellationToken cancellationToken=default);

        Task<Result<string>> ResetPasswordAsync(ResetPasswordRequest request
            , CancellationToken cancellationToken = default);

        Task<Result<string>> ForgetPasswordAsync(ForgetPasswordRequest request,
            CancellationToken cancellationToken = default);

        Task<Result<string>> ConfirmEmailAsync(ConfirmEmailRequest request
            , CancellationToken cancellationToken = default);

        Task<Result<string>> SendConfirmEmailAsync(SendConfirmEmailRequest request
            , CancellationToken cancellationToken = default);
    }
}
