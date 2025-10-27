using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enums;

public enum TransferStatus : byte
{
    Pending = 1,
    Approved = 2,
    InTransit = 3,
    Completed = 4,
    Cancelled = 5,
    Rejected = 6,
    Failed = 7
}
