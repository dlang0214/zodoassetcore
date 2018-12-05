using HZC.SearchUtil;
using System;

namespace Zodo.Assets.Application
{
    public class AssetLogSearchParam : ISearchParam
    {
        public string Key { get; set; }

        public int AssetId { get; set; }

        public string From { get; set; }

        public string Target { get; set; }

        public DateTime? OperateAtStart { get; set; }

        public DateTime? OperateAtEnd { get; set; }

        public string Type { get; set; }

        public MySearchUtil ToSearchUtil()
        {
            var util = MySearchUtil.New().OrderByDesc("UpdateAt");

            if (!string.IsNullOrWhiteSpace(Key))
            {
                util.AndContains(new[] { "AssetCode", "AssetName" }, Key.Trim());
            }

            if (!string.IsNullOrWhiteSpace(From))
            {
                util.AndContains(new[] { "FromAccountName", "FromDeptName" }, From.Trim());
            }

            if (!string.IsNullOrWhiteSpace(Target))
            {
                util.AndContains(new[] { "TargetAccountName", "TargetDeptName" }, Target.Trim());
            }

            if (!string.IsNullOrWhiteSpace(Type))
            {
                util.AndEqual("Type", Type.Trim());
            }

            if (OperateAtStart != null)
            {
                util.AndGreaterThanEqual("OperateAt", (DateTime)OperateAtStart);
            }

            if (OperateAtEnd != null)
            {
                util.AndLessThanEqual("OperateAt", (DateTime)OperateAtEnd);
            }

            if (AssetId > 0)
            {
                util.AndEqual("AssetId", AssetId);
            }

            return util;
        }
    }
}
