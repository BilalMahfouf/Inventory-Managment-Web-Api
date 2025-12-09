using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.PagedLists
{
    public  class PagedList<T> 
    {
        public IEnumerable<T> Item { get; init; } = null!;
        public int TotalCount { get; init; }
        public int PageSize { get; init; }
        public int Page { get; init; }
        public bool HasNextPage => Page * PageSize < TotalCount;
        public bool HasPreviousPage => Page > 1;
    }
}
