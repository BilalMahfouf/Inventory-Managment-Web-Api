using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.StockMovements.Request
{
    public sealed record StockMovementTypeRequest
    {
        public string Name { get; init; } = null!;
        public string? Description { get; init; }
        public byte Direction { get; init; }

    }
}
