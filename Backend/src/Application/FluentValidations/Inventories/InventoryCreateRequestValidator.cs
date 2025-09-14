using Application.Abstractions.UnitOfWork;
using Application.DTOs.Inventories.Request;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.FluentValidations.Inventories
{
    public class InventoryCreateRequestValidator
        : AbstractValidator<InventoryCreateRequest>
    {
        public InventoryCreateRequestValidator(IUnitOfWork uow)
        {
            RuleFor(x => x.ProductId)
                .GreaterThan(0).WithMessage("ProductId must be greater than 0.")
                .MustAsync(async (productId, ct) =>
                {
                    var isExist = await uow.Products
                    .IsExistAsync(p => p.Id == productId && !p.IsDeleted && p.IsActive
                    , ct);
                    return isExist;
                }).WithMessage("ProductId does not exist or it's not active");

            RuleFor(x => x.LocationId)
                .GreaterThan(0).WithMessage("LocationId must be greater than 0.")
                .MustAsync(async (locationId, ct) =>
                {
                    var isExist = await uow.Locations
                    .IsExistAsync(l => l.Id == locationId && !l.IsDeleted && l.IsActive
                    , ct);
                    return isExist;
                }).WithMessage("LocationId does not exist or it's not active");

            RuleFor(x => x.QuantityOnHand)
                .GreaterThanOrEqualTo(0)
                .WithMessage("QuantityOnHand must be greater than or equal to 0.");

            RuleFor(x => x.ReorderLevel)
                .GreaterThanOrEqualTo(0)
                .WithMessage("ReorderLevel must be greater than or equal to 0.");

            RuleFor(x => x.MaxLevel)
                .GreaterThan(0).WithMessage("MaxLevel must be greater than 0.")
                .GreaterThanOrEqualTo(x => x.ReorderLevel)
                .WithMessage("MaxLevel must be greater than or equal to ReorderLevel.");
        }
    }
}
