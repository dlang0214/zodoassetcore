namespace HZC.Database
{
    /// <summary>
    /// 实体属性信息
    /// </summary>
    internal class MyEntityPropertyInfo
    {
        /// <summary>
        /// 实体属性名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 实体属性对应的列名
        /// </summary>
        public string ColumnName { get; set; }

        /// <summary>
        /// 是否是主键
        /// </summary>
        public bool IsPrimaryKey { get; set; } = false;

        /// <summary>
        /// 插入时忽略
        /// </summary>
        public bool InsertIgnore { get; set; } = false;

        /// <summary>
        /// 更新时忽略
        /// </summary>
        public bool UpdateIgnore { get; set; } = false;

        /// <summary>
        /// 插入和更新时都忽略
        /// </summary>
        public bool Ignore { get; set; } = false;
    }
}
