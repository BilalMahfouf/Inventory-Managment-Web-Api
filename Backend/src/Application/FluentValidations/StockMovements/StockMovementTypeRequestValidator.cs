using Application.Abstractions.UnitOfWork;
using Application.DTOs.StockMovements.Request;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.FluentValidations.StockMovements
{
    public class StockMovementTypeRequestValidator 
        : AbstractValidator<StockMovementTypeRequest>
    {
        public StockMovementTypeRequestValidator(IUnitOfWork uow)
        {
            RuleFor(e => e.Name)
                .NotEmpty().WithMessage("Name is required")
                .MaximumLength(100).WithMessage("Name must not exceed 100 characters")
                .MustAsync(async (name, ct) =>
                {
                    var exists = await uow.StockMovementTypes
                        .IsExistAsync(smt => smt.Name == name, ct);
                    return !exists;
                }).WithMessage("Name must be unique ");
            RuleFor(e=>e.Direction).NotEmpty().WithMessage("Direction is required")
                .InclusiveBetween((byte)1, (byte)3)
                .WithMessage("Direction must be 1 (IN), 2 (OUT) or 3 (ADJUST)");
        }
    }
}
