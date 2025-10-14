using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Products.Response.Categories;

public sealed record ProductCategoryDetailsResponse
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public string? Description { get; init; } = string.Empty;
    public string Type { get; init; } = string.Empty;
    public int? ParentId { get; init; }
    public string? ParentName { get; init; }
    public DateTime CreatedAt { get; init; }
    public int CreatedByUserId { get; init; }
    public string CreatedByUserName { get; init; } = string.Empty;
    public int? UpdatedByUserId { get; init; }
    public string? UpdatedByUserName { get; init; }
    public IEnumerable<object>? SubCategories { get; init; }
}
