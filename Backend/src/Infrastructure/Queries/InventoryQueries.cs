using Application.Abstractions.Queries;
using Application.DTOs.Inventories;
using Application.PagedLists;
using Application.Results;
using Infrastructure.Persistence;
using MailKit.Search;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
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

        var query =
          from i in _context.Inventories
          join p in _context.Products on i.ProductId equals p.Id
          join l in _context.Locations on i.LocationId equals l.Id
          select new InventoryTableResponse
          {
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

        // ✅ Apply search filter (index-friendly: prefix match)
        if (!string.IsNullOrWhiteSpace(request.search))
        {
            string searchPattern = $"{request.search}%"; 
            query = query.Where(x =>
                EF.Functions.Like(x.Sku, searchPattern) ||
                EF.Functions.Like(x.Product, searchPattern));
        }

        Func<InventoryTableResponse, object> orderBy = request.SortColumn?.ToLower() switch
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
        if(request.SortOrder?.ToLower() == "desc")
        {
            query = query.OrderByDescending(orderBy).AsQueryable();
        }
        else
        {
            query = query.OrderBy(orderBy).AsQueryable();
        }
        query = query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize);
        var table = await query.ToListAsync(cancellationToken);



    }
}
