using System.Collections.Generic;
using System.Linq;
using Zodo.Assets.Core;

namespace Zodo.Assets.Application
{
    public class DataItemUtil
    {
        private static List<DataItem> _items;

        public static void Clear()
        {
            _items = null;
        }

        private static void Init()
        {
            var service = new DataItemService();
            _items = service.Fetch().ToList();
        }

        public static List<DataItem> All()
        {
            if (_items == null)
            {
                Init();
            }
            return _items;
        }

        public static DataItem Load(string key)
        {
            return All().SingleOrDefault(a => a.K == key);
        }

        public static string GetValue(string key)
        {
            var di = Load(key);
            return di == null ? string.Empty : di.V;
        }

        public static string[] GetValues(string key)
        {
            var str = GetValue(key);
            return string.IsNullOrWhiteSpace(str) ? new string[] { } : str.Split('|');
        }
    }
}
