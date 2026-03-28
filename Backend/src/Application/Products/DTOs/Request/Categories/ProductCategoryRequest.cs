using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Products.DTOs.Request.Categories
{
    public sealed record ProductCategoryRequest(string Name,string? Description
        ,int? ParentId);
}
