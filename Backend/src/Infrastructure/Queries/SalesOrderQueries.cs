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
        GetSalesOrdersRequest request,
        CancellationToken cancellationToken = default)
    {
        var page = request.PageNumber is null || request.PageNumber <= 0 ? 1 : request.PageNumber.Value;
        var pageSize = request.PageSize is null || request.PageSize <= 0 ? 10 : request.PageSize.Value;

        var query = _context.SalesOrders.AsNoTracking();

        if (request.Status is not null)
        {
            query = query.Where(e => e.SalesStatus == request.Status.Value);
        }

        if (request.CustomerId is not null)
        {
            query = query.Where(e => e.CustomerId == request.CustomerId.Value);
        }

        if (request.DateFrom is not null)
        {
            var fromDate = request.DateFrom.Value.Date;
            query = query.Where(e => e.OrderDate >= fromDate);
        }

        if (request.DateTo is not null)
        {
            var nextDate = request.DateTo.Value.Date.AddDays(1);
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
                TotalAmount = e.Items.Sum(i => i.LineAmount),
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
}
