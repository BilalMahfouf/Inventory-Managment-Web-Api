using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities
{

    public class Image
    {
        public int Id { get; set; }
        public string FileName { get; set; } = null!;
        public string StoragePath { get; set; } = null!;
        public string MimeType { get; set; } = null!;
        public int SizeInBytes { get; set; }
        public DateTime CreatedAt { get; set; }
        public int CreatedByUserId { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? UpdatedByUserId { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
        public int DeletedByUserId { get; set; }


        public virtual User CreatedByUser { get; set; } = null!;
        public virtual User? UpdatedByUser { get; set; }
        public virtual User? DeletedByUser { get; set; }
    }

}
