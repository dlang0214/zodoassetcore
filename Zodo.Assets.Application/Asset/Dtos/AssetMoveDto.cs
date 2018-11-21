using System;
using System.Collections.Generic;
using System.Text;

namespace Zodo.Assets.Application
{
    public class AssetMoveDto
    {
        public int AssetId { get; set; }

        public int FromDetpId { get; set; }

        public string FromDeptName { get; set; }

        public int FromAccountId { get; set; }

        public string FromAccountName { get; set; }

        public int TargetDeptId { get; set; }
    }
}
