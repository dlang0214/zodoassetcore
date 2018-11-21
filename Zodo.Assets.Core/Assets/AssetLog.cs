using HZC.Database;
using HZC.Infrastructure;
using Newtonsoft.Json;
using System;

namespace Zodo.Assets.Core
{
    [MyDataTable("Asset_AssetLog")]
    public class AssetLog : Entity
    {
        /// <summary>
        /// 资产ID
        /// </summary>
        public int AssetId { get; set; }

        /// <summary>
        /// 资产编号
        /// </summary>
        public string AssetCode { get; set; }

        /// <summary>
        /// 资产名称
        /// </summary>
        public string AssetName { get; set; }

        /// <summary>
        /// 操作类型
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 原部门
        /// </summary>
        public int FromDeptId { get; set; }

        /// <summary>
        /// 原部门名称
        /// </summary>
        public string FromDeptName { get; set; }

        /// <summary>
        /// 原用户
        /// </summary>
        public int FromAccountId { get; set; }

        /// <summary>
        /// 原用户名称
        /// </summary>
        public string FromAccountName { get; set; }

        /// <summary>
        /// 目标部门
        /// </summary>
        public int TargetDeptId { get; set; }

        /// <summary>
        /// 目标部门名称
        /// </summary>
        public string TargetDeptName { get; set; }

        /// <summary>
        /// 目标用户
        /// </summary>
        public int TargetAccountId { get; set; }

        /// <summary>
        /// 目标用户名称
        /// </summary>
        public string TargetAccountName { get; set; }

        /// <summary>
        /// 过程图片
        /// </summary>
        public string Pics { get; set; }

        /// <summary>
        /// 调配日期
        /// </summary>
        [JsonConverter(typeof(DateFormatConverter))]
        public DateTime OperateAt { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }
}
