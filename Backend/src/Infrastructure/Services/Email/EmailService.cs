using Application.Abstractions.Services.Email;
using Application.DTOs.Email;
using Application.Results;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services.Email
{
    internal class EmailService : IEmailService
    {
        private readonly EmailOptions _emailOptions;

        public EmailService(IOptions<EmailOptions> options)
        {
            _emailOptions = options.Value;
        }

        public async Task<Result> SendEmailAsync(SendEmailRequest request
            ,CancellationToken cancellationToken)
        {
            try
            {

                var email = new MimeMessage();
                email.From.Add(MailboxAddress.Parse(_emailOptions.Email));
                email.To.Add(MailboxAddress.Parse(request.To));
                email.Subject = request.Subject;
                email.Body = new TextPart(TextFormat.Html)
                {
                    Text = request.Body
                };

                using var smtp = new SmtpClient();
                await smtp.ConnectAsync(_emailOptions.Host, _emailOptions.Port
                    , SecureSocketOptions.StartTls, cancellationToken);
                await smtp.AuthenticateAsync(_emailOptions.Email
                    , _emailOptions.Password, cancellationToken);
                await smtp.SendAsync(email, cancellationToken);
                await smtp.DisconnectAsync(true, cancellationToken);
                return Result.Success;
            }
            catch(Exception ex)
            {
                return Result.Failure($"Error happened while sending email : {ex.Message}"
                    , Domain.Enums.ErrorType.InternalServerError);
            }
        }
    }
}
