namespace Zodo.Assets.Application
{
    /// <summary>
    /// 部门缓存实体
    /// </summary>
    public class DeptDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int ParentId { get; set; }

        public int Sort { get; set; }

        public int Level { get; set; }

        public string LevelPath { get; set; }
    }
}
