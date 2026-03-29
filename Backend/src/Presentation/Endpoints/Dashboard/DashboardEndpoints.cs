using Carter;
using Presentation.Extensions;

namespace Presentation.Endpoints.Dashboard;

public sealed class DashboardEndpoints : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/dashboard")
            .WithTags("Dashboard")
            .RequireAuthorization();

        group.MapGet("/summary", async (
                DashboardService dashboardService,
                CancellationToken cancellationToken = default) =>
            {
                var result = await dashboardService.GetDashboardSummaryAsync(cancellationToken);

                if (result.IsSuccess)
                {
                    return Results.Ok(result.Value);
                }

                return result.Problem();
            })
            .WithSummary("Get dashboard summary")
            .WithDescription("Returns dashboard aggregate summary data.")
            .Produces<object>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapGet("/inventory-alerts", async (
                DashboardService dashboardService,
                CancellationToken cancellationToken = default) =>
            {
                var result = await dashboardService.GetInventoryAlertsAsync(cancellationToken);

                if (result.IsSuccess)
                {
                    return Results.Ok(result.Value);
                }

                return result.Problem();
            })
            .WithSummary("Get inventory alerts")
            .WithDescription("Returns inventory alerts for dashboard consumption.")
            .Produces<IEnumerable<object>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapGet("/top-selling-products", async (
                DashboardService dashboardService,
                int numberOfProducts = 5,
                CancellationToken cancellationToken = default) =>
            {
                var result = await dashboardService.GetTopSellingProductsAsync(numberOfProducts, cancellationToken);

                if (result.IsSuccess)
                {
                    return Results.Ok(result.Value);
                }

                return result.Problem();
            })
            .WithSummary("Get top selling products")
            .WithDescription("Returns top selling products for the requested count.")
            .Produces<IEnumerable<object>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapGet("/today-performance", async (
                IDashboardQueries dashboardQueries,
                CancellationToken cancellationToken = default) =>
            {
                var result = await dashboardQueries.GetTodayPerformanceAsync(cancellationToken);

                if (result.IsSuccess)
                {
                    return Results.Ok(result.Value);
                }

                return result.Problem();
            })
            .WithSummary("Get today performance")
            .WithDescription("Returns today's performance metrics for dashboard.")
            .Produces<object>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }
}
