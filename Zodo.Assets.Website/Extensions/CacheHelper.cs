using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System.Text;

namespace Zodo.Assets.Website.Extensions
{
    public class CacheHelper
    {
        private readonly IDistributedCache _cache;

        public CacheHelper(IDistributedCache cache)
        {
            _cache = cache;
        }

        public T Get<T>(string key)
        {
            var str = Get(key);
            return string.IsNullOrWhiteSpace(str) ? default(T) : JsonConvert.DeserializeObject<T>(str);
        }

        public bool Set<T>(string key, T val)
        {
            var v = JsonConvert.SerializeObject(val);
            Set(key, v);
            return true;
        }

        public bool Remove(string key)
        {
            _cache.Remove(key);
            return true;
        }

        public bool Exist(string key)
        {
            var val = _cache.Get(key);
            return !(val == null || val.Length == 0);
        }

        public string Get(string key)
        {
            var val = _cache.Get(key);
            if (val == null || val.Length == 0)
            {
                return string.Empty;
            }

            return Encoding.UTF8.GetString(val);
        }

        public bool Set(string key, string val)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return false;
            }
            _cache.Set(key, Encoding.UTF8.GetBytes(val));
            return true;
        }
    }
}
