using Application.Shared.DTOs;
using Domain.Shared.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Shared.Contracts
{
    public interface IEmailService
    {
        Task<Result> SendEmailAsync(SendEmailRequest request
            , CancellationToken cancellationToken);
    }
}
