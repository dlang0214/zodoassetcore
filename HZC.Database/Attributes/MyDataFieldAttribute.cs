using System;

namespace HZC.Database
{
    /// <summary>
    /// 数据实体-数据列配置
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class MyDataFieldAttribute : Attribute
    {
        public MyDataFieldAttribute()
        { }

        /// <summary>
        /// 属性列对应的数据库字段名
        /// </summary>
        public string ColumnName { get; set; } = "";

        /// <summary>
        /// 是否是主键
        /// </summary>
        public bool IsPrimaryKey { get; set; } = false;

        /// <summary>
        /// Insert操作时忽略此属性
        /// </summary>
        public bool InsertIgnore { get; set; } = false;

        /// <summary>
        /// Update操作时忽略此属性
        /// </summary>
        public bool UpdateIgnore { get; set; } = false;

        /// <summary>
        /// Inser和Update时忽略此树形
        /// </summary>
        public bool Ignore { get; set; } = false;

        /// <summary>
        /// 序列化时忽略此属性
        /// </summary>
        public bool NotMap { get; set; } = false;
    }
}
