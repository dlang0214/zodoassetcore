using HZC.Database;
using System.Collections.Generic;

namespace Zodo.Assets.Core
{
    [MyDataTable("Asset_AssetCate")]
    public class AssetCate : Entity
    {
        /// <summary>
        /// 类型名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 上级类型
        /// </summary>
        public int ParentId { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }

        #region 导航属性
        [MyDataField(Ignore = true)]
        public virtual List<AssetCate> Children { get; set; }
        #endregion
    }
}
