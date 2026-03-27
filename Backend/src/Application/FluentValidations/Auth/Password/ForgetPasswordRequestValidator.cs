using Application.DTOs.Authentication.Password;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.FluentValidations.Auth.Password
{
    public class ForgetPasswordRequestValidator:AbstractValidator<ForgetPasswordRequest>
    {
        public ForgetPasswordRequestValidator()
        {
            RuleFor(e => e.Email).NotEmpty().WithMessage("Email is required")
                 .EmailAddress().WithMessage("Invalid email address");
            RuleFor(e => e.ClientUri).NotEmpty().WithMessage("Client Uri is required");
        }
    }
}
