using Application.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.UnitOfMeasures.DTOs
{
    public sealed record UnitOfMeasureReadResponse:BaseFullReadResponse
    {
        public string Name { get; init; } = string.Empty;
        public string? Description { get; init; } = string.Empty;
    }
}
