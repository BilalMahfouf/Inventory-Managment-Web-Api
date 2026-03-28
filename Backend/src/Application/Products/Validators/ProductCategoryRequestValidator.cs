using Application.Products.DTOs.Request.Categories;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Products.Validators
{
    public class ProductCategoryRequestValidator : AbstractValidator<ProductCategoryRequest>
    {
        public ProductCategoryRequestValidator()
        {
            RuleFor(e => e.Name).NotEmpty().WithMessage("name is required");
            RuleFor(e => e.ParentId).GreaterThan(0).WithMessage("invalid parent Id");
        }
    }
}
