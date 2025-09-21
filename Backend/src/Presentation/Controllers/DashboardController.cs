﻿using Application.Services.Shared;
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

        [HttpGet("inventory-alerts")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<ActionResult<IEnumerable<object>>> GetInventoryAlertsAsync(
            CancellationToken cancellationToken = default)
        {
            var result = await _dashboardService.GetInventoryAlertsAsync
                (cancellationToken);
            return result.HandleResult();
        }

        [HttpGet("top-selling-products")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<ActionResult<IEnumerable<object>>> GetTopSellingProductsAsync(
           [FromQuery] int numberOfProducts = 5,
            CancellationToken cancellationToken = default)
        {
            var result = await _dashboardService.GetTopSellingProductsAsync(
                numberOfProducts, cancellationToken);
            return result.HandleResult();
        }
        


    }
}
