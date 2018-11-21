using System.Collections.Generic;

namespace HZC.Infrastructure
{
    public class PageListResult<T> : Result
    {
        public int RecordCount { get; set; }

        public int PageIndex { get; set; }

        public int PageSize { get; set; }

        public IEnumerable<T> Body { get; set; }
    }
}
