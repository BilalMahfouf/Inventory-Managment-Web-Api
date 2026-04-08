using Application.Sales.Queries;
using Application.Sales.RequestResponse;
using Application.Shared.Paging;
using Domain.Shared.Errors;
using Domain.Shared.Results;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Infrastructure.Queries;

internal class SalesOrderQueries : ISalesOrderQueries
{
    private readonly InventoryManagmentDBContext _context;

    public SalesOrderQueries(InventoryManagmentDBContext context)
    {
        _context = context;
    }

    public async Task<Result<PagedList<SalesOrderTableResponse>>> GetSalesOrdersAsync(
        TableRequest request,
        SalesOrderStatus? status,
        int? customerId,
        DateTime? dateFrom,
        DateTime? dateTo,
        CancellationToken cancellationToken = default)
    {
        var page = request.Page <= 0 ? 1 : request.Page;
        var pageSize = request.PageSize <= 0 ? 10 : request.PageSize;

        var query = _context.SalesOrders.AsNoTracking();

        if (status is not null)
        {
            query = query.Where(e => e.SalesStatus == status.Value);
        }

        if (customerId is not null)
        {
            query = query.Where(e => e.CustomerId == customerId.Value);
        }

        if (dateFrom is not null)
        {
            var fromDate = dateFrom.Value.Date;
            query = query.Where(e => e.OrderDate >= fromDate);
        }

        if (dateTo is not null)
        {
            var nextDate = dateTo.Value.Date.AddDays(1);
            query = query.Where(e => e.OrderDate < nextDate);
        }

        var projectedQuery = query.Select(so => new SalesOrderTableResponse
        {
            Id = so.Id,
            CustomerId = so.CustomerId,
            CustomerName = so.Customer != null ? so.Customer.Name : "Walk-in",
            CustomerEmail = so.Customer != null ? so.Customer.Email : null,
            IsWalkIn = so.IsWalkIn,
            OrderDate = so.OrderDate,
            TotalAmount = so.Items.Sum(i => i.LineAmount),
            Items = so.Items.Count(),
            Status = so.SalesStatus.ToString(),
            PaymentStatus = so.PaymentStatus.ToString(),
        });

        Expression<Func<SalesOrderTableResponse, object>> orderSelector =
            request.SortColumn?.ToLower() switch
            {
                "customername" => r => r.CustomerName!,
                "orderdate" => r => r.OrderDate,
                "totalamount" => r => r.TotalAmount,
                "items" => r => r.Items,
                "status" => r => r.Status,
                "paymentstatus" => r => r.PaymentStatus,
                _ => r => r.Id,
            };

        projectedQuery = request.SortOrder?.ToLower() == "desc"
            ? projectedQuery.OrderByDescending(orderSelector)
            : projectedQuery.OrderBy(orderSelector);

        var count = await projectedQuery.CountAsync(cancellationToken);
        if (count == 0)
        {
            return Result<PagedList<SalesOrderTableResponse>>.NotFound("Sales Orders");
        }

        var data = await projectedQuery
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var result = new PagedList<SalesOrderTableResponse>
        {
            Item = data,
            TotalCount = count,
            Page = page,
            PageSize = pageSize,
        };

        return Result<PagedList<SalesOrderTableResponse>>.Success(result);
    }

    public async Task<Result<SalesOrderReadResponse>> GetSalesOrderByIdAsync(
        int orderId,
        CancellationToken cancellationToken)
    {
        var order = await _context.SalesOrders
            .AsNoTracking()
            .Where(e => e.Id == orderId)
            .Select(e => new SalesOrderReadResponse
            {
                Id = e.Id,
                CustomerId = e.CustomerId,
                CustomerName = e.Customer != null ? e.Customer.Name : "Walk-in",
                CustomerEmail = e.Customer != null ? e.Customer.Email : null,
                IsWalkIn = e.IsWalkIn,
                OrderDate = e.OrderDate,
                TotalAmount = e.TotalAmount,
                SalesStatus = e.SalesStatus.ToString(),
                PaymentStatus = e.PaymentStatus.ToString(),
                Description = e.Description,
                ShippingAddress = e.ShippingAddress,
                TrackingNumber = e.TrackingNumber,
                Items = e.Items.Select(i => new SalesOrderItemResponse
                {
                    Id = i.Id,
                    ProductId = i.ProductId,
                    InventoryId = i.InventoryId,
                    LocationId = i.LocationId,
                    LocationName = i.Inventory.Location.Name,
                    ProductName = i.Product.Name,
                    Quantity = i.OrderedQuantity,
                    UnitPrice = i.UnitCost,
                    TotalPrice = i.LineAmount,
                }),
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (order is null)
        {
            return Result<SalesOrderReadResponse>.Failure(Error.NotFound("Sales Order"));
        }

        return Result<SalesOrderReadResponse>.Success(order);
    }

    public async Task<Result<object>> GetDahsboardSummaryAsync(
           CancellationToken cancellationToken = default)
    {
        var totalOrders = await _context.SalesOrders.CountAsync(cancellationToken);
        var pendingOrders = await _context.SalesOrders
            .CountAsync(e => e.SalesStatus == SalesOrderStatus.Pending
            , cancellationToken);

        var averageOrderValue = await _context.SalesOrders
            .AverageAsync(e => e.TotalAmount, cancellationToken);

        var revenueThisMonth = await _context.SalesOrders
            .Where(e => e.OrderDate >= DateTime.UtcNow.AddMonths(-1))
            .SumAsync(e => e.TotalAmount, cancellationToken);
        var completedOrders = await _context.SalesOrders
           .CountAsync(
            e => e.SalesStatus == SalesOrderStatus.Completed,
            cancellationToken);

        var dashboardSummary = new
        {
            TotalOrders = totalOrders,
            PendingOrders = pendingOrders,
            AverageOrderValue = averageOrderValue,
            RevenueThisMonth = revenueThisMonth,
            CompletedOrders = completedOrders,
        };
        return Result<object>.Success(dashboardSummary);
    }

}
