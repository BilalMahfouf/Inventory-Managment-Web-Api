using Application.DTOs.Products.Request.Products;
using Application.DTOs.Products.Response.Products;
using Application.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Abstractions.Services.Products
{
    public interface IProductService
    {
        Task<Result<ProductReadResponse>> FindAsync(int id
            ,CancellationToken cancellationToken=default);
        Task<Result<IReadOnlyCollection<ProductReadResponse>>> GetAllAsync
            (CancellationToken cancellationToken = default);

        Task<Result<ProductReadResponse>> CreateAsync(ProductCreateRequest request
            , CancellationToken cancellationToken = default);
        Task<Result<ProductReadResponse>> UpdateAsync(int id
            , ProductUpdateRequest request
            , CancellationToken cancellationToken = default);
        Task<Result> DeleteAsync(int id,CancellationToken cancellationToken=default);
        Task<Result> ActivateAsync(int id
            , CancellationToken cancellationToken = default);
        Task<Result> DeactivateAsync(int id
            , CancellationToken cancellationToken = default);
    }
}
