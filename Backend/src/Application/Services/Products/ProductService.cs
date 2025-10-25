using Application.Abstractions.Queries;
using Application.Abstractions.Repositories.Base;
using Application.Abstractions.Repositories.Products;
using Application.Abstractions.Services.Products;
using Application.Abstractions.Services.User;
using Application.Abstractions.UnitOfWork;
using Application.DTOs.Inventories;
using Application.DTOs.Products.Request.Products;
using Application.DTOs.Products.Response.Products;
using Application.FluentValidations.Products;
using Application.PagedLists;
using Application.Results;
using Application.Services.Shared;
using Domain.Entities;
using Domain.Entities.Products;
using Domain.Enums;
using Domain.Exceptions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Application.Services.Products;

public class ProductService : DeleteService<Product>, IProductService
{

    private readonly ProductValidatorContainer _validatorContainer;
    private readonly IProductRepository _productRepository;
    private readonly IProductQueries _query;
    // flaws of inheriting from DeleteService
    public ProductService(IProductRepository repository
        , ICurrentUserService currentUserService
        , IUnitOfWork uow
        , ProductValidatorContainer validatorContainer,
IProductQueries query)
        : base(repository, currentUserService, uow)
    {
        _validatorContainer = validatorContainer;
        _productRepository = repository;
        _query = query;
    }

    private ProductReadResponse MapToReadResponse(Product product)
    {
        if (product.Inventories is null || !product.Inventories.Any())
        {
            return new ProductReadResponse
            {
                Id = product.Id,
                SKU = product.Sku,
                Name = product.Name,
                Description = product.Description,
                CategoryId = product.CategoryId,
                CategoryName = product.Category.Name,
                UnitOfMeasureId = product.UnitOfMeasureId,
                UnitOfMeasureName = product.UnitOfMeasure.Name,
                CostPrice = product.Cost,
                UnitPrice = product.UnitPrice,
                IsActive = product.IsActive,

                CreatedAt = product.CreatedAt,
                CreatedByUserId = product.CreatedByUserId,
                CreatedByUserName = product.CreatedByUser?.UserName,

                UpdatedAt = product.UpdatedAt,
                UpdatedByUserId = product.UpdatedByUserId,
                UpdatedByUserName = product.UpdatedByUser?.UserName,

                IsDeleted = product.IsDeleted,
                DeleteAt = product.DeletedAt,
                DeletedByUserId = product.DeletedByUserId,
                DeletedByUserName = product.DeletedByUser?.UserName,

            };
        }
        return new ProductReadResponse

        {
            Id = product.Id,
            SKU = product.Sku,
            Name = product.Name,
            Description = product.Description,
            CategoryId = product.CategoryId,
            CategoryName = product.Category.Name,
            UnitOfMeasureId = product.UnitOfMeasureId,
            UnitOfMeasureName = product.UnitOfMeasure.Name,
            CostPrice = product.Cost,
            UnitPrice = product.UnitPrice,
            IsActive = product.IsActive,
            Inventories = product.Inventories.Select(e => new
            {
                LocationId = e.LocationId,

            }),

            CreatedAt = product.CreatedAt,
            CreatedByUserId = product.CreatedByUserId,
            CreatedByUserName = product.CreatedByUser?.UserName,

            UpdatedAt = product.UpdatedAt,
            UpdatedByUserId = product.UpdatedByUserId,
            UpdatedByUserName = product.UpdatedByUser?.UserName,

            IsDeleted = product.IsDeleted,
            DeleteAt = product.DeletedAt,
            DeletedByUserId = product.DeletedByUserId,
            DeletedByUserName = product.DeletedByUser?.UserName,


        };
    }

    private async Task<Result> _UpdateProductStatus(int id
        , bool isActive, CancellationToken cancellationToken)
    {
        if (id <= 0)
        {
            return Result.InvalidId();
        }
        try
        {
            var product = await _productRepository.FindAsync(e => e.Id == id
             , cancellationToken);
            if (product is null)
            {
                return Result.NotFound("Product");
            }
            if (product.IsActive == isActive)
            {
                string status = isActive ? "active" : "inactive";
                return Result.Failure($"Product is already {status}"
                    , ErrorType.Conflict);
            }
            product.IsActive = isActive;
            product.UpdatedAt = DateTime.UtcNow;
            product.UpdatedByUserId = _currentUserService.UserId;
            _productRepository.Update(product);
            await _uow.SaveChangesAsync(cancellationToken);
            return Result.Success;

        }
        catch (Exception ex)
        {
            return Result.Exception(nameof(_UpdateProductStatus), ex);
        }
    }
    public async Task<Result> ActivateAsync(int id
        , CancellationToken cancellationToken = default)
    {
        return await _UpdateProductStatus(id, true, cancellationToken);
    }

