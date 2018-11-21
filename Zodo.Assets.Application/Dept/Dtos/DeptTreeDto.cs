using System.Collections.Generic;

namespace Zodo.Assets.Application
{
    /// <summary>
    /// 部门树形实体
    /// </summary>
    public class DeptTreeDto : ITreeModel<DeptTreeDto>
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int Sort { get; set; }

        public int Level { get; set; }

        public bool IsLeaf { get; set; }

        public int ParentId { get; set; }

        public List<DeptTreeDto> Children { get; set; }
    }
}
