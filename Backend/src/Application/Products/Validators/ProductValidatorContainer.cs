using Application.Products.DTOs.Request.Products;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Products.Validators
{
    public class ProductValidatorContainer
    {
        public IValidator<ProductCreateRequest> CreateValidator { get; }
        public IValidator<ProductUpdateRequest> UpdateValidator { get; }
        public ProductValidatorContainer(IValidator<ProductCreateRequest> createValidator, IValidator<ProductUpdateRequest> updateValidator)
        {
            CreateValidator = createValidator;
            UpdateValidator = updateValidator;
        }
    }
}
