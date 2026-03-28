using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Users.Contracts
{
    public interface ICurrentUserService
    {
        int UserId { get; }
        
    }
}
