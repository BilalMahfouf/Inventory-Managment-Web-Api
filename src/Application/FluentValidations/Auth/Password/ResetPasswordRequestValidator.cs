using Application.DTOs.Authentication.Password;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.FluentValidations.Auth.Password
{
    public class ResetPasswordRequestValidator:AbstractValidator<ResetPasswordRequest>
    {
        public ResetPasswordRequestValidator()
        {
            RuleFor(e => e.Password).NotEmpty().WithMessage("Password is required")
                .MinimumLength(6).WithMessage("Password not strong");
            RuleFor(e => e.Token).NotEmpty().WithMessage("token is required");
            RuleFor(e => e.ConfirmPassword).NotEmpty()
                .WithMessage("confirm password is required")
                .Equal(e => e.Password).WithMessage("invalid password");
            RuleFor(e => e.Email).NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email address");
        }
    }
}
