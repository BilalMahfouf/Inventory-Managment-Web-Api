using Application.Abstractions.Repositories;
using Domain.Entities;
using Infrastructure.Persistence;
using Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.User
{
    internal class UserSessionRepository : BaseRepository<UserSession>,IUserSessionRepository
    {
        public UserSessionRepository(InventoryManagmentDBContext context) : base(context)
        {
        }
        public async Task DeleteAllSessionsByUserIdAsync(int userId
            ,CancellationToken cancellationToken)
        {
            await _context.UserSessions
                .Where(u => u.UserId == userId)
                .ExecuteDeleteAsync(cancellationToken);

        }
    }
}
