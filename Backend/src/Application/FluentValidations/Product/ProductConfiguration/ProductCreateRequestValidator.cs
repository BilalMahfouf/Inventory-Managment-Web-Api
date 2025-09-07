using Application.DTOs.Products.Request.Products;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.FluentValidations.Product.Configuration
{
    public class ProductCreateRequestValidator : AbstractValidator<ProductCreateRequest>
    {
        public ProductCreateRequestValidator()
        {
            RuleFor(p => p.SKU)
                .NotEmpty().WithMessage("Product SKU is required.")
                .MaximumLength(50).WithMessage("Product SKU must not exceed 50 characters.");
            RuleFor(p => p.UnitOfMeasureId)
                .GreaterThan(0).WithMessage("Unit of Measure ID must be a positive integer.");
            RuleFor(p => p.Name)
                .NotEmpty().WithMessage("Product name is required.")
                .MaximumLength(100).WithMessage("Product name must not exceed 100 characters.");
            RuleFor(p => p.Description)
                .MaximumLength(500).WithMessage("Product description must not exceed 500 characters.");
            RuleFor(p => p.CategoryId)
                .GreaterThan(0).WithMessage("Category ID must be a positive integer.");
            RuleFor(p => p.UnitPrice)
                .GreaterThanOrEqualTo(0).WithMessage("Unit price must be a non-negative value.");
            RuleFor(p => p.CostPrice)
                .GreaterThanOrEqualTo(0).WithMessage("Cost price must be a non-negative value.")
                .LessThanOrEqualTo(p => p.UnitPrice).WithMessage("Cost price must not exceed unit price.");

        }
    }
}
