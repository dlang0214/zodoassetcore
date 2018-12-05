using HZC.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using Zodo.Assets.Application;
using Zodo.Assets.Core;

namespace Zodo.Assets.Website.Controllers
{
    public class AssetCateController : MvcController
    {
        private readonly AssetCateService _service = new AssetCateService();

        #region 首页
        // GET: AssetCate
        public ActionResult Index() => View();

        public ActionResult Get()
        {
            var deptList = AssetCateUtil.All();
            return Json(ResultUtil.Success<List<AssetCateDto>>(deptList));
        }
        #endregion

        #region 编辑
        public ActionResult Edit(int? id, int? p)
        {
            AssetCate entity;
            if (!id.HasValue)
            {
                entity = new AssetCate
                {
                    Sort = 99
                };

                if (p.HasValue)
                {
                    entity.ParentId = (int)p;
                    var children = AssetCateUtil.All().Where(d => d.ParentId == p).ToList();
                    entity.Sort = !children.Any() ? 1 : children.Max(c => c.Sort) + 1;
                }
            }
            else
            {
                entity = _service.Load((int)id);
                if (entity == null)
                {
                    return new EmptyResult();
                }
            }
            InitUi();
            return View(entity);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            var entity = new AssetCate();
            try
            {
                TryUpdateModelAsync(entity);
                var result = _service.Save(entity, AppUser);

                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(ResultUtil.Exception(ex));
            }
        }
        #endregion

        #region 删除
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Delete(int id, IFormCollection collection)
        {
            try
            {
                var result = _service.Delete(id, AppUser);
                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(ResultUtil.Exception(ex));
            }
        }
        #endregion

        #region 私有方法
        private void InitUi()
        {
            var list = AssetCateUtil.All().ToSelectList("Id", "Name");
            ViewBag.Parents = list;
        }
        #endregion
    }
}