using Application.DTOs.Inventories.Request;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.FluentValidations.Inventories
{
    public  class InventoryUpdateRequestValidator
        : AbstractValidator<InventoryUpdateRequest>
    {
        public InventoryUpdateRequestValidator()
        {
            RuleFor(x => x.QuantityOnHand)
                .GreaterThanOrEqualTo(0)
                .WithMessage("QuantityOnHand must be greater than or equal to 0.");

            RuleFor(x => x.ReorderLevel)
                .GreaterThanOrEqualTo(0)
                .WithMessage("ReorderLevel must be greater than or equal to 0.");

            RuleFor(x => x.MaxLevel)
                .GreaterThanOrEqualTo(0)
                .WithMessage("MaxLevel must be greater than or equal  0.");

        }
    }
}
