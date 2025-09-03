using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Base
{
    public abstract record BaseDeletableReadResponse : BaseReadResponse
    {
        public DateTime? DeleteAt { get; init; }
        public bool IsDeleted { get; init; }
        public int? DeletedByUserId { get; init; }
        public string? DeletedByUserName { get; init; } = string.Empty;
    }
}
