using Application.DTOs.Inventories;
using Application.DTOs.StockMovements.Response;
using Application.Inventories;
using Application.PagedLists;
using Application.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Abstractions.Queries;

public interface  IInventoryQueries
{
    Task<Result<PagedList<InventoryTableResponse>>> GetInventoryTableAsync(
        TableRequest request,
        CancellationToken cancellationToken = default);
    Task<Result<object>> GetInventorySummaryAsync(CancellationToken cancellationToken = default);
    Task<Result<object>> GetByIdAsync(int id,
        CancellationToken cancellationToken = default);
    Task<Result<PagedList<StockTransfersReadResponse>>> GetStockTransfersAsync(
        TableRequest request,
        CancellationToken cancellationToken = default);
    Task<Result<LowStockNotificationDetails>> GetLowStockMessageDetailsAsync(
        int productId,
        int locationId,
        CancellationToken cancellationToken = default);
}
