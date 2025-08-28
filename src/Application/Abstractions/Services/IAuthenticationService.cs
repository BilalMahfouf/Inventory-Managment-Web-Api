using Application.DTOs.Authentication;
using Application.Result;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Abstractions.Services
{
    public interface IAuthenticationService
    {
        Task<Result<LoginResponse>> LoginAsync
            (LoginRequest request, CancellationToken cancellationToken = default);
    }
}
