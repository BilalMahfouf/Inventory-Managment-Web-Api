using Application.Users.DTOs.Response;
using Domain.Shared.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Users.Contracts
{
    public interface IUserSessionService
    {
        Task<Result<UserSessionReadResponse>> GetAllUserSessionsAsync(
           CancellationToken cancellationToken);
        Task<Result> DeleteAsync(int userId, CancellationToken cancellationToken);

    }
}
