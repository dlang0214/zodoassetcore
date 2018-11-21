using HZC.Database;
using Newtonsoft.Json;
using System;
using HZC.Infrastructure;

namespace Zodo.Assets.Core
{
    [MyDataTable("Asset_Maintain")]
    public partial class Maintain : Entity
    {
        /// <summary>
        /// 资产ID
        /// </summary>
        public int? AssetId { get; set; }

        /// <summary>
        /// 资产编号
        /// </summary>
        public string AssetCode { get; set; }

        /// <summary>
        /// 资产编号
        /// </summary>
        public string AssetName { get; set; }

        /// <summary>
        /// 品牌
        /// </summary>
        public string Band { get; set; }

        /// <summary>
        /// 资产型号
        /// </summary>
        public string Model { get; set; }

        /// <summary>
        /// 资产序列号
        /// </summary>
        public string Imei { get; set; }

        /// <summary>
        /// 维修前的状态
        /// </summary>
        public string OrigState { get; set; }

        /// <summary>
        /// 部门
        /// </summary>
        public int? DeptId { get; set; }

        /// <summary>
        /// 部门名称
        /// </summary>
        public string DeptName { get; set; }

        /// <summary>
        /// 使用人
        /// </summary>
        public int? AccountId { get; set; }

        /// <summary>
        /// 使用人姓名
        /// </summary>
        public string AccountName { get; set; }

        /// <summary>
        /// 位置
        /// </summary>
        public string Position { get; set; }

        /// <summary>
        /// 申请人
        /// </summary>
        public string RepairMan { get; set; }

        /// <summary>
        /// 申请人姓名
        /// </summary>
        public string RepairManName { get; set; }

        /// <summary>
        /// 申请时间
        /// </summary>
        [JsonConverter(typeof(DateFormatConverter))]
        public DateTime RepairAt { get; set; }

        /// <summary>
        /// 维修人
        /// </summary>
        public string ServiceMan { get; set; }

        /// <summary>
        /// 维修人姓名
        /// </summary>
        public string ServiceManName { get; set; }

        /// <summary>
        /// 问题描述
        /// </summary>
        public string Describe { get; set; }

        /// <summary>
        /// 服务类型
        /// </summary>
        public string ServiceType { get; set; }

        /// <summary>
        /// 期望服务时间
        /// </summary>
        public DateTime? ExpectedServiceTime { get; set; }

        /// <summary>
        /// 维修开始时间
        /// </summary>
        [JsonConverter(typeof(DateTimeFormatConverter))]
        public DateTime? ServiceStartAt { get; set; }

        /// <summary>
        /// 维修结束时间
        /// </summary>
        [JsonConverter(typeof(DateTimeFormatConverter))]
        public DateTime? ServiceFinishAt { get; set; }

        /// <summary>
        /// 维修结果
        /// </summary>
        public string ServiceResult { get; set; }

        /// <summary>
        /// 维修费用
        /// </summary>
        public decimal ServicePrice { get; set; }

        /// <summary>
        /// 满意度
        /// </summary>
        public string Satisfaction { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Remark { get; set; }

    }
}
