using System.Collections.Generic;

namespace Zodo.Assets.Application
{
    public class AssetCateTreeDto : AssetCateDto, ITreeModel<AssetCateTreeDto>
    {
        public bool IsLeaf { get; set; }

        public List<AssetCateTreeDto> Children { get; set; }
    }
}
