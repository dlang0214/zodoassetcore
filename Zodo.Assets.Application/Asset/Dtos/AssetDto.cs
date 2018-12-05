using HZC.Infrastructure;
using Newtonsoft.Json;
using System;

namespace Zodo.Assets.Application
{
    public class AssetDto
    {
        public int Id { get; set; }

        public int AssetCateId { get; set; }

        public string AssetCate { get; set; }

        public string FinancialCode { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public string Cover { get; set; }

        public string Pics { get; set; }

        public string Band { get; set; }

        public string Model { get; set; }

        public string Imei { get; set; }

        public string Specification { get; set; }

        public string Position { get; set; }

        [JsonConverter(typeof(DateFormatConverter))]
        public DateTime? AssignDate { get; set; }

        public string State { get; set; }

        public string Healthy { get; set; }

        public string Source { get; set; }

        [JsonConverter(typeof(DateTimeFormatConverter))]
        public DateTime? LastCheckTime { get; set; }

        public int DeptId { get; set; }

        public string DeptName { get; set; }

        public int AccountId { get; set; }

        public string AccountName { get; set; }

        public string Remark { get; set; }

        public decimal Price { get; set; }
    }
}
