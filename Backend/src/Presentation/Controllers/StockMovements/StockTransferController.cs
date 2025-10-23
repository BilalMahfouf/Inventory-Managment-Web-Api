using Application.Abstractions.Queries;
using Application.DTOs.StockMovements.Request;
using Application.DTOs.StockMovements.Response;
using Application.PagedLists;
using Application.Services.StockMovements;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Extensions;

namespace Presentation.Controllers.StockMovements;

[ApiController]
[Route("api/stock-transfers")]

[Authorize]
public class StockTransferController : ControllerBase
{
    private readonly IInventoryQueries _query;
    private readonly StockTransferService _service;

    public StockTransferController(
        IInventoryQueries query,
        StockTransferService service)
    {
        _query = query;
        _service = service;
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

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]

    public async Task<ActionResult<int>> TransferStockAsync(
        StockTransferRequest request,
        CancellationToken cancellationToken = default)
    {
       var response = await _service.TransferStockAsync(
           request, cancellationToken); 
        return response.HandleResult();
    }



}
