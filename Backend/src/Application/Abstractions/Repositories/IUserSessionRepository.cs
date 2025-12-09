using Application.Abstractions.Repositories.Base;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Abstractions.Repositories
{
    public interface IUserSessionRepository:IBaseRepository<UserSession>
    {
        Task DeleteAllSessionsByUserIdAsync(int userId
            , CancellationToken cancellationToken);
    }
}
