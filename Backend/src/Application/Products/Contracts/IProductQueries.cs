using Application.Products.DTOs.Response.Products;
using Application.Shared.Paging;
using Domain.Shared.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Products.Contracts
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
