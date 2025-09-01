using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.User.Response
{
    public sealed record UserSessionReadResponse
    {
        public int Id { get; init; }             
        public int UserId { get; init; }        
        public string UserName { get; init; }=string.Empty;
        public string TokenType { get; init; } =string.Empty;
        public DateTime CreatedAt { get; init; }  
    }
}
