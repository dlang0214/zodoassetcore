using HZC.Infrastructure;
using Newtonsoft.Json;
using System;

namespace Zodo.Assets.Application
{
    public partial class MaintainDto
    {
        public int Id { get; set; }

        public string AssetCode { get; set; }

        public string AssetName { get; set; }

        public string Band { get; set; }

        public string Model { get; set; }

        public string Imei { get; set; }

        public string DeptId { get; set; }

        public string DeptName { get; set; }

        public string AccountId { get; set; }

        public string AccountName { get; set; }

        public string Position { get; set; }

        public string RepairMan { get; set; }

        public string RepairManName { get; set; }

        [JsonConverter(typeof(DateFormatConverter))]
        public DateTime? RepairAt { get; set; }

        public string ServiceMan { get; set; }

        public string ServiceManName { get; set; }

        public string Describe { get; set; }

        [JsonConverter(typeof(DateFormatConverter))]
        public DateTime? ServiceStartAt { get; set; }

        [JsonConverter(typeof(DateFormatConverter))]
        public DateTime? ServiceFinishAt { get; set; }

        public string ServiceResult { get; set; }

        public decimal ServicePrice { get; set; }

        public string Satisfaction { get; set; }

        public string Remark { get; set; }

    }
}
