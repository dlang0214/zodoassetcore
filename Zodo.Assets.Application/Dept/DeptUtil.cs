using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Zodo.Assets.Application
{
    public class DeptUtil
    {
        private static List<DeptDto> _depts = null;

        private static List<DeptTreeDto> _tree = null;

        private static List<DeptSelectListItemDto> _selectItems = null;

        private static Dictionary<int, int[]> _levelPath = new Dictionary<int, int[]>();

        #region 基础数据
        public static List<DeptDto> All()
        {
            if (_depts == null)
            {
                Init();
            }
            return _depts;
        }

        public static DeptDto Get(int id)
        {
            return All().Where(a => a.Id == id).SingleOrDefault();
        }

        private static void Init()
        {
            DeptService service = new DeptService();
            var depts = service.Fetch(new DeptSearchParam());

            _depts = new List<DeptDto>();
            Dg(depts, "");

            void Dg(List<DeptDto> source, string levelPath = ",", int parent = 0, int level = 1)
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
                    _depts.Add(temp);

                    temp.Level = level;
                    temp.LevelPath = levelPath + r.Id.ToString() + ",";

                    if (source.Where(s => s.ParentId == r.Id).Any())
                    {
                        level++;

                        Dg(source, temp.LevelPath, r.Id, level);

                        level--;
                    }
                }
            }
        }
        #endregion

        #region 清理缓存
        public static void Clear()
        {
            _depts = null;
            _tree = null;
            _selectItems = null;
            _levelPath.Clear();
        }
        #endregion

        #region 树形数据
        public static List<DeptTreeDto> Tree()
        {
            if (_tree == null)
            {
                InitTree();
            }
            return _tree;
        }

        public static void InitTree()
        {
            List<DeptTreeDto> Dg(List<DeptDto> source, int parent = 0)
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
                    if (children.Count() > 0)
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

            var all = All();
            _tree = Dg(all);
        }
        #endregion

        #region 链表
        public static int[] GetSelfAndChildrenIds(int id)
        {
            int[] result;
            if (_levelPath.TryGetValue(id, out result))
            {
                return result;
            }
            else
            {
                var dept = Get(id);
                if (dept == null)
                {
                    result = new int[] { };
                }
                result = CalcSelfAndChildrenIds(id);
                _levelPath.Add(id, result);
                return result;
            }
        }

        private static int[] CalcSelfAndChildrenIds(int id)
        {
            string key = "," + id.ToString() + ",";
            return All()
                   .Where(d => d.LevelPath.Contains(key))
                   .Select(d => d.Id)
                   .ToArray();
        }
        #endregion

        #region 部门下拉框数据源
        public static List<DeptSelectListItemDto> GetSelectList()
        {
            if (_selectItems == null)
            {
                _selectItems = new List<DeptSelectListItemDto>();

                var depts = All();
                foreach (var d in depts)
                {
                    _selectItems.Add(new DeptSelectListItemDto { Id = d.Id, Name = showName(d.Name, d.Level) });
                }

                string showName(string txt, int level)
                {
                    string str = "";
                    if (level > 1)
                    {
                        for (int i = 1; i < level; i++)
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
            }
            return _selectItems;
        } 
        #endregion
    }
}
