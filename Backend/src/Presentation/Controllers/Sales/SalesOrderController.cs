using Application.Sales.RequestResponse;
using Application.Sales.Services1;
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

    public SalesOrderController(SalesOrderService salesOrderService)
    {
        _salesOrderService = salesOrderService;
    }


    [HttpPost]
    public async Task<ActionResult<int>> PlaceOrderAsync(
    [FromBody] CreateSalesOrderRequest request,
    CancellationToken cancellationToken = default)
    {
        var response = await _salesOrderService.CreateSalesOrderAsync(
            request,
            cancellationToken);
        return response.HandleResult();
    }

}
