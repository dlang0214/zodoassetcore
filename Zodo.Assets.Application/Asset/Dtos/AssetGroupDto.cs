using System.Collections.Generic;

namespace Zodo.Assets.Application
{
    public class AssetGroupDto
    {
        public string GroupName { get; set; }

        public List<AssetDto> Assets { get; set; }
    }
}
