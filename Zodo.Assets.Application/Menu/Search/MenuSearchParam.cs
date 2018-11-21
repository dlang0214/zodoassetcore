using HZC.SearchUtil;

namespace Zodo.Assets.Application
{
    public class MenuSearchParam : ISearchParam
    {
        public MySearchUtil ToSearchUtil()
        {
            return MySearchUtil.New().AndEqual("IsDel", false);
        }
    }
}
