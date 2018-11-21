using HZC.Database;
using HZC.Infrastructure;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Zodo.Assets.Core
{
    [MyDataTable("Asset_Stock")]
    public class Stock : Entity
    {
        /// <summary>
        /// 盘点标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 盘点是否完成
        /// </summary>
        [MyDataField(UpdateIgnore = true)]
        public bool IsFinish { get; set; } = false;

        /// <summary>
        /// 盘点完成时间
        /// </summary>
        [JsonConverter(typeof(DateFormatConverter))]
        public DateTime? FinishAt { get; set; }

        [MyDataField(Ignore = true)]
        public List<StockItem> Items { get; set; }
    }
}
