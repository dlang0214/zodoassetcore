using System;
using System.Collections.Generic;
using System.Text;

namespace Zodo.Assets.Application
{
    public class MenuDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Icon { get; set; }

        public string Url { get; set; }

        public int ParentId { get; set; }

        public int Level { get; set; }

        public int Sort { get; set; }
    }
}
