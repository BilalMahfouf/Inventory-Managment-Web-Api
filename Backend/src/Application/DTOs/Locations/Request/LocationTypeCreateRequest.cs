using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Locations.Request
{
    public sealed record LocationTypeCreateRequest
    {
        public string Name { get; init; } = null!;

        public string? Description { get; init; }
    }
}
