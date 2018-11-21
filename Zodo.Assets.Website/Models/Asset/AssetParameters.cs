namespace Zodo.Assets.Website.Models
{
    public class AssetParameters
    {
        public static string[] States
        {
            get
            {
                return new string[] { "使用中", "闲置", "借出", "故障", "维修", "租赁", "报废" };
            }
        }

        public static string[] Healthy
        {
            get
            {
                return new string[] { "全新", "良好", "一般", "较差" };
            }
        }
    }
}
