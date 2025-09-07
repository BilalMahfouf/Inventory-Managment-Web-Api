using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Abstractions.Services.User
{
    public interface ICurrentUserService
    {
        int UserId { get; }
        
    }
}
