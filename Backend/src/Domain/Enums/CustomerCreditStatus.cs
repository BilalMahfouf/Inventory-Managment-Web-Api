using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enums;

public enum CustomerCreditStatus : byte
{
    Active = 1,
    OnHold = 2,
    Suspended = 3,
    Closed = 4
}
