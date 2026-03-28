using Application.Shared.Contracts;
using Domain.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Users.Contracts
{
    public interface IUserSessionRepository:IBaseRepository<UserSession>
    {
        Task DeleteAllSessionsByUserIdAsync(int userId
            , CancellationToken cancellationToken);
    }
}
