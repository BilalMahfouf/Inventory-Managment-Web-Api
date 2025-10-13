using Application.Abstractions.Queries;
using Application.Abstractions.Repositories.Base;
using Application.Abstractions.Services.Products;
using Application.Abstractions.Services.User;
using Application.Abstractions.UnitOfWork;
using Application.DTOs.Products.Request.Categories;
using Application.DTOs.Products.Response.Categories;
using Application.FluentValidations.Product;
using Application.Results;
using Application.Services.Shared;
using Domain.Entities.Products;
using Domain.Enums;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Products;

public class ProductCategoryService : DeleteService<ProductCategory>
    , IProductCategoryService
{


    private readonly IValidator<ProductCategoryRequest> _validator;
    private readonly IProductCategoryQueries _query;
    private ProductCategoryReadResponse Map(ProductCategory entity)
    {
        var result = new ProductCategoryReadResponse()
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            IsDeleted = entity.IsDeleted,

            ParentId = entity.ParentId,
            ParentName = entity.Parent?.Name,

            CreatedAt = entity.CreatedAt,
            CreatedByUserId = entity.CreatedByUserId,
            CreatedByUserName = entity.CreatedByUser?.UserName,

            UpdatedAt = entity.UpdateAt,
            UpdatedByUserId = entity.UpdatedByUserId,
            UpdatedByUserName = entity.UpdatedByUser?.UserName,

            DeleteAt = entity.DeletedAt,
            DeletedByUserId = entity.DeletedByUserId,
            DeletedByUserName = entity.DeletedByUser?.UserName,
        };
        return result;
    }
    public ProductCategoryService(IBaseRepository<ProductCategory> repo
        , IUnitOfWork uow
        , ICurrentUserService currentUserService
        , IValidator<ProductCategoryRequest> validator,
IProductCategoryQueries query)
        : base(repo, currentUserService, uow)
    {
        _validator = validator;
        _query = query;
    }

    public async Task<Result<ProductCategoryReadResponse>> AddAsync(
        ProductCategoryRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var result = _validator.Validate(request);
            if (!result.IsValid)
            {
                var errorMessage = string.Join(";", result.Errors.Select(e => e.ErrorMessage));
                return Result<ProductCategoryReadResponse>.Failure(errorMessage
                    , ErrorType.BadRequest);
            }
            if (request.ParentId != null)
            {
                if (!(await _repository.IsExistAsync(e => e.ParentId
                == request.ParentId, cancellationToken)))
                {
                    return Result<ProductCategoryReadResponse>.Failure
                        ($"parentId {request.ParentId}don't exist"
                        , ErrorType.NotFound);
                }
            }

            var productCategory = new ProductCategory()
            {
                Name = request.Name,
                Description = request.Description,
                ParentId = request.ParentId,
                CreatedAt = DateTime.Now,
                CreatedByUserId = _currentUserService.UserId,
            };
            _repository.Add(productCategory);
            await _uow.SaveChangesAsync(cancellationToken);
            return await FindAsync(productCategory.Id, cancellationToken);
        }
        catch (Exception ex)
        {
            return Result<ProductCategoryReadResponse>.Failure($"Error:{ex.Message}"
                , ErrorType.InternalServerError);
        }
    }

    public async Task<Result> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        try
        {
            return await SoftDeleteAsync(id, cancellationToken);
        }
        catch (Exception ex)
        {
            return Result.Failure($"Error:{ex.Message}"
                , ErrorType.InternalServerError);
        }
    }

    public async Task<Result<ProductCategoryReadResponse>> FindAsync(int id
        , CancellationToken cancellationToken = default)
    {
        try
        {
            if (id <= 0)
            {
                return Result<ProductCategoryReadResponse>.InvalidId();
            }
            var category = await _repository.FindAsync(e => e.Id == id
            , cancellationToken, "Parent,CreatedByUser,UpdatedByUser,DeletedByUser");
            if (category is null)
            {
                return Result<ProductCategoryReadResponse>.NotFound(nameof(category));
            }
            var result = Map(category);
            return Result<ProductCategoryReadResponse>.Success(result);
        }
        catch (Exception ex)
        {
            return Result<ProductCategoryReadResponse>.Failure($"Error:{ex.Message}"
                , ErrorType.InternalServerError);
        }
    }

    public async Task<Result<IReadOnlyCollection<ProductCategoryReadResponse>>>
        GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var categories = await _repository.GetAllAsync(null!, cancellationToken
                , "Parent,CreatedByUser,UpdatedByUser,DeletedByUser");
            if (categories is null || !categories.Any())
            {
                return Result<IReadOnlyCollection<ProductCategoryReadResponse>>
                    .NotFound(nameof(categories));
            }
            var result = new List<ProductCategoryReadResponse>();
            foreach (var category in categories)
            {
                result.Add(Map(category));
            }
            return Result<IReadOnlyCollection<ProductCategoryReadResponse>>
                .Success(result);
        }
        catch (Exception ex)
        {
            return Result<IReadOnlyCollection<ProductCategoryReadResponse>>
                .Failure($"Error:{ex.Message}"
                , ErrorType.InternalServerError);
        }
    }

    public async Task<Result<IReadOnlyCollection<ProductCategoryChildrenReadResponse>>>
        GetAllChildrenAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var categories = await _repository.GetAllAsync(e => e.ParentId == id
            , cancellationToken, "Parent,CreatedByUser,UpdatedByUser,DeletedByUser");
            if (categories is null || !categories.Any())
            {
                return Result<IReadOnlyCollection<ProductCategoryChildrenReadResponse>>
                    .NotFound(nameof(categories));
            }
            var result = new List<ProductCategoryChildrenReadResponse>();
            foreach (var category in categories)
            {
                var e = new ProductCategoryChildrenReadResponse()
                {
                    Id = category.Id,
                    Name = category.Name,
                    Description = category.Description,
                    IsDeleted = category.IsDeleted,
                    CreatedAt = category.CreatedAt,
                    CreatedByUserId = category.CreatedByUserId,
                    CreatedByUserName = category.CreatedByUser?.UserName,
                    UpdatedAt = category.UpdateAt,
                    UpdatedByUserId = category.UpdatedByUserId,
                    UpdatedByUserName = category.UpdatedByUser?.UserName,
                    DeleteAt = category.DeletedAt,
                    DeletedByUserId = category.DeletedByUserId,
                    DeletedByUserName = category.DeletedByUser?.UserName,
                };
                result.Add(e);
            }
            return Result<IReadOnlyCollection<ProductCategoryChildrenReadResponse>>
                .Success(result);
        }
        catch (Exception ex)
        {
            return Result<IReadOnlyCollection<ProductCategoryChildrenReadResponse>>
                .Failure($"Error:{ex.Message}"
                , ErrorType.InternalServerError);
        }
    }

    public async Task<Result<IReadOnlyCollection<ProductCategoryTreeReadResponse>>>
        GetAllTreeAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var parents = await _repository.GetAllAsync(e => e.ParentId == null
            , cancellationToken, "Parent,CreatedByUser,UpdatedByUser,DeletedByUser");
            if (parents is null || !parents.Any())
            {
                return Result<IReadOnlyCollection<ProductCategoryTreeReadResponse>>
                    .NotFound(nameof(parents));
            }
            var result = new List<ProductCategoryTreeReadResponse>();
            foreach (var parent in parents)
            {
                var children = await _repository.GetAllAsync(
                    e => e.ParentId == parent.Id, cancellationToken
                    , "Parent,CreatedByUser,UpdatedByUser,DeletedByUser");
                var childrenResponse = new List<ProductCategoryTreeReadResponse>();
                foreach (var child in children)
                {
                    var childResponse = new ProductCategoryTreeReadResponse()
                    {
                        Id = child.Id,
                        Name = child.Name,
                        Description = child.Description,

                        CreatedAt = child.CreatedAt,
                        CreatedByUserId = child.CreatedByUserId,
                        CreatedByUserName = child.CreatedByUser?.UserName,

                        UpdatedAt = child.UpdateAt,
                        UpdatedByUserId = child.UpdatedByUserId,
                        UpdatedByUserName = child.UpdatedByUser?.UserName,

                        IsDeleted = child.IsDeleted,
                        DeleteAt = child.DeletedAt,
                        DeletedByUserId = child.DeletedByUserId,
                        DeletedByUserName = child.DeletedByUser?.UserName,

                        Children = null
                    };
                    childrenResponse.Add(childResponse);
                }
                var parentResponse = MapTree(parent, childrenResponse);
                result.Add(parentResponse);
            }
            return Result<IReadOnlyCollection<ProductCategoryTreeReadResponse>>
                .Success(result);
        }
        catch (Exception ex)
        {
            return Result<IReadOnlyCollection<ProductCategoryTreeReadResponse>>
                .Failure($"Error:{ex.Message}", ErrorType.InternalServerError);
        }
    }
    private ProductCategoryTreeReadResponse MapTree(ProductCategory parent
        , IReadOnlyCollection<ProductCategoryTreeReadResponse>? children = null)
    {
        var result = new ProductCategoryTreeReadResponse()
        {
            Id = parent.Id,
            Name = parent.Name,
            Description = parent.Description,

            CreatedAt = parent.CreatedAt,
            CreatedByUserId = parent.CreatedByUserId,
            CreatedByUserName = parent.CreatedByUser?.UserName,

            UpdatedAt = parent.UpdateAt,
            UpdatedByUserId = parent.UpdatedByUserId,
            UpdatedByUserName = parent.UpdatedByUser?.UserName,

            IsDeleted = parent.IsDeleted,
            DeleteAt = parent.DeletedAt,
            DeletedByUserId = parent.DeletedByUserId,
            DeletedByUserName = parent.DeletedByUser?.UserName,

            Children = children
        };
        return result;
    }

    public async Task<Result<ProductCategoryDetailsResponse>> UpdateAsync(int id
        , ProductCategoryRequest request, CancellationToken cancellationToken)
    {
        try
        {
            if (id <= 0)
            {
                return Result<ProductCategoryDetailsResponse>.InvalidId();
            }
            var result = _validator.Validate(request);
            if (!result.IsValid)
            {
                var errorMessage = string.Join(";", result.Errors.
                    Select(e => e.ErrorMessage));
                return Result<ProductCategoryDetailsResponse>
                    .Failure(errorMessage, ErrorType.BadRequest);
            }
            var category = await _repository.FindAsync(e => e.Id == id
            , cancellationToken);
            if (category is null)
            {
                return Result<ProductCategoryDetailsResponse>.NotFound(nameof(category));
            }
            category.ParentId = request.ParentId;
            category.Name = request.Name;
            category.Description = request.Description;
            category.UpdateAt = DateTime.UtcNow;
            category.UpdatedByUserId = _currentUserService.UserId;
            _repository.Update(category);
            await _uow.SaveChangesAsync(cancellationToken);
            return await  _query.GetCategoryByIdAsync(category.Id, cancellationToken);

        }
        catch (Exception ex)
        {
            return Result<ProductCategoryDetailsResponse>.Failure($"Error:{ex.Message}"
                , ErrorType.InternalServerError);
        }
    }

    public async Task<Result<IEnumerable<object>>> GetCategoriesNamesAsync(
        CancellationToken cancellationToken = default)
    {
        try
        {
            var categories = await _repository.GetAllAsync(
                e => !e.IsDeleted,
             cancellationToken);
            if (categories is null || !categories.Any())
            {
                return Result<IEnumerable<object>>.NotFound("categories");
            }
            var result = categories.Select(e => new
            {
                Id=e.Id,
                Name=e.Name
            }).ToList();
            return Result<IEnumerable<object>>.Success(result);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<object>>
                .Exception(nameof(GetCategoriesNamesAsync), ex);
        }

    }
}
