using System.Collections.Generic;

namespace Zodo.Assets.Application
{
    /// <summary>
    /// 指定类型的树模型
    /// </summary>
    /// <typeparam name="T">主键类型</typeparam>
    public interface ITreeModel<TEntity, T>
    {
        T ParentId { get; set; }

        /// <summary>
        /// 级别
        /// </summary>
        int Level { get; set; }

        /// <summary>
        /// 是否叶子节点
        /// </summary>
        bool IsLeaf { get; set; }

        /// <summary>
        /// 下属节点
        /// </summary>
        List<TEntity> Children { get; set; }
    }

    public interface ITreeModel<T> : ITreeModel<T, int>
    { }
}
