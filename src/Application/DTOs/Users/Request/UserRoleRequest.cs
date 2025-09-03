using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Users.Request
{
    public sealed record UserRoleRequest(string Name,string? Description);
    
}
