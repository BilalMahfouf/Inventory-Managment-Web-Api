using Application.Abstractions.Queries;
using Application.Results;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Queries;

internal class TransferQueries : ITransferQueries
{
    private readonly InventoryManagmentDBContext _context;

    public TransferQueries(InventoryManagmentDBContext context)
    {
        _context = context;
    }

    public async Task<Result<object>> GetByIdAsync
        (int id,
        CancellationToken cancellationToken = default)
    {
        if (id <= 0)
        {
            return Result<object>.InvalidId();
        }
        try
        {
            var transfer = await _context.StockTransfers
                .Select(e => new
                {
                    Id = e.Id,
                    FromLocationId = e.FromLocationId,
                    FromLocationName = e.FromLocation.Name,

                    ToLocationId = e.ToLocationId,
                    ToLocationName = e.ToLocation.Name,

                    ProdcutId = e.ProductId,
                    ProductName = e.Product.Name,

                    Status = e.TransferStatus.ToString(),
                    Quantity = e.Quantity,

                    CreatedAt = e.CreatedAt,
                    CreatedByUserId = e.CreatedByUserId,
                    CreatedByUserName = e.CreatedByUser.UserName,
                }).FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
            if (transfer is null)
            {
                return Result<object>.NotFound($"Transfer with id {id}");
            }
            return Result<object>.Success(transfer);
        }
        catch (Exception ex)
        {
            return Result<object>.Exception(nameof(GetByIdAsync), ex);
        }
    }
}
