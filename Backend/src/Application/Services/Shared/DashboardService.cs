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
                var TotalSalesOrders = await _uow.SalesOrders.GetCountAsync(null
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


        // to do make it real time with SignalR
        public async Task<Result<IEnumerable<object>>> GetInventoryAlertsAsync(
            CancellationToken cancellationToken = default)
        {
            try
            {
                var lowStockProducts = await _uow.Inventories.GetAllAsync(
                    i => i.QuantityOnHand <= i.ReorderLevel,
                    includeProperties: "Product",
                    cancellationToken: cancellationToken);
                if(lowStockProducts == null || !lowStockProducts.Any())
                {
                    return Result<IEnumerable<object>>.NotFound(nameof(lowStockProducts));
                }
                var alerts = lowStockProducts.Select(i => new
                {
                    ProductName = i.Product.Name,
                    Description = i.QuantityOnHand == 0 ?
                        "Out of stock" : $"Only {(int)i.QuantityOnHand} units left",
                    Status = i.QuantityOnHand == 0 ? "critical" : "high"
                }).ToList();
                return Result<IEnumerable<object>>.Success(alerts);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<object>>
                    .Exception(nameof(GetInventoryAlertsAsync), ex);
            }

        }
    }
}
