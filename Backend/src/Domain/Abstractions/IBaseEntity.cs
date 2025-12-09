using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Abstractions
{
    public interface IBaseEntity:IEntity
    {
        DateTime CreatedAt { get; set; }
        int CreatedByUserId { get; set; }
    }
}
