using System.Collections.Generic;

namespace Zodo.Assets.Application
{
    public class MenuTreeDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Icon { get; set; }

        public string Url { get; set; }

        public int ParentId { get; set; }

        public int Level { get; set; }

        public int Sort { get; set; }

        public bool IsTheaf { get; set; }

        public IEnumerable<MenuTreeDto> Children { get; set; }
    }
}
