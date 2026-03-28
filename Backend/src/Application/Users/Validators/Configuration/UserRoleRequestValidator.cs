using Application.Users.DTOs.Request;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Users.Validators.Configuration
{
    public class UserRoleRequestValidator:AbstractValidator<UserRoleRequest>
    {
        public UserRoleRequestValidator()
        {
            RuleFor(e => e.Name).NotEmpty().WithMessage("Name is required");
        }
    }
}
