using HZC.Database;
using HZC.Infrastructure;
using Newtonsoft.Json;
using System;

namespace Zodo.Assets.Core
{
    [MyDataTable("Asset_StockItem")]
    public partial class StockItem : Entity
    {
        /// <summary>
        /// 盘点Id
        /// </summary>
        public int StockId { get; set; }

        /// <summary>
        /// 是否结束
        /// </summary>
        public bool IsFinish { get; set; }

        /// <summary>
        /// 资产id
        /// </summary>
        public int AssetId { get; set; }

        /// <summary>
        /// 资产编号
        /// </summary>
        public string AssetCode { get; set; }

        /// <summary>
        /// 财务编号
        /// </summary>
        public string FinancialCode { get; set; }

        /// <summary>
        /// 健康度
        /// </summary>
        public string Healthy { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public string State { get; set; }

        /// <summary>
        /// 资产名称
        /// </summary>
        public string AssetName { get; set; }

        /// <summary>
        /// 部门id
        /// </summary>
        public int DeptId { get; set; }

        /// <summary>
        /// 部门名称
        /// </summary>
        public string DeptName { get; set; }

        /// <summary>
        /// 用户id
        /// </summary>
        public int AccountId { get; set; }

        /// <summary>
        /// 用户名称
        /// </summary>
        public string AccountName { get; set; }

        /// <summary>
        /// 资产位置
        /// </summary>
        public string Position { get; set; }

        /// <summary>
        /// 盘点日期
        /// </summary>
        [JsonConverter(typeof(DateTimeFormatConverter))]
        public DateTime? CheckAt { get; set; }

        /// <summary>
        /// 盘点人
        /// </summary>
        public int? CheckBy { get; set; }

        /// <summary>
        /// 盘点人姓名
        /// </summary>
        public string Checkor { get; set; }

        /// <summary>
        /// 是否已盘点
        /// </summary>
        public int CheckResult { get; set; } = 0;

        /// <summary>
        /// 盘点方式
        /// </summary>
        public int CheckMethod { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

    }
}
