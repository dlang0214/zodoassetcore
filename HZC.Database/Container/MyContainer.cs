using System;
using System.Collections.Concurrent;

namespace HZC.Database
{
    /// <summary>
    /// 数据实体的容器
    /// </summary>
    internal class MyContainer
    {
        /// <summary>
        /// 实体及实体信息的字典
        /// </summary>
        private static ConcurrentDictionary<string, MyEntityInfo> _dict = new ConcurrentDictionary<string, MyEntityInfo>();

        #region 公共方法
        public static MyEntityInfo Get(Type type)
        {
            if (_dict.TryGetValue(type.Name, out var result)) return result;

            result = new MyEntityInfo(type);
            _dict.TryAdd(type.Name, result);
            return result;
        }
        #endregion
    }
}
