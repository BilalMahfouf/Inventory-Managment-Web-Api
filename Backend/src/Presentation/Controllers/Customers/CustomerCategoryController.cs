using Application.Customers;
using Microsoft.AspNetCore.Mvc;
using Presentation.Extensions;

namespace Presentation.Controllers.Customers;

[ApiController]
[Route("api/customer-categories")]
public class CustomerCategoryController : ControllerBase
{
    private readonly CustomerCategoryService _service;

    public CustomerCategoryController(CustomerCategoryService service)
    {
        _service = service;
    }

    [HttpGet("names")]

    public async Task<ActionResult<IEnumerable<object>>> GetCustomerCategoriesNamesAsync(
        CancellationToken cancellationToken = default)
    {
        var response = await _service.GetCategoriesNamesAsync(cancellationToken);
        return response.HandleResult();
    }
}
