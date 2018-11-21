using System.Collections.Generic;
using System.Linq;

namespace Zodo.Assets.Application
{
    public class MenuUtil
    {
        private static List<MenuDto> _menus = null;
        private static List<MenuTreeDto> _tree = null;

        private static void Init()
        {
            var service = new MenuService();
            var menus = service.FetchDto();

            _menus = new List<MenuDto>();

            Dg(menus);

            void Dg(List<MenuDto> source, int parent = 0, int level = 1)
            {
                var root = source.Where(s => s.ParentId == parent).OrderBy(s => s.Sort);
                foreach (var r in root)
                {
                    r.Level = level;
                    _menus.Add(r);

                    var hasChildren = source.Where(s => s.ParentId == r.Id).Any();
                    if (hasChildren)
                    {
                        level++;
                        Dg(source, r.Id, level);
                        level--;
                    }
                }
            }
        }

        public static void Clear()
        {
            _menus = null;
            _tree = null;
        }

        public static List<MenuDto> All()
        {
            if (_menus == null)
            {
                Init();
            }
            return _menus;
        }

        public static List<MenuTreeDto> Tree()
        {
            if (_tree == null)
            {
                SetTree();
            }
            return _tree;
        }

        private static void SetTree()
        {
            var all = All();
            _tree = Dg(all, 0);

            List<MenuTreeDto> Dg(List<MenuDto> source, int parentId = 0, int level = 1)
            {
                var result = new List<MenuTreeDto>();
                var root = source.Where(s => s.ParentId == parentId).OrderBy(s => s.Sort);

                foreach (var r in root)
                {
                    var node = new MenuTreeDto();
                    node.Id = r.Id;
                    node.Name = r.Name;
                    node.ParentId = r.ParentId;
                    node.Icon = r.Icon;
                    node.Url = r.Url;
                    node.Sort = r.Sort;
                    node.Level = level;

                    result.Add(node);

                    var hasChildren = source.Where(s => s.ParentId == r.Id).Any();
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
