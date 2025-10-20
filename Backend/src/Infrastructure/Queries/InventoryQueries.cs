using Application.Abstractions.Queries;
using Application.DTOs.Inventories;
using Application.PagedLists;
using Application.Results;
using Domain.Entities;
using Infrastructure.Persistence;
using MailKit.Search;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Queries;

public class InventoryQueries : IInventoryQueries
{
    private readonly InventoryManagmentDBContext _context;

    public InventoryQueries(InventoryManagmentDBContext context)
    {
        _context = context;
    }

    public async Task<Result<PagedList<InventoryTableResponse>>>
        GetInventoryTableAsync
        (TableRequest request,
        CancellationToken cancellationToken = default)
    {

        try
        {

            // to do add Count for total count
            var count = await _context.Inventories.CountAsync(cancellationToken);
            if (count is 0)
            {
                return Result<PagedList<InventoryTableResponse>>
                    .NotFound("Inventory Table ");
            }
            var query =
              from i in _context.Inventories
              join p in _context.Products on i.ProductId equals p.Id
              join l in _context.Locations on i.LocationId equals l.Id
              select new InventoryTableResponse
              {
                  Id = i.Id,
                  Sku = p.Sku,
                  Product = p.Name,
                  Location = l.Name,
                  Quantity = i.QuantityOnHand,
                  Reorder = i.ReorderLevel,
                  Max = i.MaxLevel,
                  Status = i.QuantityOnHand == 0
                      ? "Out Of Stock"
                      : i.QuantityOnHand <= i.ReorderLevel
                          ? "Low Stock"
                          : "In Stock",
                  StockPercentage = Math.Round((double)((i.QuantityOnHand * 100) / i.MaxLevel), 0),
                  PotentialProfit = (p.UnitPrice - p.Cost) * i.QuantityOnHand
              };

            if (!string.IsNullOrWhiteSpace(request.search))
            {
                string searchPattern = $"{request.search}%";
                query = query.Where(x =>
                    EF.Functions.Like(x.Sku, searchPattern) ||
                    EF.Functions.Like(x.Product, searchPattern) ||
                    EF.Functions.Like(x.Location, searchPattern));
            }

            Expression<Func<InventoryTableResponse, object>> orderBy =
                    request.SortColumn?.ToLower() switch
                    {
                        "sku" => x => x.Sku,
                        "product" => x => x.Product,
                        "location" => x => x.Location,
                        "quantity" => x => x.Quantity,
                        "reorder" => x => x.Reorder,
                        "max" => x => x.Max,
                        "status" => x => x.Status,
                        "stockpercentage" => x => x.StockPercentage,
                        "potentialprofit" => x => x.PotentialProfit,
                        _ => x => x.Product // Default sorting by Product
                    };
            if (request.SortOrder?.ToLower() == "desc")
            {
                query = query.OrderByDescending(orderBy);
            }
            else
            {
                query = query.OrderBy(orderBy);
            }
            query = query
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize);
            var table = await query.ToListAsync(cancellationToken);

            if (table is null || !table.Any())
            {
                return Result<PagedList<InventoryTableResponse>>.NotFound("Inventory Table ");
            }
            var result = new PagedList<InventoryTableResponse>
            {
                Item = table,
                TotalCount = count,
                PageSize = request.PageSize,
                Page = request.Page
            };
            return Result<PagedList<InventoryTableResponse>>.Success(result);
        }
        catch (Exception ex)
        {
            return Result<PagedList<InventoryTableResponse>>
                .Exception(nameof(GetInventoryTableAsync), ex);
        }
    }
    public async Task<Result<object>> GetInventorySummaryAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var lowStockItems = await _context.Inventories
                .CountAsync(e => e.QuantityOnHand <= e.ReorderLevel);
            var outOfStockItems = await _context.Inventories
                .CountAsync(e => e.QuantityOnHand == 0);
            var totalInventoryItems = await _context.Inventories.CountAsync(cancellationToken);
            var totalPotentialProfit = await (from i in _context.Inventories
                                              join p in _context.Products on i.ProductId equals p.Id
                                              select (p.UnitPrice - p.Cost) * i.QuantityOnHand).SumAsync(cancellationToken);
            var summary = new
            {
                LowStockItems = lowStockItems,
                OutOfStockItems = outOfStockItems,
                TotalInventoryItems = totalInventoryItems,
                TotalPotentialProfit = totalPotentialProfit
            };
            return Result<object>.Success(summary);
        }
        catch (Exception ex)
        {
            return Result<object>.Exception(nameof(GetInventorySummaryAsync), ex);
        }
    }

    public async Task<Result<object>> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        if (id <= 0)
        {
            return Result<object>.InvalidId();
        }
        try
        {
            var inventory = await _context.Inventories
                .Where(e => e.Id == id)
                .Select(e => new
                {
                    Id = e.Id,
                    Product = new
                    {
                        Id = e.ProductId,
                        Sku=e.Product.Sku,
                        Name = e.Product.Name,
                        CategoryName = e.Product.Category.Name,
                        UnitOfMeasureName = e.Product.UnitOfMeasure.Name
                    },
                    Location = new
                    {
                        Id = e.LocationId,
                        Name = e.Location.Name,
                        Address = e.Location.Address,
                        Type = e.Location.LocationType.Name
                    },
                    QuantityOnHand = e.QuantityOnHand,
                    ReorderLevel = e.ReorderLevel,
                    MaxLevel = e.MaxLevel,

                }).FirstOrDefaultAsync(cancellationToken);
            if (inventory is null)
            {
                return Result<object>.NotFound(nameof(inventory));
            }
            return Result<object>.Success(inventory);
        }
        catch (Exception ex)
        {
            return Result<object>.Exception(nameof(GetByIdAsync), ex);
        }
    }


}
