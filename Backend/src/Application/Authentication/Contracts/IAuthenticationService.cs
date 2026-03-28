using Application.Authentication.DTOs;
using Application.Authentication.DTOs.Email;
using Application.Authentication.DTOs.Login;
using Application.Authentication.DTOs.Password;
using Domain.Shared.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Authentication.Contracts
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
