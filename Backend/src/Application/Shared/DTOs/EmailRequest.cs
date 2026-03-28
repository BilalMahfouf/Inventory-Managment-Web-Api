using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Shared.DTOs
{
    public sealed record SendEmailRequest(string To,string Subject,string Body);
    
}
