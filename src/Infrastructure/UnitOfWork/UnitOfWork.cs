using Application.Abstractions.UnitOfWork;
using Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.UOW
{
    internal class UnitOfWork : IUnitOfWork
    {
        private readonly InventoryManagmentDBContext _context;

        public UnitOfWork(InventoryManagmentDBContext context)
        {
            _context = context;
        }

        public ValueTask DisposeAsync()
        {
            return _context.DisposeAsync();
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
