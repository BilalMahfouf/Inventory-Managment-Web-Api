using Application.Abstractions.Queries;
using Application.DTOs.Customers;
using Application.PagedLists;
using Application.Results;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Queries;

internal class CustomerQueries : ICustomerQueries
{
    private readonly InventoryManagmentDBContext _context;

    public CustomerQueries(InventoryManagmentDBContext context)
    {
        _context = context;
    }

    public async Task<Result<PagedList<CustomerTableReadResponse>>> GetAllAsync(
        TableRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var count = await _context.Customers.CountAsync(cancellationToken);
            if (count <= 0)
            {
                return Result<PagedList<CustomerTableReadResponse>>
                    .NotFound("Customers");
            }
            var customerQuery = _context.Customers.AsQueryable();
            if (!string.IsNullOrWhiteSpace(request.search))
            {
                // use this for performance 
                customerQuery = customerQuery.Where(p =>
                    EF.Functions.Like(p.Name, $"%{request.search}%") ||
                    EF.Functions.Like(p.Phone, $"%{request.search}%") ||
                    EF.Functions.Like(p.Email, $"%{request.search}%"));
            }

            var query =
    from c in _context.Customers
    join cc in _context.CustomerCategories
        on c.CustomerCategoryId equals cc.Id into catJoin
    from cc in catJoin.DefaultIfEmpty()   // LEFT JOIN

    join so in _context.SalesOrders
        on c.Id equals so.CustomerId into soJoin
    from so in soJoin.DefaultIfEmpty()    // LEFT JOIN

    group new { c, cc, so } by new
    {
        c.Id,
        c.Name,
        c.Email,
        c.Phone,
        c.CustomerCategoryId,
        CustomerCategoryName = cc.Name,
        c.IsActive,
        c.CreatedAt
    }
    into g
    select new CustomerTableReadResponse
    {
        Id = g.Key.Id,
        Name = g.Key.Name,
        Email = g.Key.Email,
        Phone = g.Key.Phone,
        CustomerCategoryId = g.Key.CustomerCategoryId,
        CustomerCategoryName = g.Key.CustomerCategoryName,

        TotalOrders = g.Count(x => x.so != null),

        TotalSpent = g.Sum(x =>
            x.so != null && x.so.SalesStatus == 2
                ? x.so.TotalAmount
                : 0
        ),

        IsActive = g.Key.IsActive,
        CreatedAt = g.Key.CreatedAt
    };


            Expression<Func<CustomerTableReadResponse, object>> orderSelector =
                request.SortColumn?.ToLower() switch
                {
                    "name" => x => x.Name,
                    "customercategoryname" => x => x.CustomerCategoryName,
                    "totalorders" => x => x.TotalOrders,
                    "totalspent" => x => x.TotalSpent,
                    "createdat" => x => x.CreatedAt,
                    _ => x => x.Id

                };
            if (request.SortOrder?.ToLower() == "desc")
            {
                query = query.OrderByDescending(orderSelector);
            }
            else
            {
                query = query.OrderBy(orderSelector);
            }

            query = query.Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize);

            var item = await query.AsNoTracking().ToListAsync(cancellationToken);
            if (item is null || !item.Any())
            {
                return Result<PagedList<CustomerTableReadResponse>>
                    .NotFound("Customers");
            }

            var result = new PagedList<CustomerTableReadResponse>()
            {
                Item = item,
                Page = request.Page,
                PageSize = request.PageSize,
                TotalCount = count
            };
            return Result<PagedList<CustomerTableReadResponse>>.Success(result);
        }
        catch (Exception ex)
        {
            return Result<PagedList<CustomerTableReadResponse>>.Exception(
                nameof(GetAllAsync),
                ex);
        }


    }

    public async Task<Result<CustomerReadResponse>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        if (id <= 0)
        {
            return Result<CustomerReadResponse>.InvalidId();
        }
        try
        {

            var customer = await _context.Customers
                .Select(e => new CustomerReadResponse
                {
                    Id = e.Id,
                    Name = e.Name,
                    Email = e.Email,
                    Phone = e.Phone,
                    CustomerCategoryId = e.CustomerCategoryId,
                    CustomerCategoryName = e.CustomerCategory == null ? null : e.CustomerCategory.Name,
                    IsActive = e.IsActive,

                    Street = e.Address.Street,
                    City = e.Address.City,
                    State = e.Address.State,
                    ZipCode = e.Address.ZipCode,

                    CreditLimit = e.CreditLimit,
                    CreditStatus = e.CreditStatus.ToString(),
                    PaymentTerm = e.PaymentTerms!,
                    CreatedAt = e.CreatedAt,
                    CreatedByUserId = e.CreatedByUserId,
                    CreatedByUserName = e.CreatedByUser.UserName
                }).FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
            if (customer is null)
            {
                return Result<CustomerReadResponse>.NotFound($"Customer with Id {id}");
            }
            return Result<CustomerReadResponse>.Success(customer);
        }
        catch (Exception ex)
        {
            return Result<CustomerReadResponse>.Exception(
                nameof(GetByIdAsync),
                nameof(CustomerQueries),
                ex);
        }
    }

    public async Task<Result<object>> GetCustomerSummary(CancellationToken cancellationToken = default)
    {
        try
        {
            var totalCustomers = await _context.Customers.CountAsync();
            var activeCustomers = await _context.Customers.CountAsync(c => c.IsActive);
            var totalRevenue = await _context.SalesOrders
                .Where(so => so.SalesStatus == 2)
                .SumAsync(so => so.TotalAmount);
            var newCustomersLastMonth = await _context.Customers
                .Where(c => c.CreatedAt >= DateTime.UtcNow.AddMonths(-1))
                .CountAsync();

            var summary = new
            {
                TotalCustomers = totalCustomers,
                TotalRevenue = totalRevenue,
                NewCustomersLastMonth = newCustomersLastMonth,
                ActiveCustomers = activeCustomers,

            };
            return Result<object>.Success(summary);
        }
        catch (Exception ex)
        {
            return Result<object>.Exception(
                nameof(GetCustomerSummary),
                ex);
        }
    }
}
