using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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