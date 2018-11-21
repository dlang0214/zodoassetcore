using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            DataItemService service = new DataItemService();
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
            return All().Where(a => a.K == key).SingleOrDefault();
        }

        public static string GetValue(string key)
        {
            var di = Load(key);
            if (di == null)
            {
                return string.Empty;
            }
            else
            {
                return di.V;
            }
        }

        public static string[] GetValues(string key)
        {
            var str = GetValue(key);
            if (string.IsNullOrWhiteSpace(str))
            {
                return new string[] { };
            }
            else
            {
                return str.Split('|');
            }
        }
    }
}
