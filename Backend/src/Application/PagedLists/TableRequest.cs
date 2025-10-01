using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.PagedLists
{
    public record  TableRequest
    {
      public  int PageSize { get; init; }
        public int Page {  get; init; }
        public string? search { get; init; } = null;
    }
}
