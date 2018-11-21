using System.Collections.Generic;
using System.Linq;

namespace Zodo.Assets.Application
{
    public class AssetCateUtil
    {
        private static List<AssetCateDto> _cates = null;

        private static List<AssetCateTreeDto> _tree = null;

        private static Dictionary<int, int[]> _levelPath = new Dictionary<int, int[]>();

        #region 基础数据
        public static List<AssetCateDto> All()
        {
            if (_cates == null)
            {
                Init();
            }
            return _cates;
        }

        public static AssetCateDto Get(int id)
        {
            return All().Where(a => a.Id == id).SingleOrDefault();
        }

        private static void Init()
        {
            AssetCateService service = new AssetCateService();
            var cates = service.Fetch();

            _cates = new List<AssetCateDto>();
            Dg(cates, "");
            void Dg(List<AssetCateDto> source, string levelPath, int parent = 0, int level = 1)
            {
                var root = source.Where(s => s.ParentId == parent).OrderBy(s => s.Sort);
                foreach (var r in root)
                {
                    var temp = new AssetCateDto
                    {
                        Id = r.Id,
                        Name = r.Name,
                        ParentId = r.ParentId,
                        Sort = r.Sort
                    };
                    _cates.Add(temp);

                    temp.Level = level;
                    temp.LevelPath = levelPath + "," + r.Id.ToString();

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
            _cates = null;
            _tree = null;
            _levelPath.Clear();
        }
        #endregion

        #region 树形数据
        public static List<AssetCateTreeDto> Tree()
        {
            if (_tree == null)
            {
                InitTree();
            }
            return _tree;
        }

        public static void InitTree()
        {
            List<AssetCateTreeDto> Dg(List<AssetCateDto> source, int parent = 0)
            {
                var root = source.Where(s => s.ParentId == parent).OrderBy(s => s.Sort);
                var result = new List<AssetCateTreeDto>();
                foreach (var r in root)
                {
                    var temp = new AssetCateTreeDto
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
                        temp.Children = new List<AssetCateTreeDto>();
                    }
                }
                return result;
            }

            var all = All();
            _tree = Dg(all);
        }
        #endregion

        #region 链表
        /// <summary>
        /// 获取自己及所有下属分类的ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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
            string key = "," + id.ToString();
            return All()
                   .Where(d => d.LevelPath.Contains(key))
                   .Select(d => d.Id)
                   .ToArray();
        }
        #endregion
    }
}
