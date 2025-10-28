using Application.Abstractions.Queries;
using Application.DTOs.Customers;
using Application.PagedLists;
using Microsoft.AspNetCore.Mvc;
using Presentation.Extensions;

namespace Presentation.Controllers;

[ApiController]
[Route("api/customers")]
public class CustomerController : ControllerBase
{
    private readonly ICustomerQueries _query;

    public CustomerController(ICustomerQueries query)
    {
        _query = query;
    }


    [HttpGet("summary")]

    public async Task<ActionResult<object>> GetCustomersSummaryAsync(
        CancellationToken cancellationToken = default)
    {
        var response = await _query.GetCustomerSummary(cancellationToken);
        return response.HandleResult();
    }

    [HttpGet()]

    public async Task<ActionResult<PagedList<CustomerTableReadResponse>>> GetAllCustomersAsync(
        [FromQuery] int? page,
        [FromQuery] int? pageSize,
        [FromQuery] string? search,
        [FromQuery] string? sortColumn,
        [FromQuery] string? sortOrder,
        CancellationToken cancellationToken = default)
    {
        var request = TableRequest.Create(pageSize, page, search, sortColumn, sortOrder);

        var response = await _query.GetAllAsync(request, cancellationToken);
        return response.HandleResult();
    }

    [HttpGet("{id:int}")]

    public async Task<ActionResult<object>> GetCustomerByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var response = await _query.GetByIdAsync(id, cancellationToken);
        return response.HandleResult();
    }

}
