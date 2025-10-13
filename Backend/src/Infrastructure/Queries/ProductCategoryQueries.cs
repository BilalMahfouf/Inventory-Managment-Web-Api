using Application.Abstractions.Queries;
using Application.DTOs.Products.Response.Categories;
using Application.Results;
using Domain.Enums;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Queries;

public class ProductCategoryQueries : IProductCategoryQueries
{
    private readonly InventoryManagmentDBContext _context;

    public ProductCategoryQueries(InventoryManagmentDBContext context)
    {
        _context = context;
    }


    public async Task<Result<ProductCategoryDetailsResponse>>
        GetCategoryByIdAsync(int id, CancellationToken cancellationToken)
    {
        try
        {
            var category = await _context.ProductCategories.Include(e => e.Parent)
                .Include(e => e.CreatedByUser).Include(e => e.UpdatedByUser)
                .FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted, cancellationToken);
            if (category is null)
            {
                return Result<ProductCategoryDetailsResponse>.NotFound(nameof(category));
            }

            if (category.Type == ProductCategoryType.SubCategory)
            {
                return Result<ProductCategoryDetailsResponse>.Success(new ProductCategoryDetailsResponse
                {
                    Id = category.Id,
                    Name = category.Name,
                    Description = category.Description,
                    Type = category.Type.ToString(),
                    ParentId = category.ParentId,
                    ParentName = category.Parent?.Name,
                    CreatedAt = category.CreatedAt,
                    CreatedByUserId = category.CreatedByUserId,
                    CreatedByUserName = category.CreatedByUser.UserName,
                    UpdatedByUserId = category.UpdatedByUserId,
                    UpdatedByUserName = category.UpdatedByUser?.UserName
                });
            }
            var result = new ProductCategoryDetailsResponse
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                Type = category.Type.ToString(),
                CreatedAt = category.CreatedAt,
                CreatedByUserId = category.CreatedByUserId,
                CreatedByUserName = category.CreatedByUser.UserName,
                UpdatedByUserId = category.UpdatedByUserId,
                UpdatedByUserName = category.UpdatedByUser?.UserName,
                SubCategories = await _context.ProductCategories
      .Where(e => e.ParentId == category.Id && !e.IsDeleted)
      .Select(e => new
      {
          SubCategoryId = e.Id,
          SubCategoryName = e.Name,
      }).ToListAsync(cancellationToken)
            };
            return Result<ProductCategoryDetailsResponse>.Success(result);

        }
        catch (Exception ex)
        {
            return Result<ProductCategoryDetailsResponse>.Exception(nameof(GetCategoryByIdAsync), ex);
        }

    }

    public async Task<Result<IEnumerable<object>>> GetMainCategoriesAsync(
        CancellationToken cancellationToken)
    {
        try
        {
            var categories = await _context.ProductCategories
                .Where(e => e.Type == ProductCategoryType.MainCategory && !e.IsDeleted)
                .Select(e => new
                {
                    e.Id,
                    e.Name
                }).ToListAsync(cancellationToken);
            if (categories is null || !categories.Any())
            {
                return Result<IEnumerable<object>>.NotFound(nameof(categories));
            }
            return Result<IEnumerable<object>>.Success(categories);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<object>>.Exception(nameof(GetMainCategoriesAsync), ex);
        }
    }
}
