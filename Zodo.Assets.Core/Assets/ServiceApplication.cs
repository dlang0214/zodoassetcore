using HZC.Database;
using System;
using Newtonsoft.Json;
using HZC.Infrastructure;

namespace Zodo.Assets.Core
{
    [MyDataTable("Asset_ServiceApplication")]
    public class ServiceApplication : Entity
    {
        [MyDataField(UpdateIgnore = true)]
        public int DeptId { get; set; }

        [MyDataField(UpdateIgnore = true)]
        public string DeptName { get; set; }

        [MyDataField(UpdateIgnore = true)]
        public string AccountId { get; set; }

        [MyDataField(UpdateIgnore = true)]
        public string AccountName { get; set; }

        [MyDataField(UpdateIgnore = true)]
        public string Type { get; set; }

        [MyDataField(UpdateIgnore = true)]
        public string Describe { get; set; }

        [MyDataField(UpdateIgnore = true)]
        [JsonConverter(typeof(DateFormatConverter))]
        public DateTime ApplyAt { get; set; }

        [MyDataField(UpdateIgnore = true)]
        [JsonConverter(typeof(DateFormatConverter))]
        public DateTime RequireCompleteAt { get; set; }

        public string Reply { get; set; }

        public DateTime? CompleteAt { get; set; }

        public string State { get; set; }
    }
}
