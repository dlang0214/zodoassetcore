using System;

namespace HZC.Database
{
    /// <summary>
    /// 数据实体-数据表配置
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class MyDataTableAttribute : Attribute
    {
        public MyDataTableAttribute() : this(string.Empty)
        { }
        
        public MyDataTableAttribute(string name)
        {
            Name = name;
        }

        /// <summary>
        /// 对应的数据表表名
        /// </summary>
        public string Name { get; set; }
    }
}
