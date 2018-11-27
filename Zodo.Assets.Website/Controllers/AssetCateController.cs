using HZC.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Zodo.Assets.Application;
using Zodo.Assets.Core;

namespace Zodo.Assets.Website.Controllers
{
    public class AssetCateController : MvcController
    {
        private AssetCateService service = new AssetCateService();

        #region 首页
        // GET: AssetCate
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Get()
        {
            var depts = AssetCateUtil.All();
            return Json(ResultUtil.Success<List<AssetCateDto>>(depts));
        }
        #endregion

        #region 编辑
        public ActionResult Edit(int? id, int? p)
        {
            AssetCate entity;
            if (!id.HasValue)
            {
                entity = new AssetCate();
                entity.Sort = 99;

                if (p.HasValue)
                {
                    entity.ParentId = (int)p;
                    var children = AssetCateUtil.All().Where(d => d.ParentId == p);
                    if (children.Count() == 0)
                    {
                        entity.Sort = 1;
                    }
                    else
                    {
                        entity.Sort = children.Max(c => c.Sort) + 1;
                    }
                }
            }
            else
            {
                entity = service.Load((int)id);
                if (entity == null)
                {
                    return new EmptyResult();
                }
            }
            InitUI();
            return View(entity);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                AssetCate entity = new AssetCate();
                TryUpdateModelAsync(entity);
                var result = service.Save(entity, AppUser);

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
                var result = service.Delete(id, AppUser);
                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(ResultUtil.Exception(ex));
            }
        }
        #endregion

        #region 私有方法
        private void InitUI()
        {
            var cates = AssetCateUtil.Tree();
            var list = cates.ToSelectList("Id", "Name");
            ViewBag.Parents = list;
        }
        #endregion
    }
}