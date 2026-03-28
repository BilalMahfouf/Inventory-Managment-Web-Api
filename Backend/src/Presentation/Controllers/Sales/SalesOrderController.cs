using Application.Shared.Paging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Extensions;

namespace Presentation.Controllers.Sales;

[ApiController]
[Route("api/sales-orders")]

[Authorize]
public class SalesOrderController : ControllerBase
{
    private readonly SalesOrderService _salesOrderService;
    private readonly ISalesOrderQueries _query;

    public SalesOrderController(SalesOrderService salesOrderService, ISalesOrderQueries query)
    {
        _salesOrderService = salesOrderService;
        _query = query;
    }


    [HttpPost]
    public async Task<ActionResult<int>> PlaceOrderAsync(
    [FromBody] CreateSalesOrderRequest request,
    CancellationToken cancellationToken = default)
    {
        var response = await _salesOrderService.CreateSalesOrderAsync(
            request,
            cancellationToken);
        return response.HandleResult(nameof(GetOrderByIdAsync), new
        {
            id = response.Value
        });
    }


    [HttpGet]

    public async Task<ActionResult<PagedList<SalesOrderTableResponse>>>
        GetAllOrdersAsync(
        [FromQuery] int? page,
        [FromQuery] int? pageSize,
        [FromQuery] string? search,
        [FromQuery] string? sortColumn,
        [FromQuery] string? sortOrder,
        CancellationToken cancellationToken = default)
    {
        var request = TableRequest.Create(
            pageSize,
            page,
            search,
            sortColumn,
            sortOrder);

        var response = await _query.GetOrdersTableAsync(
            request, cancellationToken);
        return response.HandleResult();
    }

    [HttpGet("{id:int}", Name = nameof(GetOrderByIdAsync))]

    public async Task<ActionResult<SalesOrderReadResponse>>
        GetOrderByIdAsync(
        [FromRoute] int id,
        CancellationToken cancellationToken = default)
    {
        var response = await _query.GetSalesOrderByIdAsync(
            id, cancellationToken);
        return response.HandleResult();
    }

    [HttpPost("{id:int}/complete")]

    public async Task<IActionResult> CompleteOrderAsync(
        [FromRoute] int id,
        CancellationToken cancellationToken = default)
    {
        var response = await _salesOrderService.CompleteOrderAsync(
            id, cancellationToken);
        return response.HandleResult();
    }

    [HttpGet("summary")]

    public async Task<ActionResult<object>> GetOrdersSummaryAsync(
        CancellationToken cancellationToken = default)
    {
        var response = await _query.GetDahsboardSummaryAsync(cancellationToken);
        return response.HandleResult();
    }


}
