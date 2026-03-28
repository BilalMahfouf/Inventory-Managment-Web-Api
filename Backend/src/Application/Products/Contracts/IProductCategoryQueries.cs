using Application.Products.DTOs.Response.Categories;
using Domain.Shared.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Products.Contracts;

public interface IProductCategoryQueries
{
    Task<Result<ProductCategoryDetailsResponse>> GetCategoryByIdAsync(int id, CancellationToken cancellationToken);
    Task<Result<IEnumerable<object>>> GetMainCategoriesAsync(
        CancellationToken cancellationToken);
}
