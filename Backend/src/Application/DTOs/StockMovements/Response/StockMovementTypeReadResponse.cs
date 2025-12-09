using Application.DTOs.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.StockMovements.Response
{
    public sealed record StockMovementTypeReadResponse : BaseDeletableReadResponse
    {
        public string Name { get; init; } = null!;
        public string? Description { get; init; }
        public string Direction { get; init; } = null!;

    }
}
