using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Locations.Request
{
    public sealed record LocationUpdateRequest
    {
        public int Id { get; set; }
        public string Name { get; init; } = null!;  
        public string Address { get; init; } = null!;
        public int LocationTypeId { get; init; }
        public bool IsActive { get; init; }
    }
}
