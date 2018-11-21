using HZC.Database;
using System.Collections.Generic;

namespace Zodo.Assets.Core
{
    [MyDataTable("Base_Dept")]
    public class Dept : Entity
    {
        /// <summary>
        /// 部门名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 上级部门ID
        /// </summary>
        public int ParentId { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }
    }
}
