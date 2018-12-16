using System.Collections.Generic;
using System.Linq;

namespace Zodo.Assets.Application
{
    public class AssetCateUtil
    {
        private static List<AssetCateTreeDto> _tree;

        public static List<AssetCateDto> Cates { get; set; }
        public static Dictionary<int, int[]> LevelPath { get; set; } = new Dictionary<int, int[]>();

        #region 基础数据
        public static List<AssetCateDto> All()
        {
            if (Cates == null)
            {
                Init();
            }
            return Cates;
        }

        public static AssetCateDto Get(int id)
        {
            return All().SingleOrDefault(a => a.Id == id);
        }

        private static void Init()
        {
            var service = new AssetCateService();
            var cates = service.Fetch();

            Cates = new List<AssetCateDto>();
            Dg(cates, new List<int>());
            void Dg(IReadOnlyCollection<AssetCateDto> source, IReadOnlyCollection<int> levelPath, int parent = 0, int level = 1)
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
                    Cates.Add(temp);

                    temp.Level = level;
                    temp.LevelPath = levelPath.ToList();
                    temp.LevelPath.Add(r.Id);

                    if (source.All(s => s.ParentId != r.Id)) continue;

                    level++;
                    Dg(source, temp.LevelPath, r.Id, level);
                    level--;
                }
            }
        }
        #endregion

        #region 清理缓存
        public static void Clear()
        {
            Cates = null;
            _tree = null;
            LevelPath.Clear();
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
            List<AssetCateTreeDto> Dg(IReadOnlyCollection<AssetCateDto> source, int parent = 0)
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
                    if (children.Any())
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
            if (LevelPath.TryGetValue(id, out var result))
            {
                return result;
            }

            var cate = Get(id);
            if (cate == null)
            {
                return new int[] { };
            }

            result = CalcSelfAndChildrenIds(id);
            LevelPath.Add(id, result);
            return result;
        }

        private static int[] CalcSelfAndChildrenIds(int id)
        {
            return All()
                   .Where(d => d.LevelPath.Contains(id))
                   .Select(d => d.Id)
                   .ToArray();
        }
        #endregion
    }
}
