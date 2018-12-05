using System.Collections.Generic;
using System.Linq;

namespace Zodo.Assets.Application
{
    public class MenuUtil
    {
        public static List<MenuDto> Menus { get; set; }
        public static List<MenuTreeDto> TreeList { get; set; }

        private static void Init()
        {
            var service = new MenuService();
            var menus = service.FetchDto();

            Menus = new List<MenuDto>();

            Dg(menus);

            void Dg(IReadOnlyCollection<MenuDto> source, int parent = 0, int level = 1)
            {
                var root = source.Where(s => s.ParentId == parent).OrderBy(s => s.Sort);
                foreach (var r in root)
                {
                    r.Level = level;
                    Menus.Add(r);

                    var hasChildren = source.Any(s => s.ParentId == r.Id);
                    if (!hasChildren) continue;
                    level++;
                    Dg(source, r.Id, level);
                    level--;
                }
            }
        }

        public static void Clear()
        {
            Menus = null;
            TreeList = null;
        }

        public static List<MenuDto> All()
        {
            if (Menus == null)
            {
                Init();
            }
            return Menus;
        }

        public static List<MenuTreeDto> Tree()
        {
            if (TreeList == null)
            {
                SetTree();
            }
            return TreeList;
        }

        private static void SetTree()
        {
            var all = All();
            TreeList = Dg(all);

            List<MenuTreeDto> Dg(IReadOnlyCollection<MenuDto> source, int parentId = 0, int level = 1)
            {
                var result = new List<MenuTreeDto>();
                var root = source.Where(s => s.ParentId == parentId).OrderBy(s => s.Sort);

                foreach (var r in root)
                {
                    var node = new MenuTreeDto
                    {
                        Id = r.Id,
                        Name = r.Name,
                        ParentId = r.ParentId,
                        Icon = r.Icon,
                        Url = r.Url,
                        Sort = r.Sort,
                        Level = level
                    };

                    result.Add(node);

                    var hasChildren = source.Any(s => s.ParentId == r.Id);
                    if (hasChildren)
                    {
                        node.IsTheaf = false;
                        level++;
                        node.Children = Dg(source, r.Id, level);
                        level--;
                    }
                    else
                    {
                        node.IsTheaf = true;
                        node.Children = new List<MenuTreeDto>();
                    }
                }
                return result;
            }
        }
    }
}
