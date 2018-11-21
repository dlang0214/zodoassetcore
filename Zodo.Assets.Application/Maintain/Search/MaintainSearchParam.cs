using HZC.SearchUtil;
using System;

namespace Zodo.Assets.Application
{
    public partial class MaintainSearchParam : ISearchParam
    {
        public string Key { get; set; }

        public int? AssetId { get; set; }

        public string AssetCode { get; set; }

        public string RepairMan { get; set; }

        public string ServiceMan { get; set; }

        public DateTime? RepairAtStart { get; set; }

        public DateTime? RepairAtEnd { get; set; }

        public string ServiceResult { get; set; }

        public string Satisfaction { get; set; }

        public MySearchUtil ToSearchUtil()
        {
            MySearchUtil util = MySearchUtil.New().OrderByDesc("UpdateAt");

            if (!string.IsNullOrWhiteSpace(Key))
            {
                util.AndContains(new string[] { "AssetCode", "AssetName", "DeptName", "AccountName", "Position", "Describe", "ServiceManName" }, Key.Trim());
            }

            if (AssetId.HasValue)
            {
                util.AndEqual("AssetId", AssetId);
            }

            if (!string.IsNullOrWhiteSpace(RepairMan))
            {
                util.AndContains("RepairManName", RepairMan);
            }

            if (!string.IsNullOrWhiteSpace(ServiceMan))
            {
                util.AndEqual("ServiceManName", ServiceMan);
            }

            if (!string.IsNullOrWhiteSpace(ServiceResult))
            {
                util.AndEqual("ServiceResult", ServiceResult);
            }

            var dt = DateTime.Parse("1900-1-1");

            if (RepairAtStart.HasValue && RepairAtStart >= dt)
            {
                util.AndGreaterThanEqual("RepairAt", RepairAtStart);
            }

            if (RepairAtEnd.HasValue && RepairAtEnd >= dt)
            {
                util.AndLessThanEqual("RepairAt", RepairAtEnd);
            }

            return util;
        }
    }
}
