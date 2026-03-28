using Application.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Locations.DTOs.Response
{
    public sealed record LocationTypeReadResponse : BaseDeletableReadResponse
    {
        public string Name { get; init; } = null!;
        public string? Description { get; init; }
    }
}
