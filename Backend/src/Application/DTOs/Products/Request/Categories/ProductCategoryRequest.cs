using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Products.Request.Categories
{
    public sealed record ProductCategoryRequest(string Name,string? Description
        ,int? ParentId);
}
