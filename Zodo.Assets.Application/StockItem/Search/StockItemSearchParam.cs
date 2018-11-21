using HZC.SearchUtil;
using System;
using Zodo.Assets.Application;

namespace Zodo.Assets.Services
{
    public partial class StockItemSearchParam : ISearchParam
    {
        public string Key { get; set; }

        public int StockId { get; set; }

        public int? DeptId { get; set; }

        public int? CheckMethod { get; set; }

        public int? CheckResult { get; set; }

        public string Checkor { get; set; }

        public bool? IsFinish { get; set; }

        public int? OrderBy { get; set; }

        public MySearchUtil ToSearchUtil()
        {
            MySearchUtil util = MySearchUtil.New().AndEqual("IsDel", false);

            if (StockId > 0)
            {
                util.AndEqual("StockId", StockId);
            }

            if (CheckMethod.HasValue)
            {
                util.AndEqual("CheckMethod", CheckMethod.Value);
            }

            if (DeptId.HasValue)
            {
                util.AndEqual("DeptId", DeptId.Value);
            }

            if (CheckResult.HasValue)
            {
                util.AndEqual("CheckResult", CheckResult.Value);
            }

            if (IsFinish.HasValue)
            {
                util.AndEqual("IsFinish", IsFinish.HasValue);
            }

            if (!string.IsNullOrWhiteSpace(Checkor))
            {
                util.AndContains("Checkor", Checkor);
            }

            if (!string.IsNullOrWhiteSpace(Key))
            {
                util.AndContains(new string[] { "AssetCode", "FinancialCode", "AssetName", "DeptName", "AccountName" }, Key.Trim());
            }

            if (OrderBy.HasValue)
            {
                switch (OrderBy.Value)
                {
                    case 1:
                        util.OrderBy("AssetCode");
                        break;
                    case 2:
                        util.OrderBy("DeptName");
                        break;
                    case 3:
                        util.OrderBy("AccountName");
                        break;
                    case 4:
                        util.OrderByDesc("CreateAt");
                        break;
                    default:
                        util.OrderByDesc("Id");
                        break;
                }
            }
            else
            {
                util.OrderByDesc("Id");
            }

            return util;
        }
    }
}
