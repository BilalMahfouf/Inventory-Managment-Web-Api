using Application.Services.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Presentation.Extensions;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/dashboard")]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly DashboardService _dashboardService;
        public DashboardController(DashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }
        [HttpGet("summary")]
        public async Task<ActionResult<object>> GetDashboardSummaryAsync(
            CancellationToken cancellationToken = default)
        {
            var result = await _dashboardService.GetDashboardSummaryAsync(cancellationToken);
            return result.HandleResult();
        }
    }
}
