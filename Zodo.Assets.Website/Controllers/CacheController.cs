using Microsoft.AspNetCore.Mvc;
using Zodo.Assets.Application;

namespace Zodo.Assets.Website.Controllers
{
    public class CacheController : Controller
    {
        public IActionResult DeptDtos()
        {
            var list = DeptUtil.All();
            return Json(list);
        }

        public IActionResult DeptTree()
        {
            var list = DeptUtil.Tree();
            return Json(list);
        }

        public IActionResult AssetCateDtos()
        {
            var list = AssetCateUtil.All();
            return Json(list);
        }

        public IActionResult AssetCateTree()
        {
            var list = AssetCateUtil.Tree();
            return Json(list);
        }
    }
}