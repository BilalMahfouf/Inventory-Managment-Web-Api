using Application.DTOs.Users.Request;
using Application.FluentValidations.Extensions;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.FluentValidations.User.Configuration
{
    public class UserCreateRequestValidator : AbstractValidator<UserCreateRequest>
    {
        public UserCreateRequestValidator()
        {
            RuleFor(e => e.Email).ValidEmail();
            RuleFor(e => e.UserName).NotEmpty().WithMessage("User Name is required");
            RuleFor(e => e.Password).NotEmpty().WithMessage("Password is required")
                .MinimumLength(6).WithMessage("Weak Password");
            RuleFor(e => e.FirstName).NotEmpty().WithMessage("FirstName is required");
            RuleFor(e => e.LastName).NotEmpty().WithMessage("LastName is required");
            RuleFor(e => e.RoleId).GreaterThan(0).WithMessage("Invalid Id");
        }
    }
}
