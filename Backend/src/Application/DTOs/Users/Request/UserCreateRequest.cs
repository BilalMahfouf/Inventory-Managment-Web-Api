using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Users.Request
{
    public sealed record UserCreateRequest
    {
        public string UserName { get; init; } = string.Empty;
        public string Email { get; init; } = string.Empty;
        public string Password { get; init; } = string.Empty;
        public string FirstName {  get; init; } = string.Empty;
        public string LastName { get; init; }= string.Empty;
        public int RoleId { get; init; }
    }
}

