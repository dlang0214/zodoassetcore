namespace Zodo.Assets.Application
{
    public class AssetCateDto
    {
        public int Id { get; set; }

        public string Name { get; set; }
        
        public int ParentId { get; set; }

        public int Level { get; set; }
        
        public int Sort { get; set; }
        
        public string LevelPath { get; set; }
    }
}
