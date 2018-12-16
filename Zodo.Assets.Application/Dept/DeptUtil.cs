using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Zodo.Assets.Application
{
    public class DeptUtil
    {
        public static List<DeptDto> Depts { get; set; }

        public static Dictionary<int, int[]> LevelPath { get; set; } = new Dictionary<int, int[]>();

        public static List<DeptSelectListItemDto> SelectItems { get; set; }

        public static List<DeptTreeDto> TreeList { get; set; }

        #region 基础数据
        public static List<DeptDto> All()
        {
            if (Depts == null)
            {
                Init();
            }
            return Depts;
        }

        public static DeptDto Get(int id)
        {
            return All().SingleOrDefault(a => a.Id == id);
        }

        private static void Init()
        {
            var service = new DeptService();
            var depts = service.Fetch(new DeptSearchParam());

            Depts = new List<DeptDto>();
            Dg(depts, new List<int>());

            void Dg(IReadOnlyCollection<DeptDto> source, IReadOnlyCollection<int> levelPath, int parent = 0, int level = 1)
            {
                var root = source.Where(s => s.ParentId == parent).OrderBy(s => s.Sort);
                foreach (var r in root)
                {
                    var temp = new DeptDto
                    {
                        Id = r.Id,
                        Name = r.Name,
                        ParentId = r.ParentId,
                        Sort = r.Sort
                    };
                    Depts.Add(temp);

                    temp.Level = level;
                    temp.LevelPath = levelPath.ToList();
                    temp.LevelPath.Add(r.Id);

                    if (source.All(s => s.ParentId != r.Id)) continue;

                    level++;
                    Dg(source, temp.LevelPath.ToList(), r.Id, level);
                    level--;
                }
            }
        }
        #endregion

        #region 清理缓存
        public static void Clear()
        {
            Depts = null;
            TreeList = null;
            SelectItems = null;
            LevelPath.Clear();
        }
        #endregion

        #region 树形数据
        public static List<DeptTreeDto> Tree()
        {
            if (TreeList == null)
            {
                InitTree();
            }
            return TreeList;
        }

        public static void InitTree()
        {
            var all = All();
            TreeList = Dg(all);

            List<DeptTreeDto> Dg(IReadOnlyCollection<DeptDto> source, int parent = 0)
            {
                var root = source.Where(s => s.ParentId == parent).OrderBy(s => s.Sort);
                var result = new List<DeptTreeDto>();
                foreach (var r in root)
                {
                    var temp = new DeptTreeDto
                    {
                        Id = r.Id,
                        Name = r.Name,
                        ParentId = r.ParentId,
                        Level = r.Level,
                        Sort = r.Sort
                    };
                    result.Add(temp);
                    var children = source.Where(c => c.ParentId == r.Id);
                    if (children.Any())
                    {
                        temp.IsLeaf = false;
                        temp.Children = Dg(source, r.Id);
                    }
                    else
                    {
                        temp.IsLeaf = true;
                        temp.Children = new List<DeptTreeDto>();
                    }
                }
                return result;
            }
        }
        #endregion

        #region 链表
        public static int[] GetSelfAndChildrenIds(int id)
        {
            if (LevelPath.TryGetValue(id, out var result))
            {
                return result;
            }
            else
            {
                var dept = Get(id);
                if (dept == null)
                {
                    return new int[] { };
                }
                result = CalcSelfAndChildrenIds(id);
                LevelPath.Add(id, result);
                return result;
            }
        }

        private static int[] CalcSelfAndChildrenIds(int id)
        {
            return All()
                   .Where(d => d.LevelPath.Contains(id))
                   .Select(d => d.Id)
                   .ToArray();
        }
        #endregion

        #region 部门下拉框数据源
        public static List<DeptSelectListItemDto> GetSelectList()
        {
            if (SelectItems != null) return SelectItems;

            SelectItems = new List<DeptSelectListItemDto>();

            var depts = All();
            foreach (var d in depts)
            {
                SelectItems.Add(new DeptSelectListItemDto { Id = d.Id, Name = ShowName(d.Name, d.Level) });
            }

            string ShowName(string txt, int level)
            {
                var str = string.Empty;
                if (level > 1)
                {
                    for (var i = 1; i < level; i++)
                    {
                        str += HttpUtility.HtmlDecode("&nbsp;&nbsp;&nbsp;&nbsp;");
                    }
                    str += "|- " + txt;
                }
                else
                {
                    str = txt;
                }

                return str;
            }
            return SelectItems;
        } 
        #endregion
    }
}
