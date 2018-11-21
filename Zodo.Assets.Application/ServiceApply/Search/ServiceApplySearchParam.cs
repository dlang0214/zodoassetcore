using HZC.SearchUtil;
using System;

namespace Zodo.Assets.Application
{
    public class ServiceApplySearchParam : ISearchParam
    {
        public string Key { get; set; }

        public string AssetCode { get; set; }

        public string State { get; set; }

        public string Type { get; set; }

        public string UserId { get; set; }

        public DateTime? ApplyAtStart { get; set; }

        public DateTime? ApplyAtEnd { get; set; }

        public DateTime? CompleteAtStart { get; set; }

        public DateTime? CompleteAtEnd { get; set; }

        public string ServiceMan { get; set; }

        public string Score { get; set; }

        public MySearchUtil ToSearchUtil()
        {
            MySearchUtil util = MySearchUtil.New().OrderByDesc("CreateAt");

            if (!string.IsNullOrWhiteSpace(Key))
            {
                util.AndContains(new string[] { "DeptName", "UserName", "Describe", "ServiceManId", "ServiceManName", "AssetCode" }, Key.Trim());
            }

            if (!string.IsNullOrWhiteSpace(AssetCode))
            {
                util.AndEqual("AssetCode", AssetCode.Trim());
            }

            if (!string.IsNullOrEmpty(Type))
            {
                util.AndEqual("Type", Type.Trim());
            }

            if (!string.IsNullOrWhiteSpace(State))
            {
                util.AndEqual("State", State.Trim());
            }

            if (!string.IsNullOrWhiteSpace(Score))
            {
                util.AndEqual("Score", Score.Trim());
            }

            if (ApplyAtStart.HasValue)
            {
                util.AndGreaterThanEqual("ApplyAt", ApplyAtStart);
            }

            if (ApplyAtEnd.HasValue)
            {
                util.AndLessThanEqual("ApplyAt", ApplyAtEnd);
            }

            if (CompleteAtStart.HasValue)
            {
                util.AndGreaterThanEqual("CompleteAt", ApplyAtStart);
            }

            if (CompleteAtEnd.HasValue)
            {
                util.AndLessThanEqual("CompleteAt", CompleteAtEnd);
            }

            if (!string.IsNullOrWhiteSpace(ServiceMan))
            {
                util.AndEqual("ServiceManName", ServiceMan);
            }

            if (!string.IsNullOrWhiteSpace(UserId))
            {
                util.AndEqual("UserId", UserId.Trim());
            }

            return util;
        }
    }
}
