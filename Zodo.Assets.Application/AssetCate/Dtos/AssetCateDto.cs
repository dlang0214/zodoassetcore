using System.Collections.Generic;

namespace Zodo.Assets.Application
{
    public class AssetCateDto
    {
        public AssetCateDto()
        {
            LevelPath = new List<int>();
        }

        public int Id { get; set; }

        public string Name { get; set; }
        
        public int ParentId { get; set; }

        public int Level { get; set; }
        
        public int Sort { get; set; }

        public List<int> LevelPath { get; set; }
    }
}
