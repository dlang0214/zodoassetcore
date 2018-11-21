using HZC.Database;
using System;
using System.Collections.Generic;
using System.Text;

namespace Zodo.Assets.Core
{
    [MyDataTable("Base_DataItem")]
    public class DataItem : Entity
    {
        [MyDataField(UpdateIgnore = true)]
        public string K { get; set; }

        public string V { get; set; } = "";
    }
}
