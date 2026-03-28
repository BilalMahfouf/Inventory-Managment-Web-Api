using Application.Inventories.DTOs;
using Application.Products.DTOs.Request.Products;
using Application.Products.DTOs.Response.Products;
using Application.Shared.Paging;
using Domain.Shared.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Products.Contracts
{
    public interface IProductService
    {
        
        Task<Result<PagedList<ProductReadResponse>>> GetAllAsync
            (int page,
             int pageSize,
             string? search,
             CancellationToken cancellationToken = default);

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

        Task<Result<IReadOnlyCollection<ProductSuppliersReadResponse>>> FindProductSuppliersAsync
            (int productId, CancellationToken cancellationToken = default);
        Task<Result<IReadOnlyCollection<ProductsLowStockReadResponse>>>
            GetProductsWithLowStockAsync(CancellationToken cancellationToken = default);
        Task<Result<IReadOnlyCollection<InventoryBaseReadResponse>>>
            FindProductInInventoryAsync(int id
            , CancellationToken cancellationToken = default);

    }
}
