using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enums
{
    public enum ErrorType
    {
        NotFound=1,
        BadRequest,
        InternalServerError,
        Conflict,
        Unauthorized
    }
}
