using Application.Abstractions.Queries;
using Application.DTOs.StockMovements.Response;
using Application.PagedLists;
using Microsoft.AspNetCore.Mvc;
using Presentation.Extensions;

namespace Presentation.Controllers.StockMovements;

[ApiController]
[Route("api/stock-transfers")]
public class StockTransferController : ControllerBase
{
    private readonly IInventoryQueries _query;

    public StockTransferController(IInventoryQueries query)
    {
        _query = query;
    }

    [HttpGet]
    public async Task<ActionResult<PagedList<StockTransfersReadResponse>>>
        GetStockTransfersAsync(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null,
        [FromQuery] string? sortColumn = null,
        [FromQuery] string? sortOrder = null,
        CancellationToken cancellationToken = default)
    {
        var request = TableRequest.Create(
            pageSize,
            page,
            search,
            sortColumn,
            sortOrder);
        var response = await _query.GetStockTransfersAsync(request, cancellationToken);
        return response.HandleResult(); 
    }

}
