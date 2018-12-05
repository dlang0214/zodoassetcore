using HZC.SearchUtil;

namespace Zodo.Assets.Application
{
    public class StockSearchParam : ISearchParam
    {
        public string Key { get; set; }

        public bool? IsFinish { get; set; }

        public MySearchUtil ToSearchUtil()
        {
            var util = MySearchUtil.New().AndEqual("IsDel", false);

            if (!string.IsNullOrWhiteSpace(Key))
            {
                util.AndContains("Title", Key.Trim());
            }

            if (IsFinish.HasValue)
            {
                util.AndEqual("IsFinish", (bool)IsFinish);
            }

            return util;
        }
    }
}
