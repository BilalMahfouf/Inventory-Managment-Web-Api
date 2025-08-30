using Application.DTOs.Email;
using Application.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Abstractions.Services.Email
{
    public interface IEmailService
    {
        Task<Result> SendEmailAsync(SendEmailRequest request
            , CancellationToken cancellationToken);
    }
}
