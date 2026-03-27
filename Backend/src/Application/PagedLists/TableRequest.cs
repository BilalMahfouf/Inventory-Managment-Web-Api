using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.PagedLists
{
    public class TableRequest
    {
      public  int PageSize { get; init; }
        public int Page {  get; init; }
        public string? search { get; init; } = null;
        public string? SortColumn { get; init; } = null;
        public string? SortOrder { get; init; }= null;
        public TableRequest()
        {

        }
        private TableRequest(
            int pageSize, 
            int page,
            string? search,
            string? sortColumn, 
            string? sortOrder)
        {
            PageSize = pageSize;
            Page = page;
            this.search = search;
            SortColumn = sortColumn;
            SortOrder = sortOrder;
        }
        public static TableRequest Create(
            int? pageSize,
            int? page,
            string? search = null,
            string? sortColumn = null,
            string? sortOrder = null)
        {
               int pageNumber = page is null || page <= 0 ? 1 : (int)page; 
                int size = pageSize is null || pageSize <= 0 ? 10 : (int)pageSize;
            return new TableRequest(
                size,
                pageNumber,
                search,
                sortColumn,
                sortOrder);
        }
    }
}
