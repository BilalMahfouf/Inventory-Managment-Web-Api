using Application.Abstractions.UnitOfWork;
using Application.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Shared
{
    public class DashboardService
    {
        private readonly IUnitOfWork _uow;

        public DashboardService(IUnitOfWork uow)
        {
            _uow = uow;
        }


        public async Task<Result<object>> GetDashboardSummaryAsync(
            CancellationToken cancellationToken = default)
        {
            try
            {
                var ActiveProducts = await _uow.Products.GetCountAsync
                    (p => !p.IsDeleted, cancellationToken);
                var LowStockProducts = await _uow.Inventories.GetCountAsync
                    (i => i.QuantityOnHand <= i.ReorderLevel, cancellationToken);
                var ActiveCustomers = await _uow.Customers.GetCountAsync(
                    c => c.IsActive && !c.IsDeleted, cancellationToken);
                var TotalSalesOrders = await _uow.SalesOrders.GetCountAsync(
                    e => e.CreatedAt >= DateTime.UtcNow.AddMonths(-1)
                    , cancellationToken);
                var result = new
                {
                    ActiveProducts,
                    LowStockProducts,
                    ActiveCustomers,
                    TotalSalesOrders
                };
                return Result<object>.Success(result);
            }
            catch (Exception ex)
            {
                return Result<object>.Exception(nameof(GetDashboardSummaryAsync), ex);
            }
        }
    }
}
