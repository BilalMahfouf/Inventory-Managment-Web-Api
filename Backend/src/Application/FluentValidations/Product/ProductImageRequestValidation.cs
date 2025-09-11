using Application.DTOs.Products.Request.ProductImages;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.FluentValidations.Product
{
    public class ProductImageUploadRequestValidation 
        : AbstractValidator<ProductImageUploadRequest>
    {
        public ProductImageUploadRequestValidation()
        {
            RuleFor(e => e.FileStream)
                .NotNull()
                .WithMessage("File stream must be provided.")
                .Must(stream => stream.Length > 0)
                .WithMessage("File stream cannot be empty.");
        }
    }
}
