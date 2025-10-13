using Application.DTOs.Products.Response.Categories;
using Application.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Abstractions.Queries;

public interface IProductCategoryQueries
{
    Task<Result<ProductCategoryDetailsResponse>> GetCategoryByIdAsync(int id, CancellationToken cancellationToken);
    Task<Result<IEnumerable<object>>> GetMainCategoriesAsync(
        CancellationToken cancellationToken);
}
