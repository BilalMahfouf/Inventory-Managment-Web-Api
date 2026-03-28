using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Authentication.DTOs.Email
{
    public sealed record SendConfirmEmailRequest(string Email,string ClientUri);
}
