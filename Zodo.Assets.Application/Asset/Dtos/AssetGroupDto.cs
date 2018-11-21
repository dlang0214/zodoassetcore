using System;
using System.Collections.Generic;
using System.Text;

namespace Zodo.Assets.Application
{
    public class AssetGroupDto
    {
        public string GroupName { get; set; }

        public List<AssetDto> Assets { get; set; }
    }
}
