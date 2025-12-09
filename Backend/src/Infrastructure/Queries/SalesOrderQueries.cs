using Application.Results;
using Application.Sales;
using Domain.Enums;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Queries;

internal class SalesOrderQueries : ISalesOrderQueries
{
    private readonly InventoryManagmentDBContext _context;

    public SalesOrderQueries(InventoryManagmentDBContext context)
    {
        _context = context;
    }

    public async Task<Result<object>> GetDashboardSumamryAsync(
        CancellationToken cancellationToken = default)
    {
        try
        {
            var totalOrders = await _context.SalesOrders.CountAsync(cancellationToken);
            var pendingOrders = await _context.SalesOrders
                .CountAsync(e => e.SalesStatus == (byte)SalesOrderStatus.Pending
                , cancellationToken);

            var averageOrderValue = await _context.SalesOrders
                .AverageAsync(e => e.TotalAmount, cancellationToken);

            var revenueThisMonth = await _context.SalesOrders
                .Where(e => e.OrderDate >= DateTime.UtcNow.AddMonths(-1))
                .SumAsync(e => e.TotalAmount, cancellationToken);
            var dashboardSummary = new
            {
                TotalOrders = totalOrders,
                PendingOrders = pendingOrders,
                AverageOrderValue = averageOrderValue,
                RevenueThisMonth = revenueThisMonth
            };
            return Result<object>.Success(dashboardSummary);
        }
        catch (Exception ex)
        {
            return Result<object>.Exception(nameof(GetDashboardSumamryAsync), ex);
        }
    }

    public Task<Result<object>> GetOrderByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
