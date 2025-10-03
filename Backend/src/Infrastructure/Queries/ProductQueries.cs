using Application.Abstractions.Queries;
using Application.DTOs.Products.Response.Products;
using Application.PagedLists;
using Application.Results;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Infrastructure.Queries;

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
              .Where(i => i.QuantityOnHand <= i.ReorderLevel).AsNoTracking()
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
            if (result is null)
            {
                return Result<object>.NotFound("no summary data is found");
            }
            return Result<object>.Success(result);
        }
        catch (Exception ex)
        {
            return Result<object>.Exception(nameof(GetProductDashboardSummaryAsync), ex);
        }
    }
    public async Task<Result<PagedList<ProductTableResponse>>> GetAllAsync(
        TableRequest request, CancellationToken cancellationToken = default)
    {

        try
        {
            var count = await
                        _context.Products
                        .Where(p => _context.Inventories.Any(i => i.ProductId == p.Id))
                        .Select(p => p.Id)
                        .Distinct()
                        .CountAsync();
            if (count == 0)
            {
                return Result<PagedList<ProductTableResponse>>.NotFound("Products");
            }

            var productsQuery = _context.Products.AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.search))
            {
                // use this for performance 
                productsQuery = productsQuery.Where(p =>
                    EF.Functions.Like(p.Sku, $"%{request.search}%") ||
                    EF.Functions.Like(p.Name, $"%{request.search}%"));
            }

            var query = (from p in productsQuery
                         join i in _context.Inventories on p.Id equals i.ProductId
                         join pc in _context.ProductCategories on p.CategoryId equals pc.Id
                         group new { p, i, pc } by p.Id into g
                         select new ProductTableResponse
                         {
                             Id = g.Key,
                             SKU = g.Min(x => x.p.Sku) ?? string.Empty,
                             Product = g.Min(x => x.p.Name) ?? string.Empty,
                             Stock = g.Sum(x => x.i.QuantityOnHand),
                             CategoryId = g.Min(x => x.p.CategoryId),
                             Category = g.Min(x => x.pc.Name) ?? string.Empty,
                             Price = g.Min(x => x.p.UnitPrice),
                             Cost = g.Min(x => x.p.Cost),
                             IsActive = g.Select(e => e.p.IsActive).FirstOrDefault(),
                             CreatedAt = g.Min(x => x.p.CreatedAt)
                         });


            Expression<Func<ProductTableResponse, object>> orderSelector =
               request.SortColumn?.ToLower() switch
               {
                   "sku" => p => p.SKU,
                   "product" => p => p.Product,
                   "price" => p => p.Price,
                   "stock" => p => p.Stock,
                   "createdAt" => p => p.CreatedAt,
                   _ => p => p.Id
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
                return Result<PagedList<ProductTableResponse>>.NotFound("Products");
            }


            var result = new PagedList<ProductTableResponse>
            {
                Item = item,
                Page = request.Page,
                PageSize = item.Count,
                TotalCount = count // to do make this a background job for performance 
            };
            return Result<PagedList<ProductTableResponse>>.Success(result);
        }
        catch (Exception ex)
        {
            return Result<PagedList<ProductTableResponse>>.Exception(
                nameof(GetAllAsync), ex);
        }
    }

    public async Task<Result<PagedList<object>>> GetStockMovementsHistoryAsync(
        TableRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var count = await _context.StockMovements.CountAsync(cancellationToken);
            if (count is 0)
            {
                return Result<PagedList<object>>.NotFound("Stock Movements");
            }


            var query = _context.StockMovements
                        .Include(e => e.Product)
                        .Include(e => e.MovementType)
                        .Include(e => e.CreatedByUser)
                        .Select(e => new
                        {

                            Id = e.Id,
                            Product = e.Product.Name,
                            Type = e.MovementType.Direction.ToString(),
                            Queries = e.Quantity,
                            Reason = e.MovementType.Name,
                            CreatedAt = e.CreatedAt,
                            CreatedByUser = e.CreatedByUser.UserName,
                        });
            query = query.Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize);
            var item = await query.AsNoTracking().ToListAsync(cancellationToken);
            if (item is null || !item.Any())
            {
                return Result<PagedList<object>>.NotFound("Stock Movements");
            }
            var result = new PagedList<object>
            {
                Item = item,
                Page = request.Page,
                PageSize = item.Count,
                TotalCount = count
            };
            return Result<PagedList<object>>.Success(result);


        }
        catch (Exception ex)
        {
            return Result<PagedList<object>>
                .Exception(nameof(GetStockMovementsHistoryAsync), ex);
        }
    }





}

