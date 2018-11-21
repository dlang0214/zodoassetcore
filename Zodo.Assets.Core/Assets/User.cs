using HZC.Database;
using HZC.Infrastructure;

namespace Zodo.Assets.Core
{
    [MyDataTable("Base_User")]
    public class User : Entity
    {
        public string Name { get; set; }

        [MyDataField(UpdateIgnore = true)]
        public string Pw { get; set; }

        public string Version { get; set; }
    }
}
