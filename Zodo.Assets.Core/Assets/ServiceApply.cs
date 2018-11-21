using HZC.Database;
using HZC.Infrastructure;
using Newtonsoft.Json;
using System;

namespace Zodo.Assets.Core
{
    [MyDataTable("Asset_ServiceApply")]
    public class ServiceApply
    {
        [MyDataField(IsPrimaryKey = true)]
        public int Id { get; set; }

        public int DeptId { get; set; }

        public string DeptName { get; set; }

        public string UserId { get; set; }

        public string UserName { get; set; }

        public string AssetCode { get; set; }

        public string Type { get; set; }

        public string Describe { get; set; }

        [JsonConverter(typeof(DateTimeFormatConverter))]
        public DateTime ApplyAt { get; set; } = DateTime.Now;

        [JsonConverter(typeof(DateTimeFormatConverter))]
        public DateTime? ReceiveAt { get; set; } = null;

        [JsonConverter(typeof(DateFormatConverter))]
        public DateTime? RequireCompleteAt { get; set; } = null;

        [JsonConverter(typeof(DateTimeFormatConverter))]
        public DateTime? ConfirmAt { get; set; }

        public string State { get; set; }

        public string Score { get; set; } = "未评价";

        public string Reply { get; set; }

        [JsonConverter(typeof(DateTimeFormatConverter))]
        public DateTime? CompleteAt { get; set; } = null;

        public string ServiceManId { get; set; }

        public string ServiceManName { get; set; }

        public string Reason { get; set; }

        public string Analysis { get; set; }

        public string Solution { get; set; }

        public string Remark { get; set; }

        public bool IsDel { get; set; } = false;
        
        public DateTime CreateAt { get; set; } = DateTime.Now;

        [MyDataField(Ignore = true)]
        public virtual Asset Asset { get; set; } = null;
    }
}
