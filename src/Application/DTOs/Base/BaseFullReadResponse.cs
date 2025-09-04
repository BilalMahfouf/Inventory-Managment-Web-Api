using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Base
{
    public abstract record BaseFullReadResponse : BaseDeletableReadResponse
    {
        
        public DateTime? UpdatedAt { get; init; }
   

       
        public int? UpdatedByUserId { get; init; }
     

       
        public string? UpdatedByUserName { get; init; } = string.Empty;
       
    }
}
