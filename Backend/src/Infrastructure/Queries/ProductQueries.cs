using Application.Abstractions.Queries;
using Application.DTOs.Products.Response.Products;
using Application.PagedLists;
using Application.Results;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Asn1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
        public async Task<Result<PagedList<ProductTableResponse>>> GetAllAsync(
            TableRequest request, CancellationToken cancellationToken = default)
        {
            try
            {

                var query = from p in _context.Products
                            join i in _context.Inventories on p.Id equals i.ProductId
                            join pc in _context.ProductCategories on p.CategoryId equals pc.Id
                            select new ProductTableResponse
                            {
                                Id = p.Id,
                                SKU = p.Sku,
                                Product = p.Name,
                                Stock = i.QuantityOnHand,
                                CategoryId = p.CategoryId,
                                Category = pc.Name,
                                Price = p.UnitPrice,
                                Cost = p.Cost,
                                IsActive = p.IsActive,
                                CreatedAt = p.CreatedAt
                            };
                if (!string.IsNullOrWhiteSpace(request.search))
                {
                    query = query.Where(e => e.SKU.ToLower().Contains(request.search)
                    || e.Product.ToLower().Contains(request.search));
                }
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
                if(request.SortOrder?.ToLower()=="desc")
                {
                    query = query.OrderByDescending(orderSelector);
                }
                else
                {
                    query = query.OrderBy(orderSelector);
                }

                    query = query.Skip((request.Page - 1) * request.PageSize)
                        .Take(request.PageSize);

                var item = await  query.ToListAsync(cancellationToken);
                    if (item is null || !item.Any())
                {
                    return Result<PagedList<ProductTableResponse>>.NotFound("Products");
                }

                var count = await _context.Products.CountAsync(cancellationToken);

                var result = new PagedList<ProductTableResponse>
                {
                    Item = item,
                    Page = request.Page,
                    PageSize = item.Count,
                    TotalCount = count
                };
                return Result<PagedList<ProductTableResponse>>.Success(result);
            }
            catch (Exception ex)
            {
                return Result<PagedList<ProductTableResponse>>.Exception(
                    nameof(GetAllAsync), ex);
            }
        }
    }
}
