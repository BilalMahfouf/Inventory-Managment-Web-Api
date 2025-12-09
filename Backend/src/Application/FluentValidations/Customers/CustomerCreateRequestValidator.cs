using Application.Abstractions.UnitOfWork;
using Application.DTOs.Customers;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.FluentValidations.Customers;

public class CustomerCreateRequestValidator : AbstractValidator<CustomerCreateRequest>
{
    public CustomerCreateRequestValidator(IUnitOfWork uow)
    {
        RuleFor(e => e.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(100);
        RuleFor(e => e.Email).EmailAddress().WithMessage("Invalid email format");
        RuleFor(e => e.Phone).NotEmpty().WithMessage("Phone is required");
        RuleFor(e => e.CustomerCategoryId)
            .GreaterThan(0)
            .WithMessage("CustomerCategoryId must be greater than 0")
            .MustAsync(async (id, cancellation) =>
            {
                var isExist = await uow.CustomerCategories
                .IsExistAsync(e => e.Id == id, cancellationToken: cancellation);
                return isExist;
            });
        RuleFor(e => e.City)
            .NotEmpty().WithMessage("City is required")
            .MaximumLength(50);
        RuleFor(e => e.State)
            .NotEmpty().WithMessage("State is required")
            .MaximumLength(50);
        RuleFor(e => e.ZipCode)
            .NotEmpty().WithMessage("ZipCode is required")
            .MaximumLength(20);
        RuleFor(e => e.Street)
            .NotEmpty().WithMessage("Street is required")
            .MaximumLength(100);
    }
}
