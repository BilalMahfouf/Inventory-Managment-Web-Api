using Application.PagedLists;
using Application.Results;
using Application.Sales.Queries;
using Application.Sales.RequestResponse;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Infrastructure.Queries;

internal class SalesOrderQueries : ISalesOrderQueries
{
    private readonly InventoryManagmentDBContext _context;

    public SalesOrderQueries(InventoryManagmentDBContext context)
    {
        _context = context;
    }

    public Task<Result<object>> GetDahsboardSummaryAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<PagedList<SalesOrderTableResponse>>> GetOrdersTableAsync(TableRequest request, CancellationToken cancellationToken = default)
    {
        var count = await _context.SalesOrders.CountAsync(cancellationToken);
        if (count is 0)
        {
            return Result<PagedList<SalesOrderTableResponse>>.NotFound("Sales Orders");
        }
        var query = _context.SalesOrders
            .Select(so => new SalesOrderTableResponse
            {
                Id = so.Id,
                CustomerName = so.Customer.Name,
                CustomerEmail = so.Customer.Email,
                OrderDate = so.OrderDate,
                TotalAmount = so.TotalAmount,
                Items = so.Items.Count(),
                Status = so.SalesStatus.ToString()
            });
        if (!string.IsNullOrWhiteSpace(request.search))
        {
            query = query.Where(r =>
                r.CustomerName.Contains(request.search) ||
                r.CustomerEmail.Contains(request.search) ||
                r.Status.Contains(request.search));
        }
        Expression<Func<SalesOrderTableResponse, object>> orderSelector =
            request.SortColumn?.ToLower() switch
            {
                "customername" => r => r.CustomerName,
                "customeremail" => r => r.CustomerEmail,
                "orderdate" => r => r.OrderDate,
                "totalamount" => r => r.TotalAmount,
                "items" => r => r.Items,
                "status" => r => r.Status,
                _ => r => r.Id
            };
        if (request.SortOrder is "desc")
        {
            query = query.OrderByDescending(orderSelector);
        }
        else
        {
            query = query.OrderBy(orderSelector);
        }
        query = query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize);

        var data = await query.ToListAsync(cancellationToken);
        if (data is null || !data.Any())
        {
            return Result<PagedList<SalesOrderTableResponse>>.NotFound("Sales Orders");
        }
        var result = new PagedList<SalesOrderTableResponse>
        {
            Item = data,
            TotalCount = count,
            Page = request.Page,
            PageSize = request.PageSize
        };
        return Result<PagedList<SalesOrderTableResponse>>.Success(result);
    }
}
