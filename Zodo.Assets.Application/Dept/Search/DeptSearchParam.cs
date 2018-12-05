using HZC.SearchUtil;

namespace Zodo.Assets.Application
{
    public class DeptSearchParam : ISearchParam
    {
        public string Key { get; set; }

        public int Parent { get; set; }
        
        public MySearchUtil ToSearchUtil()
        {
            var util = MySearchUtil.New().AndEqual("IsDel", false);

            if (!string.IsNullOrWhiteSpace(Key))
            {
                util.AndContains("Name", Key.Trim());
            }

            if (Parent > 0)
            {
                util.AndEqual("ParentId", Parent);
            }

            return util;
        }
    }
}
