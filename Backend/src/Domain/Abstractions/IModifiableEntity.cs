using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Abstractions
{
    public interface IModifiableEntity
    {
        public DateTime? UpdatedAt { get; set; }
        public int? UpdatedByUserId { get; set; }
    }
}