    public async Task<Result<ProductReadResponse>> CreateAsync(
        ProductCreateRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = _validatorContainer.CreateValidator.Validate(request);
            if (!result.IsValid)
            {
                var errorMessage = string.Join(";"
                    , result.Errors.Select(e => e.ErrorMessage));
                return Result<ProductReadResponse>.Failure(errorMessage
                    , ErrorType.BadRequest);
            }
            // this is business rules 
            var existingProduct = await _productRepository.IsExistAsync
                (p => p.Sku == request.SKU, cancellationToken);
            if (existingProduct)
            {
                return Result<ProductReadResponse>.Failure("SKU already exists"
                    , ErrorType.Conflict);
            }
            var productResult = Product.Create(
                sku: request.SKU,
                name: request.Name,
                description: request.Description,
                categoryId: request.CategoryId,
                unitOfMeasureId: request.UnitOfMeasureId,
                unitPrice: request.UnitPrice,
                cost: request.CostPrice
                );
            if (!productResult.IsSuccess)
            {
                return Result<ProductReadResponse>
                    .Failure(productResult.ErrorMessage!
                    , productResult.ErrorType);
            }
            var inventory = Inventory.Create(
                product: productResult.Value!,
                locationId: request.LocationId,
                quantityOnHand: request.QuantityOnHand,
                reorderLevel: request.ReorderLevel,
                maxLevel: request.MaxLevel
                );

           _uow.Products.Add(productResult.Value!);
            _uow.Inventories.Add(inventory);
            await _uow.SaveChangesAsync(cancellationToken);
            return await _query.GetByIdAsync(
                productResult.Value!.Id,
                cancellationToken);
        }
        catch(DomainException dex)
        {
            return Result<ProductReadResponse>.Failure(dex.Message
                , ErrorType.Conflict);
        }
        catch (Exception ex)
        {
            return Result<ProductReadResponse>.Exception(nameof(CreateAsync), ex);

        }
    }

    public Task<Result> DeactivateAsync(int id, CancellationToken cancellationToken = default)
    {
        return _UpdateProductStatus(id, false, cancellationToken);
    }

    public async Task<Result> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        return await SoftDeleteAsync(id, cancellationToken);
    }
    public async Task<Result<PagedList<ProductReadResponse>>> GetAllAsync(
      int page,
      int pageSize,
      string? search,
      CancellationToken cancellationToken = default)
    {
        try
        {
            var products = await _productRepository.GetAllWithPaginationAsync(
                page: page,
                pageSize: pageSize,
                cancellationToken: cancellationToken,
                includeProperties: @"Category,UnitOfMeasure,CreatedByUser,UpdatedByUser,DeletedByUser"
            );
            var response = products
                .Select(p => MapToReadResponse(p)).ToList().AsReadOnly();

            var totalCount = await _productRepository.GetCountAsync(
                filter: p => !p.IsDeleted, cancellationToken: cancellationToken);
            var result = new PagedList<ProductReadResponse>()
            {
                Item = response,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };

            return Result<PagedList<ProductReadResponse>>
                .Success(result);
        }
        catch (Exception ex)
        {
            return Result<PagedList<ProductReadResponse>>
                .Exception(nameof(GetAllAsync), ex);
        }
    }

    // to do refactor to return from query and make update the inventory also
    public async Task<Result<ProductReadResponse>> UpdateAsync(
        int id,
        ProductUpdateRequest request,
        CancellationToken cancellationToken = default)
    {
        if (id <= 0)
        {
            return Result<ProductReadResponse>.InvalidId();
        }
        try
        {
            var result = _validatorContainer.UpdateValidator.Validate(request);
            if (!result.IsValid)
            {
                var errorMessage = string.Join(";"
                    , result.Errors.Select(e => e.ErrorMessage));

                return Result<ProductReadResponse>.Failure(errorMessage
                    , ErrorType.BadRequest);
            }

            var product = await _productRepository.FindAsync(p => p.Id == id
            && (!p.IsDeleted), cancellationToken);
            if (product is null)
            {
                return Result<ProductReadResponse>.NotFound("Product");
            }
            var inventory = await _uow.Inventories.FindAsync(
                e => e.ProductId == id && e.LocationId == request.LocationId,
                cancellationToken);
            if (inventory is null)
            {
                return Result<ProductReadResponse>.NotFound(nameof(inventory));
            }
            var productResult = product.Update(
                request.Name,
                request.Description,
                request.CategoryId,
                request.UnitPrice,
                request.CostPrice);
            if (!productResult.IsSuccess)
            {
                return Result<ProductReadResponse>.Failure(productResult.ErrorMessage!
                    , productResult.ErrorType);
            }
             inventory.UpdateInventoryLevels(
                 request.QuantityOnHand,
                 request.ReorderLevel,
                 request.MaxLevel);

            _productRepository.Update(product);
            _uow.Inventories.Update(inventory);

            await _uow.SaveChangesAsync(cancellationToken);
            return await _query.GetByIdAsync(id, cancellationToken);

        }
        catch(DomainException ex)
        {
            return Result<ProductReadResponse>.Failure(ex.Message
                , ErrorType.Conflict);
        }
        catch (Exception ex)
        {
            return Result<ProductReadResponse>.Exception(nameof(UpdateAsync), ex);
        }
    }

    public async Task<Result<IReadOnlyCollection<ProductSuppliersReadResponse>>>
        FindProductSuppliersAsync(int productId, CancellationToken cancellationToken)
    {
        if (productId <= 0)
        {
            return Result<IReadOnlyCollection<ProductSuppliersReadResponse>>
                .InvalidId();
        }
        try
        {
            var productSuppliers = await _uow.ProductSuppliers.
            GetAllAsync(e => e.ProductId == productId, cancellationToken
            , "Product,Supplier");
            if (productSuppliers is null || !productSuppliers.Any())
            {
                return Result<IReadOnlyCollection<ProductSuppliersReadResponse>>
                    .NotFound("Product Suppliers");
            }
            var response = productSuppliers.Select(
                ps => new ProductSuppliersReadResponse
                {
                    Id = ps.Id,
                    ProductId = ps.ProductId,
                    ProductName = ps.Product.Name,
                    SupplierId = ps.SupplierId,
                    SupplierName = ps.Supplier.Name,
                    SupplierProductCode = ps.SupplierProductCode,
                    LeadTimeDay = ps.LeadTimeDays,
                    MinOrderQuantity = ps.MinOrderQuantity
                }).ToList().AsReadOnly();
            return Result<IReadOnlyCollection<ProductSuppliersReadResponse>>
                .Success(response);
        }
        catch (Exception ex)
        {
            return Result<IReadOnlyCollection<ProductSuppliersReadResponse>>
                .Exception(nameof(FindProductSuppliersAsync), ex);
        }
    }

    public async Task<Result<IReadOnlyCollection<ProductsLowStockReadResponse>>>
        GetProductsWithLowStockAsync(CancellationToken cancellationToken)
    {
        try
        {

            var productsWithLowStock = await _uow.Inventories.GetAllAsync(
                e => e.QuantityOnHand <= e.ReorderLevel, cancellationToken
                , "Product,Location");
            if (productsWithLowStock is null || !productsWithLowStock.Any())
            {
                return Result<IReadOnlyCollection<ProductsLowStockReadResponse>>
                    .NotFound("Products with low stock");
            }
            var response = productsWithLowStock.Select(p => new ProductsLowStockReadResponse
            {
                ProductId = p.ProductId,
                ProductName = p.Product.Name,
                LocationId = p.LocationId,
                LocationName = p.Location.Name,
                QuantityOnHand = p.QuantityOnHand,
                ReorderLevel = p.ReorderLevel,
            }).ToList().AsReadOnly();
            return Result<IReadOnlyCollection<ProductsLowStockReadResponse>>
                .Success(response);
        }
        catch (Exception ex)
        {
            return Result<IReadOnlyCollection<ProductsLowStockReadResponse>>
                .Exception(nameof(GetProductsWithLowStockAsync), ex);
        }
    }

    public async Task<Result<IReadOnlyCollection<InventoryBaseReadResponse>>>
        FindProductInInventoryAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        if (id <= 0)
        {
            return Result<IReadOnlyCollection<InventoryBaseReadResponse>>
                .InvalidId();
        }
        try
        {
            var inventories = await _uow.Inventories.GetAllAsync(
                e => e.ProductId == id, cancellationToken
                , "Product,Location");
            if (inventories is null || !inventories.Any())
            {
                return Result<IReadOnlyCollection<InventoryBaseReadResponse>>
                    .NotFound("Inventories for the product");
            }
            var response = inventories.Select(i => new InventoryBaseReadResponse
            {
                Id = i.Id,
                ProductId = i.ProductId,
                ProductName = i.Product.Name,
                LocationId = i.LocationId,
                LocationName = i.Location.Name,
                QuantityOnHand = i.QuantityOnHand,
                ReorderLevel = i.ReorderLevel,
                MaxLevel = i.MaxLevel
            }).ToList().AsReadOnly();
            return Result<IReadOnlyCollection<InventoryBaseReadResponse>>
                .Success(response);
        }
        catch (Exception ex)
        {
            return Result<IReadOnlyCollection<InventoryBaseReadResponse>>
                .Exception(nameof(FindProductInInventoryAsync), ex);
        }
    }
}