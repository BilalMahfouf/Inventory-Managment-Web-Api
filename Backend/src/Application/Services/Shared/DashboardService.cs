using Application.Abstractions.UnitOfWork;
using Application.Results;
using Domain.Enums;
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
                var TotalProducts = await _uow.Products.GetCountAsync
                    (p => !p.IsDeleted, cancellationToken);
                var LowStockProducts = await _uow.Inventories.GetCountAsync
                    (i => i.QuantityOnHand <= i.ReorderLevel, cancellationToken);
                var ActiveCustomers = await _uow.Customers.GetCountAsync(
                    c => c.IsActive && !c.IsDeleted, cancellationToken);
                var TotalSalesOrders = await _uow.SalesOrders.GetCountAsync(null
                    , cancellationToken);
                var TotalRevenues = await _uow.SalesOrderItems
                    .GetTotalRevenuesAsync(cancellationToken);

                var PendingSalesOrders = await _uow.SalesOrders.GetCountAsync(
                    e => e.SalesStatus == (byte)SalesOrderStatus.Pending
                    , cancellationToken);

                var CompletedSalesOrders = await _uow.SalesOrders.GetCountAsync(
                    e => e.SalesStatus == (byte)SalesOrderStatus.Completed
                    , cancellationToken);

                var ActiveSuppliers = await _uow.Suppliers.GetCountAsync(
                    s => s.IsActive && !s.IsDeleted, cancellationToken);

                var result = new
                {
                    TotalProducts,
                    LowStockProducts,
                    ActiveCustomers,
                    TotalSalesOrders,
                    TotalRevenues,
                    PendingSalesOrders,
                    ActiveSuppliers,
                    CompletedSalesOrders
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
    
        public async Task<Result<IEnumerable<object>>> GetTopSellingProductsAsync(
            int numberOfProducts = 5
            , CancellationToken cancellationToken = default)
        {
            if (numberOfProducts <= 0)
            {
                return Result<IEnumerable<object>>.Failure("invalid number of products"
                    , ErrorType.BadRequest);
            }
            try
            {
                var topProducts = await _uow.SalesOrderItems
                    .GetTopSellingProductsAsync(numberOfProducts, cancellationToken);
                if (topProducts == null || !topProducts.Any())
                {
                    return Result<IEnumerable<object>>
                        .NotFound(nameof(topProducts));
                }
                return Result<IEnumerable<object>>.Success(topProducts);
            }
            catch (Exception ex)
            {
                return Result<IEnumerable<object>>
                    .Exception(nameof(GetTopSellingProductsAsync), ex);
            }
        }
    
    }
}
