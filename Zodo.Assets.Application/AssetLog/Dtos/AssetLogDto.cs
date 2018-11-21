using HZC.Infrastructure;
using Newtonsoft.Json;
using System;

namespace Zodo.Assets.Application
{
    public partial class AssetLogDto
    {
        public int Id { get; set; }

        public int AssetId { get; set; }

        public string AssetCode { get; set; }

        public string AssetName { get; set; }

        public string Type { get; set; }

        [JsonConverter(typeof(DateFormatConverter))]
        public DateTime OperateAt { get; set; }

        public int FromDeptId { get; set; }

        public int FromAccountId { get; set; }

        public string FromDeptName { get; set; }

        public string FromAccountName { get; set; }

        public int TargetDeptId { get; set; }

        public int TargetAccountId { get; set; }

        public string TargetDeptName { get; set; }

        public string TargetAccountName { get; set; }

        public string Pics { get; set; }

        public string Remark { get; set; }

    }
}
