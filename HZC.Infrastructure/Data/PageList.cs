using System.Collections.Generic;

namespace HZC.Infrastructure
{
    public class PageList<T>
    {
        /// <summary>
        /// 记录总数
        /// </summary>
        public int RecordCount { get; set; }

        /// <summary>
        /// 当前页码
        /// </summary>
        public int PageIndex { get; set; }

        /// <summary>
        /// 每页条数
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// 数据列表
        /// </summary>
        public IEnumerable<T> Body { get; set; }
    }
}
