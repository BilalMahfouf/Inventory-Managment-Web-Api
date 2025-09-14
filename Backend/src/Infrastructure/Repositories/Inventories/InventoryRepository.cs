using Application.Abstractions.Repositories.Inventories;
using Domain.Entities;
using Infrastructure.Persistence;
using Infrastructure.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories.Inventories
{
    public class InventoryRepository : BaseRepository<Inventory>,
        IInventoryRepository
    {
        public InventoryRepository(InventoryManagmentDBContext context) : base(context)
        {
        }
        public async Task<decimal> GetInventoryCostAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Inventories.SumAsync
                (i => i.QuantityOnHand * i.Product.Cost, cancellationToken);
        }

        public async Task<decimal> GetInventoryValuationAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Inventories.SumAsync
                (i => i.QuantityOnHand * i.Product.UnitPrice, cancellationToken);
        }
    }
}
