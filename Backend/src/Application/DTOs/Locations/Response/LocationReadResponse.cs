using Application.DTOs.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Locations.Response
{
    public sealed record LocationReadResponse : BaseDeletableReadResponse
    {
        public string Name { get; init; } = null!;
        public string Address { get; init; } = null!;
        public bool IsActive { get; init; }
        public int LocationTypeId { get; init; }
        public string LocationTypeName { get; init; } = null!;
    }
}
