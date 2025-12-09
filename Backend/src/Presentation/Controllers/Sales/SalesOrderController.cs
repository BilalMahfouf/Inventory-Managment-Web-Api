using Application.Sales;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Extensions;

namespace Presentation.Controllers.Sales;

[ApiController]
[Route("api/sales-orders")]

[Authorize]
public class SalesOrderController : ControllerBase
{
    private readonly ISalesOrderQueries _salesOrderQueries;

    public SalesOrderController(ISalesOrderQueries salesOrderQueries)
    {
        _salesOrderQueries = salesOrderQueries;
    }


    [HttpGet]

    public async Task<ActionResult<object>> GetDashboardSummaryAsync(
        CancellationToken cancellationToken = default)
    {
        var response = await _salesOrderQueries
            .GetDashboardSumamryAsync(cancellationToken);
        return response.HandleResult();
    }
}
