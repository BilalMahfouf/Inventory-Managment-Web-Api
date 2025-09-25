using Application.Abstractions.Queries;
using Application.Results;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Queries
{
    public class ProductQueries : IProductQueries
    {
        private readonly InventoryManagmentDBContext _context;

        public ProductQueries(InventoryManagmentDBContext context)
        {
            _context = context;
        }

        public async Task<Result<object>> GetProductDashboardSummaryAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var totalProductsCount = await _context.Products.CountAsync(cancellationToken);
                var inventoryValue = await (from i in _context.Inventories
                                            join p in _context.Products on i.ProductId equals p.Id
                                            select i.QuantityOnHand * p.UnitPrice)
                                            .SumAsync(cancellationToken);
                var lowStockProductsCount = await _context.Inventories
                  .Where(i => i.QuantityOnHand <= i.ReorderLevel)
                  .CountAsync(cancellationToken);
                var profitPotential = await (from i in _context.Inventories
                                             join p in _context.Products on i.ProductId equals p.Id
                                             select i.QuantityOnHand * (p.UnitPrice - p.Cost))
                                            .SumAsync(cancellationToken);
                var result = new
                {
                    TotalProducts = totalProductsCount,
                    InventoryValue = inventoryValue,
                    LowStockProducts = lowStockProductsCount,
                    ProfitPotential = profitPotential
                };
                if(result is null)
                {
                    return Result<object>.NotFound("no summary data is found");
                }
                return Result<object>.Success(result);
            }
            catch(Exception ex)
            {
                return Result<object>.Exception(nameof(GetProductDashboardSummaryAsync), ex) ;
            }
        }
    }
}
