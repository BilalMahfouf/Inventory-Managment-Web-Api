using Application.Products.Contracts;
using Application.Products.DTOs.Response.Products;
using Application.Shared.Paging;
using Domain.Shared.Results;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

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
                                        select i.QuantityOnHand * p.Cost)
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
                return Result<object>.Failure(Error.NotFound("no summary data is found"));
            }
            return Result<object>.Success(result);
        }
        catch (Exception ex)
        {
            return Result<object>.Failure(Error.Exception(nameof(GetProductDashboardSummaryAsync), ex));
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


            query = ApplyProductOrdering(query, request.SortColumn, request.SortOrder);

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

    public async Task<Result<PagedList<StockMovementsHistoryTableResponse>>>
        GetStockMovementsHistoryAsync(
        TableRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var count = await _context.StockMovements.CountAsync(cancellationToken);
            if (count is 0)
            {
                return Result<PagedList<StockMovementsHistoryTableResponse>>.NotFound("Stock Movements");
            }


            var query = _context.StockMovements
                        .Select(e => new StockMovementsHistoryTableResponse
                        {

                            Id = e.Id,
                            Product = e.Product.Name,
                            Sku = e.Product.Sku,
                            Type = e.MovementType.Direction.ToString(),
                            Quantity = e.Quantity,
                            Reason = e.MovementType.Name,
                            CreatedAt = e.CreatedAt,
                            CreatedByUser = e.CreatedByUser.UserName,
                        });
            query = ApplyStockMovementsHistoryOrdering(query, request.SortColumn, request.SortOrder);

            query = query.Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize);
            var item = await query.AsNoTracking().ToListAsync(cancellationToken);
            if (item is null || !item.Any())
            {
                return Result<PagedList<StockMovementsHistoryTableResponse>>.NotFound("Stock Movements");
            }
            var result = new PagedList<StockMovementsHistoryTableResponse>
            {
                Item = item,
                Page = request.Page,
                PageSize = item.Count,
                TotalCount = count
            };
            return Result<PagedList<StockMovementsHistoryTableResponse>>.Success(result);


        }
        catch (Exception ex)
        {
            return Result<PagedList<StockMovementsHistoryTableResponse>>
                .Exception(nameof(GetStockMovementsHistoryAsync), ex);
        }
    }

    private static IQueryable<ProductTableResponse> ApplyProductOrdering(
        IQueryable<ProductTableResponse> query,
        string? sortColumn,
        string? sortOrder)
    {
        bool desc = sortOrder?.ToLower() == "desc";

        return sortColumn?.ToLower() switch
        {
            "sku" => desc ? query.OrderByDescending(p => p.SKU) : query.OrderBy(p => p.SKU),
            "product" => desc ? query.OrderByDescending(p => p.Product) : query.OrderBy(p => p.Product),
            "price" => desc ? query.OrderByDescending(p => p.Price) : query.OrderBy(p => p.Price),
            "stock" => desc ? query.OrderByDescending(p => p.Stock) : query.OrderBy(p => p.Stock),
            "createdat" => desc ? query.OrderByDescending(p => p.CreatedAt) : query.OrderBy(p => p.CreatedAt),
            _ => desc ? query.OrderByDescending(p => p.Id) : query.OrderBy(p => p.Id),
        };
    }

    private static IQueryable<StockMovementsHistoryTableResponse> ApplyStockMovementsHistoryOrdering(
        IQueryable<StockMovementsHistoryTableResponse> query,
        string? sortColumn,
        string? sortOrder)
    {
        bool desc = sortOrder?.ToLower() == "desc";

        return sortColumn?.ToLower() switch
        {
            "product" => desc ? query.OrderByDescending(p => p.Product) : query.OrderBy(p => p.Product),
            "quantity" => desc ? query.OrderByDescending(p => p.Quantity) : query.OrderBy(p => p.Quantity),
            "date" => desc ? query.OrderByDescending(p => p.CreatedAt) : query.OrderBy(p => p.CreatedAt),
            "createdat" => desc ? query.OrderByDescending(p => p.CreatedAt) : query.OrderBy(p => p.CreatedAt),
            _ => desc ? query.OrderByDescending(p => p.Id) : query.OrderBy(p => p.Id),
        };
    }

    public async Task<Result<ProductReadResponse>> GetByIdAsync(int id,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var product = await _context.Products.Where(e => e.Id == id)
                .Include(e => e.Inventories)
                .ThenInclude(e => e.Location)
                .Select(product => new ProductReadResponse
                {
                Id = product.Id,
                SKU = product.Sku,
                Name = product.Name,
                Description = product.Description,
                CategoryId = product.CategoryId,
                CategoryName = product.Category.Name,
                UnitOfMeasureId = product.UnitOfMeasureId,
                UnitOfMeasureName = product.UnitOfMeasure.Name,
                CostPrice = product.Cost,
                UnitPrice = product.UnitPrice,
                IsActive = product.IsActive,
                Inventories = product.Inventories.Select(inventory => new
                {
                    LocationId = inventory.LocationId,
                    LocationName = inventory.Location.Name,
                    QuantityOnHand = inventory.QuantityOnHand,
                    ReorderLevel = inventory.ReorderLevel,
                    MaxLevel = inventory.MaxLevel
                }),

                CreatedAt = product.CreatedAt,
                CreatedByUserId = product.CreatedByUserId,
                CreatedByUserName = product.CreatedByUser.UserName,

                UpdatedAt = product.UpdatedAt,
                UpdatedByUserId = product.UpdatedByUserId,
                UpdatedByUserName = product.UpdatedByUser != null
                    ? product.UpdatedByUser.UserName
                    : null,

                IsDeleted = product.IsDeleted,
                DeleteAt = product.DeletedAt,
                DeletedByUserId = product.DeletedByUserId,
                DeletedByUserName = product.DeletedByUser != null
                    ? product.DeletedByUser.UserName
                    : null,
                })
                .AsNoTracking()
                .FirstOrDefaultAsync(cancellationToken);

           
           if (product is null)
            {
                return Result<ProductReadResponse>.Failure(Error.NotFound($"Product with id {id}"));
            }
            return Result<ProductReadResponse>.Success(product);  
        }
        catch(Exception ex)
        {
            return Result<ProductReadResponse>.Failure(Error.Exception(nameof(GetByIdAsync), ex));
        }
    }

}

