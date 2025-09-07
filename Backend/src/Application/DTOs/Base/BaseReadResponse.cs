using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Base
{
    public abstract record BaseReadResponse
    {
        public int Id { get; init; }
        public DateTime CreatedAt { get; init; }
        public int? CreatedByUserId { get; init; }
        public string? CreatedByUserName { get; init; } = string.Empty;
    }
}
