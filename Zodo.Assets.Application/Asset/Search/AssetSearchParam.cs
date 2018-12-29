using HZC.SearchUtil;

namespace Zodo.Assets.Application
{
    public class AssetSearchParam : ISearchParam
    {
        public string Key { get; set; }

        public string State { get; set; }

        public string Position { get; set; }

        public bool IncludeScrap { get; set; } = false;

        public int? CateId { get; set; }

        public int? DeptId { get; set; }

        public int? AccountId { get; set; }

        public int? Sort { get; set; }

        public string Healthy { get; set; }

        public bool IsContainSubDept { get; set; } = false;

        public MySearchUtil ToSearchUtil()
        {
            var util = MySearchUtil.New().AndEqual("IsDel", false);

            if (!string.IsNullOrWhiteSpace(Key))
            {
                util.AndContains(new[] { "Name", "Code", "FinancialCode", "Band", "Imei", "Model", "Source", "Remark", "Position", "DeptName", "AccountName" }, Key.Trim());
            }

            if (!string.IsNullOrWhiteSpace(Position))
            {
                util.AndContains("Position", Position.Trim());
            }

            if (!string.IsNullOrWhiteSpace(State))
            {
                util.AndEqual("State", State.Trim());
            }

            if (DeptId.HasValue)
            {
                if (!IsContainSubDept)
                {
                    util.AndEqual("DeptId", (int)DeptId);
                }
                else
                {
                    var depts = DeptUtil.GetSelfAndChildrenIds((int)DeptId);
                    util.AndIn("DeptId", depts);
                }
            }

            if (CateId.HasValue && CateId > 0)
            {
                var cate = AssetCateUtil.Get((int)CateId);
                if (cate == null)
                {
                    util.And("1=2");
                }
                else
                {
                    var ids = AssetCateUtil.GetSelfAndChildrenIds(cate.Id);
                    util.AndIn("AssetCateId", ids);
                }
            }

            if (AccountId.HasValue && AccountId > 0)
            {
                util.AndEqual("AccountId", (int)AccountId);
            }

            if(!IncludeScrap)
            {
                util.AndNotEqual("State", "报废");
            }

            if (!string.IsNullOrWhiteSpace(Healthy))
            {
                util.AndEqual("Healthy", Healthy);
            }

            if (Sort.HasValue)
            {
                switch (Sort)
                {
                    case 1:
                        util.OrderBy("Code");
                        break;
                    case 2:
                        util.OrderBy("Name");
                        break;
                    case 3:
                        util.OrderBy("DeptName");
                        break;
                    case 4:
                        util.OrderBy("AccountName");
                        break;
                    default:
                        util.OrderByDesc("UpdateAt");
                        break;
                }
            }
            else
            {
                util.OrderByDesc("UpdateAt");
            }

            return util;
        }
    }
}
