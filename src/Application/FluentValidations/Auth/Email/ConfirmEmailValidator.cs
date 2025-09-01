using Application.DTOs.Authentication.Email;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.FluentValidations.Auth.Email
{
    public class ConfirmEmailValidator:AbstractValidator<ConfirmEmailRequest>
    {
        public ConfirmEmailValidator()
        {
            RuleFor(e => e.Email).NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email address");
            RuleFor(e => e.Token).NotEmpty().WithMessage("token is required");
        }
    }
}
