using Application.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.StockMovements.DTOs.Response
{
    public sealed record StockMovementTypeReadResponse : BaseDeletableReadResponse
    {
        public string Name { get; init; } = null!;
        public string? Description { get; init; }
        public string Direction { get; init; } = null!;

    }
}
