using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enums;

public enum StockMovementStatus : byte
{
    Pending = 1,
    Completed = 2,
    Canceled = 3

}
