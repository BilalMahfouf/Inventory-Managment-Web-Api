using Application.DTOs.Users.Request;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.FluentValidations.User.Configuration
{
    public class UserRoleRequestValidator:AbstractValidator<UserRoleRequest>
    {
        public UserRoleRequestValidator()
        {
            RuleFor(e => e.Name).NotEmpty().WithMessage("Name is required");
        }
    }
}
