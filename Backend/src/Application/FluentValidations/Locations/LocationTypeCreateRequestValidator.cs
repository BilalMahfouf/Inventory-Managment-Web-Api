using Application.Abstractions.UnitOfWork;
using Application.DTOs.Locations.Request;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.FluentValidations.Locations
{
    public class LocationTypeCreateRequestValidator 
        : AbstractValidator<LocationTypeCreateRequest>
    {
        public LocationTypeCreateRequestValidator(IUnitOfWork uow)
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(100).WithMessage("Name must not exceed 100 characters.")
                .MustAsync(async(name, cancellationToken) =>
                {
                    var existing = await uow.LocationTypes
                        .FindAsync(lt => lt.Name == name && !lt.IsDeleted, cancellationToken);
                    return existing is null;
                }).WithMessage("Name must be unique.");

        }
    }
}
