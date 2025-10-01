using Application.Abstractions.Queries;
using Application.DTOs.Products.Response.Products;
using Application.PagedLists;
using Application.Results;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Asn1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Queries
{
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
                  .Where(i => i.QuantityOnHand <= i.ReorderLevel)
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
                if(result is null)
                {
                    return Result<object>.NotFound("no summary data is found");
                }
                return Result<object>.Success(result);
            }
            catch(Exception ex)
            {
                return Result<object>.Exception(nameof(GetProductDashboardSummaryAsync), ex) ;
            }
        }
        public async Task<Result<PagedList<ProductReadResponse>>> GetAllAsync(
            TableRequest request, CancellationToken cancellationToken = default)
        {
            try
            {
                var query = _context.Products.AsQueryable();

                if (!string.IsNullOrWhiteSpace(request.search))
                {
                    query = query.Where(e => e.Sku.ToLower().Contains(request.search)
                    || e.Name.ToLower().Contains(request.search));
                }
                query = query.Skip((request.Page - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .OrderBy(e=>e.Id)
                    .Include(e => e.CreatedByUser)
                    .Include(e => e.UpdatedByUser)
                    .Include(e => e.DeletedByUser)
                    .Include(e => e.UnitOfMeasure)
                    .Include(e => e.Category);
                var item = await query.Select(e => 
                Application.Helpers.Util.Utility.MapToReadResponse(e))
                    .ToListAsync(cancellationToken);
                if (item is null || !item.Any())
                {
                    return Result<PagedList<ProductReadResponse>>.NotFound("Products");
                }

                var count = await _context.Products.CountAsync(cancellationToken);

                var result = new PagedList<ProductReadResponse>
                {
                    Item = item,
                    Page = request.Page,
                    PageSize = item.Count,
                    TotalCount = count
                };
                return Result<PagedList<ProductReadResponse>>.Success(result);
            }
            catch (Exception ex)
            {
                return Result<PagedList<ProductReadResponse>>.Exception(
                    nameof(GetAllAsync), ex);
            }
        }
    }
}
