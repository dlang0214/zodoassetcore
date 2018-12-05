using HZC.SearchUtil;

namespace Zodo.Assets.Application
{
    public class UserSearchParam : ISearchParam
    {
        public string Key { get; set; }

        public MySearchUtil ToSearchUtil()
        {
            var util = MySearchUtil.New()
                .AndNotEqual("Name", "admin")
                .AndEqual("IsDel", false)
                .OrderBy("Id");
            if (!string.IsNullOrWhiteSpace(Key))
            {
                util.AndContains("Name", Key.Trim());
            }
            return util;
        }
    }
}
