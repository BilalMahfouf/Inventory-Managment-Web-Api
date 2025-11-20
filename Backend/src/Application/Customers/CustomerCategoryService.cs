using Application.Abstractions.UnitOfWork;
using Application.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Customers;

public class CustomerCategoryService
{
    private readonly IUnitOfWork _uow;

    public CustomerCategoryService(IUnitOfWork uow)
    {
        _uow = uow;
    }


    public async Task<Result<IEnumerable<object>>> GetCategoriesNamesAsync(
        CancellationToken cancellationToken = default)
    {
        try
        {
            var categories = await _uow.CustomerCategories
                .GetAllAsync(cancellationToken: cancellationToken);
            var result = categories
                .Select(c => new
                {
                    c.Id,
                    c.Name
                })
                .ToList()
                .AsReadOnly();
            if (result is null || !result.Any())
            {
                return Result<IEnumerable<object>>.NotFound("CustomerCategories");
            }
            return Result<IEnumerable<object>>.Success(result);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<object>>.Exception(
                nameof(GetCategoriesNamesAsync),
                nameof(CustomerCategoryService),
                ex);
        }
    }
}
