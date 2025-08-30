using Application.DTOs.Authentication;
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
    }
}
