using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Survey.Domain.Abstractions
{
    public class PaginatedResult<T>
    {
        public PaginatedResult(List<T> data)
        {
            List = data;
        }
        public List<T> List { get; set; }

        internal PaginatedResult(List<T> data, int count = 0, int page = 1, int pageSize = 10)
        {
            List = data;
            CurrentPage = page;
            PageSize = pageSize;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            TotalCount = count;
        }

        public static PaginatedResult<T> Success(List<T> data, int count, int page, int pageSize)
        {
            return new(data, count, page, pageSize);
        }

        public int CurrentPage { get; set; }

        public int TotalPages { get; set; }

        public int TotalCount { get; set; }

        public int PageSize { get; set; }

        public bool HasPreviousPage => CurrentPage > 1;

        public bool HasNextPage => CurrentPage < TotalPages;

    }
}
