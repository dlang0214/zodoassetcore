using HZC.SearchUtil;
using System;

namespace Zodo.Assets.Application
{
    public partial class LoanSearchParam : ISearchParam
    {
        public string Key { get; set; }

        public DateTime? ExpectedReturnDateStart { get; set; }

        public DateTime? ExpectedReturnDateEnd { get; set; }

        public DateTime? LoanAtStart { get; set; }

        public DateTime? LoanAtEnd { get; set; }

        public DateTime? ReturnAtStart { get; set; }

        public DateTime? ReturnAtEnd { get; set; }

        public string From { get; set; }

        public string Target { get; set; }

        /// <summary>
        /// 组合查询
        /// 0 正常， 1 未归还 2 已归还 3 逾期未归还， 4 正常归还 5 逾期归还
        /// </summary>
        public int State { get; set; }

        public MySearchUtil ToSearchUtil()
        {
            MySearchUtil util = MySearchUtil.New().OrderByDesc("UpdateAt");

            if (!string.IsNullOrWhiteSpace(Key))
            {
                util.AndContains(new string[] { "AssetName", "AssetCode" }, Key.Trim());
            }

            if (!string.IsNullOrWhiteSpace(From))
            {
                util.AndContains(new string[] { "FromDeptName", "FromAccountName" }, From.Trim());
            }

            if (!string.IsNullOrWhiteSpace(Target))
            {
                util.AndContains(new string[] { "TargetDeptName", "TargetAccountName" }, Target.Trim());
            }

            if (ExpectedReturnDateStart != null)
            {
                util.AndGreaterThanEqual("ExpectedReturnAt", (DateTime)ExpectedReturnDateStart);
            }

            if (ExpectedReturnDateEnd != null)
            {
                util.AndLessThanEqual("ExpectedReturnAt", (DateTime)ExpectedReturnDateEnd);
            }

            if (LoanAtStart != null)
            {
                util.AndGreaterThanEqual("LoanAt", (DateTime)LoanAtStart);
            }

            if (LoanAtEnd != null)
            {
                util.AndLessThanEqual("LoanAt", (DateTime)LoanAtEnd);
            }

            if (ReturnAtStart != null)
            {
                util.AndGreaterThanEqual("ReturnAt", (DateTime)ReturnAtStart);
            }

            if (ReturnAtEnd != null)
            {
                util.AndLessThanEqual("ReturnAt", (DateTime)ReturnAtEnd);
            }

            // 0 正常， 1 未归还 2 已归还 3 逾期未归还， 4 正常归还 5 逾期归还
            switch (State)
            {
                case 1:
                    util.AndEqual("IsReturn", false);
                    break;
                case 2:
                    util.AndEqual("IsReturn", true);
                    break;
                case 3:
                    util.AndEqual("IsReturn", false).AndLessThan("ExpectedReturnAt", DateTime.Today);
                    break;
                case 4:
                    util.AndEqual("IsReturn", true).And("ReturnAt<=ExpectedReturnAt");
                    break;
                case 5:
                    util.AndEqual("IsReturn", true).And("ReturnAt>ExpectedReturnAt");
                    break;
                default:
                    break;
            }

            return util;
        }

        protected string GetFullColumnName(string columnName)
        {
            return "Asset_Loan." + columnName;
        }
    }
}
