using HZC.Database;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using HZC.Infrastructure;

namespace Zodo.Assets.Core
{
    [MyDataTable("Asset_Asset")]
    public class Asset : Entity
    {
        /// <summary>
        /// 资产类型
        /// </summary>
        public int AssetCateId { get; set; }

        /// <summary>
        /// 财务编号
        /// </summary>
        public string FinancialCode { get; set; }

        /// <summary>
        /// 资产编号
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 资产名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 资产图片
        /// </summary>
        public string Cover { get; set; }

        /// <summary>
        /// 购入金额
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// 资产图片
        /// </summary>
        public string Pics { get; set; }

        /// <summary>
        /// 品牌
        /// </summary>
        public string Band { get; set; }

        /// <summary>
        /// 型号
        /// </summary>
        public string Model { get; set; }

        /// <summary>
        /// 序列号
        /// </summary>
        public string Imei { get; set; }

        /// <summary>
        /// 规格
        /// </summary>
        public string Specification { get; set; }

        /// <summary>
        /// 资产位置
        /// </summary>
        public string Position { get; set; }

        /// <summary>
        /// 登记时间
        /// </summary>
        [JsonConverter(typeof(DateFormatConverter))]
        public DateTime? AssignDate { get; set; }

        /// <summary>
        /// 资产状态
        /// </summary>
        public string State { get; set; }

        /// <summary>
        /// 资产健康度
        /// </summary>
        public string Healthy { get; set; }

        /// <summary>
        /// 采购来源
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// 上次盘点日期
        /// </summary>
        [MyDataField(UpdateIgnore = true)]
        [JsonConverter(typeof(DateTimeFormatConverter))]
        public DateTime? LastCheckTime { get; set; }

        /// <summary>
        /// 所在部门
        /// </summary>
        [MyDataField(UpdateIgnore = true)]
        public int DeptId { get; set; }

        /// <summary>
        /// 当前使用人
        /// </summary>
        [MyDataField(UpdateIgnore = true)]
        public int AccountId { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        #region 导航属性
        [MyDataField(Ignore = true)]
        public virtual AssetCate Cate { get; set; }

        [MyDataField(Ignore = true)]
        public virtual Dept Dept { get; set; }

        [MyDataField(Ignore = true)]
        public virtual Account Account { get; set; }

        [MyDataField(Ignore = true)]
        public virtual List<AssetLog> Logs { get; set; }
        #endregion  
    }
}
