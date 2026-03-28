using Application.Users.DTOs.Request;
using Application.Shared.Validation;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Users.Validators.Configuration
{
    public class UserUpdateRequestValidator : AbstractValidator<UserUpdateRequest>
    {
        public UserUpdateRequestValidator()
        {
            RuleFor(e => e.UserName).NotEmpty().WithMessage("User Name is required");
            RuleFor(e => e.FirstName).NotEmpty().WithMessage("FirstName is required");
            RuleFor(e => e.LastName).NotEmpty().WithMessage("LastName is required");
            RuleFor(e => e.RoleId).GreaterThan(0).WithMessage("Invalid Id");
        }
    }
}
