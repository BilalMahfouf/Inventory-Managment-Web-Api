using Application.DTOs.Inventories;
using Application.PagedLists;
using Application.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Abstractions.Queries
{
    public interface  IInventoryQueries
    {
        Task<Result<PagedList<InventoryTableResponse>>> GetInventoryTableAsync(
            int pageNumber,
            int pageSize,
            string? search,
            CancellationToken cancellationToken = default);
    }
}
