using Application.DTOs.Products.Request.Categories;
using Application.DTOs.Products.Response.Categories;
using Application.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Abstractions.Services.Products
{
    public interface IProductCategoryService
    {
        Task<Result<IReadOnlyCollection<ProductCategoryReadResponse>>>
            GetAllAsync(CancellationToken cancellationToken = default);
        Task<Result<IReadOnlyCollection<ProductCategoryChildrenReadResponse>>>
            GetAllChildrenAsync(
            int id,CancellationToken cancellationToken = default);
        Task<Result<IReadOnlyCollection<ProductCategoryTreeReadResponse>>>
            GetAllTreeAsync(CancellationToken cancellationToken = default);

        Task<Result<ProductCategoryReadResponse>> FindAsync(int id
            ,CancellationToken cancellationToken = default);
        Task<Result<ProductCategoryReadResponse>> AddAsync(
            ProductCategoryRequest request, CancellationToken cancellationToken);
        Task<Result> UpdateAsync(int id, ProductCategoryRequest request
            , CancellationToken cancellationToken);
        Task<Result> DeleteAsync(int id, CancellationToken cancellationToken);
    }
}
