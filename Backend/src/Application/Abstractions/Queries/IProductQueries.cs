using Application.DTOs.Products.Response.Products;
using Application.PagedLists;
using Application.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Abstractions.Queries
{
    public interface IProductQueries
    {
        Task<Result<object>> GetProductDashboardSummaryAsync(
            CancellationToken cancellationToken = default);
        Task<Result<PagedList<ProductTableResponse>>> GetAllAsync(TableRequest request,
            CancellationToken cancellationToken = default);
        Task<Result<PagedList<StockMovementsHistoryTableResponse>>> GetStockMovementsHistoryAsync(
            TableRequest request,
            CancellationToken cancellationToken = default);
        Task<Result<ProductReadResponse>> GetByIdAsync(int id,
            CancellationToken cancellationToken = default);
    }
}
