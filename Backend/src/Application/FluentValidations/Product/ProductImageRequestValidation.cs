using Application.Abstractions.UnitOfWork;
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
        public ProductImageUploadRequestValidation(IUnitOfWork uow)
        {
            RuleFor(e => e.FileStream)
                .NotNull()
                .WithMessage("File stream must be provided.")
                .Must(stream => stream.Length > 0)
                .WithMessage("File stream cannot be empty.");

            RuleFor(e => e.FileName).NotEmpty().WithMessage("File name is required");

            RuleFor(e=> e.MimeType).NotEmpty().WithMessage("MIME type is required");

            RuleFor(e => e.FileSize).NotEmpty().WithMessage("FileSize is required")
                .GreaterThan(0)
                .WithMessage("File size must be greater than zero.");

            RuleFor(e => e.IsPrimary).NotEmpty().WithMessage("IsPrimary is Required");

            RuleFor(e => e.ProductId).NotEmpty().WithMessage("ProductId is required")
                .GreaterThan(0).WithMessage("invalid Id")
                .MustAsync(async (id, cancellationToken) =>
                {
                    return await uow.Products.IsExistAsync(p => p.Id == id
                    , cancellationToken);
                }).WithMessage("ProductId don't exist");


                
        }
    }
}
