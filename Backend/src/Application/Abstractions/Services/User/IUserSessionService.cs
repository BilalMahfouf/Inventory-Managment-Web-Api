using Application.DTOs.Users.Response;
using Application.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Abstractions.Services.User
{
    public interface IUserSessionService
    {
        Task<Result<UserSessionReadResponse>> GetAllUserSessionsAsync(
           CancellationToken cancellationToken);
        Task<Result> DeleteAsync(int userId, CancellationToken cancellationToken);

    }
}
