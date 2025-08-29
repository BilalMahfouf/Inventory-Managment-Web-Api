using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Authentication
{
    public sealed record RefreshTokenRequest(string refreshToken);
}
