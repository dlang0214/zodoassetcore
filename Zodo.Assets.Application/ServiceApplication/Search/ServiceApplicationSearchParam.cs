using HZC.SearchUtil;
using System;

namespace Zodo.Assets.Application
{
    public class ServiceApplicationSearchParam : ISearchParam
    {
        public string Key { get; set; }

        public int Dept { get; set; }

        public string Type { get; set; }

        public DateTime? ApplyAtStart { get; set; }

        public DateTime? ApplyAtEnd { get; set; }

        public DateTime? RequireCompleteAtStart { get; set; }

        public DateTime? RequireCompleteAtEnd { get; set; }

        public DateTime? CompleteAtStart { get; set; }

        public DateTime? CompleteAtEnd { get; set; }

        public string State { get; set; }

        public MySearchUtil ToSearchUtil()
        {
            var util = MySearchUtil.New().AndEqual("IsDel", false).OrderByDesc("CreateAt");
            if (!string.IsNullOrWhiteSpace(Key))
            {
                util.AndContains(new[] { "AccountName", "Describe" }, Key);
            }

            if (Dept > 0)
            {
                util.AndEqual("DeptId", Dept);
            }

            if (!string.IsNullOrWhiteSpace(Type))
            {
                util.AndEqual("Type", Type);
            }

            if (ApplyAtStart.HasValue)
            {
                util.AndGreaterThanEqual("ApplyAt", (DateTime)ApplyAtStart);
            }

            if (ApplyAtEnd.HasValue)
            {
                util.AndLessThanEqual("ApplyAt", (DateTime)ApplyAtEnd);
            }

            if (RequireCompleteAtStart.HasValue)
            {
                util.AndLessThanEqual("RequireCompleteAt", (DateTime)RequireCompleteAtStart);
            }

            if (RequireCompleteAtEnd.HasValue)
            {
                util.AndLessThanEqual("RequireCompleteAt", (DateTime)RequireCompleteAtEnd);
            }

            if (CompleteAtStart.HasValue)
            {
                util.AndGreaterThanEqual("CompleteAt", (DateTime)CompleteAtStart);
            }

            if (CompleteAtEnd.HasValue)
            {
                util.AndLessThanEqual("CompleteAt", (DateTime)CompleteAtEnd);
            }

            if (!string.IsNullOrWhiteSpace(State))
            {
                util.AndEqual("State", State);
            }

            return util;
        }
    }
}
