using Application.Authentication.DTOs.Email;
using Application.Shared.Validation;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Authentication.Validators.Email
{
    public class SendConfirmEmailRequestValidator:AbstractValidator<SendConfirmEmailRequest>
    {
        public SendConfirmEmailRequestValidator()
        {
            RuleFor(e => e.Email).ValidEmail();
            RuleFor(e => e.ClientUri).NotEmpty().WithMessage("Client uri is required");
        }
    }
}
