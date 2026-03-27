using Application.Abstractions.UnitOfWork;
using Application.DTOs.Locations.Request;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.FluentValidations.Locations
{
    public class LocationCreateRequestValidator : AbstractValidator<LocationCreateRequest>
    {
        public LocationCreateRequestValidator(IUnitOfWork uow)
        {
            RuleFor(e=>e.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(100).WithMessage("Name must not exceed 100 characters.")
                .MustAsync(async (name, cancellation) =>
                {
                    var existingLocation = await uow.Locations.IsExistAsync(
                        e=>e.Name==name && !e.IsDeleted);
                    return !existingLocation;
                }).WithMessage("A location with the same name already exists.");

            RuleFor(e=>e.Address)
                .NotEmpty().WithMessage("Address is required.")
                .MaximumLength(200)
                .WithMessage("Address must not exceed 200 characters.");
            
            RuleFor(e=>e.LocationTypeId).GreaterThan(0)
                .NotEmpty().WithMessage("locationTypeId is required.")
                .WithMessage("LocationTypeId must be greater than 0.")
                .MustAsync(async (locationTypeId, cancellation) =>
                {
                    var locationType = await uow.LocationTypes.IsExistAsync(
                        e => e.Id == locationTypeId && !e.IsDeleted);
                    return locationType;
                }).WithMessage("LocationTypeId does not exist.");

        }
    }
}
