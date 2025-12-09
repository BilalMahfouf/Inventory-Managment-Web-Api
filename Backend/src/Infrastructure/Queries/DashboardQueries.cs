using Application.Abstractions.Queries;
using Application.Results;
using Domain.Enums;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Queries;

public sealed class DashboardQueries : IDashboardQueries
{

    private readonly InventoryManagmentDBContext _context;

    public DashboardQueries(InventoryManagmentDBContext context)
    {
        _context = context;
    }

    public Task<object> GetDashboardSummaryAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<object>> GetTodayPerformanceAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var todayNewOrders =await _context.SalesOrders
               .Where(so => so.OrderDate.Date == DateTime.UtcNow)
               .CountAsync(cancellationToken);

            var todayNewCustomers = await _context.Customers
                .Where(c => c.CreatedAt.Date == DateTime.UtcNow)
                .CountAsync(cancellationToken);
            var todaySoldProducts = await _context.SalesOrderItems
                .Where(soi => soi.SalesOrder.OrderDate.Date == DateTime.UtcNow)
                .CountAsync(cancellationToken);
            var today = DateTime.UtcNow.Date;
            var totalRevenues = await(from s in _context.SalesOrderItems
                                 join so in _context.SalesOrders on s.SalesOrderId equals so.Id
                                 join p in _context.Products on s.ProductId equals p.Id
                                 where so.SalesStatus == (byte)SalesOrderStatus.Completed
                                 && so.OrderDate.Date == today
                                 select new
                                 {
                                     Revenue = s.LineAmount - (p.Cost * s.OrderedQuantity)
                                 }).SumAsync(x => x.Revenue, cancellationToken);
            var result = new
            {
                TodayNewOrders =  todayNewOrders,
                TodayNewCustomers = todayNewCustomers,
                TodaySoldProducts = todaySoldProducts,
                TodayRevenues = totalRevenues
            };
            if (result is null)
                return Result<object>.NotFound("today performance");
            return Result<object>.Success(result);
        }
        catch (Exception ex)
        {
            return Result<object>.Exception(nameof(GetTodayPerformanceAsync), ex);
        }
    }
}

