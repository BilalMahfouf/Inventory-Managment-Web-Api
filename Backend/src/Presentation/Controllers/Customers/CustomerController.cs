using Application.Abstractions.Queries;
using Application.Customers;
using Application.Customers.Dtos;
using Application.DTOs.Customers;
using Application.PagedLists;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using Presentation.Extensions;

namespace Presentation.Controllers.Customers;

[ApiController]
[Route("api/customers")]

[Authorize]
public class CustomerController : ControllerBase
{
    private readonly ICustomerQueries _query;
    private readonly CustomerService _service;

    public CustomerController(ICustomerQueries query, CustomerService service)
    {
        _query = query;
        _service = service;
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

    [HttpGet("{id:int}",Name = nameof(GetCustomerByIdAsync))]
    public async Task<ActionResult<CustomerReadResponse>> GetCustomerByIdAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var response = await _query.GetByIdAsync(id, cancellationToken);
        return response.HandleResult();
    }

    [HttpPost]

    public async Task<ActionResult<CustomerReadResponse>> AddCustomerAsync(
        [FromBody] CustomerCreateRequest request,
        CancellationToken cancellationToken = default)
    {
        var response = await _service.AddAsync(request, cancellationToken);
        return response.HandleResult(nameof(GetCustomerByIdAsync), new
        {
            id = response.Value is null ? 0 : response.Value.Id
        });
    }

    [HttpPut("{id:int}")]

    public async Task<ActionResult<int>> UpdateCustomerAsync(
        int id,
        [FromBody] UpdateCustomerRequest request,
        CancellationToken cancellationToken = default)
    {
        var response = await _service.UpdateAsync(id,request, cancellationToken);
        return response.HandleResult();
    }

    [HttpDelete("{id:int}")]

    public async Task<ActionResult> DeleteCustomerAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        var response = await _service.SoftDeleteAsync(id, cancellationToken);
        return response.HandleResult();
    }
}
