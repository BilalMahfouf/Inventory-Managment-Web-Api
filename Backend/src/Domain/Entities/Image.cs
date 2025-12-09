using Domain.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{

    public class Image : IBaseEntity, IEntity
    {
        public int Id { get; set; }
        public string FileName { get; set; } = null!;
        public string StoragePath { get; set; } = null!;
        public string MimeType { get; set; } = null!;
        public long SizeInBytes { get; set; }
        public DateTime CreatedAt { get; set; }
        public int CreatedByUserId { get; set; }
        
      

        public virtual User CreatedByUser { get; set; } = null!;
    }

}
