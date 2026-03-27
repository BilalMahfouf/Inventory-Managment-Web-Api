using Application.DTOs.UnitOfMeasure;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.FluentValidations.UnitOfMeasures
{
    public class UnitOfMeasureRequestValidator : AbstractValidator<UnitOfMeasureRequest>
    {
        public UnitOfMeasureRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(50).WithMessage("Name must not exceed 100 characters.");
           
        }
    }
}
