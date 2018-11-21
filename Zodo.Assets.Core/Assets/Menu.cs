using HZC.Database;

namespace Zodo.Assets.Core
{
    [MyDataTable("Base_Menu")]
    public class Menu : Entity
    {
        public string Name { get; set; }

        public string Icon { get; set; }

        public string Url { get; set; }

        public int ParentId { get; set; }

        public int Sort { get; set; }
    }
}
