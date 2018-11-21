using System;
using System.Collections.Generic;
using System.Linq;
using HZC.Infrastructure;
using Zodo.Assets.Core;
using Zodo.Assets.Application;
using Microsoft.AspNetCore.Mvc;

namespace Zodo.Assets.Website.Controllers
{
    public class AssetLogController : MvcController
    {
        private AssetLogService service = new AssetLogService();

        #region 列表页
        public ActionResult Index()
        {
            InitUI();
            return View();
        }

        public JsonResult Get(AssetLogSearchParam param, int pageIndex = 1, int pageSize = 20)
        {
            var list = service.PageListDto(param, pageIndex, pageSize);
            return Json(ResultUtil.PageList<AssetLogDto>(list));

            // var list = service.ListDto(param);
            // return MyJson(ResultUtil.Success<List<AssetLogDto>>(list), JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 详情
        public IActionResult Details(int id)
        {
            var entity = service.Load(id);
            if (entity == null)
            {
                return new EmptyResult();
            }
            else
            {
                if (entity.Type == "回收")
                {
                    return View("Details_Recovery", entity);
                }
                return View(entity);
            }
        }
        #endregion

        #region 创建
        public IActionResult Create()
        {
            return View();
        }
        #endregion

        #region 辅助方法
        private void InitUI()
        {
        }
        #endregion
    }
}
