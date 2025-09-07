using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Authentication.Password
{
    public sealed record ForgetPasswordRequest(string Email,string ClientUri);
    
}
