using HZC.Infrastructure;
using Microsoft.AspNetCore.Authorization;
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
    [Authorize]
    public class DeptController : MvcController
    {
        private DeptService service = new DeptService();

        #region 首页
        // GET: Dept
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Get()
        {
            var depts = DeptUtil.All();
            return Json(ResultUtil.Success<List<DeptDto>>(depts));
        }
        #endregion

        #region 编辑
        public ActionResult Edit(int? id, int? p)
        {
            Dept entity;
            if (!id.HasValue)
            {
                entity = new Dept();
                entity.Sort = 99;

                if (p.HasValue)
                {
                    entity.ParentId = (int)p;
                    var children = DeptUtil.All().Where(d => d.ParentId == p);
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
                Dept entity = new Dept();
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

        #region 资源列表
        public ActionResult Assets(int id)
        {
            var dept = DeptUtil.Get(id);
            if (dept == null)
            {
                return new EmptyResult();
            }

            ViewBag.DeptName = dept.Name;

            var param = new AssetSearchParam();
            param.DeptId = id;
            param.isContainSubDept = true;

            var assetService = new AssetService();
            var assets = assetService.ListDto(param);
            return View(assets);
        }
        #endregion

        #region 私有方法
        private void InitUI()
        {
            var depts = DeptUtil.All();
            List<SelectListItem> listItems = new List<SelectListItem>();
            foreach (var d in depts)
            {
                listItems.Add(new SelectListItem { Text = showName(d.Name, d.Level), Value = d.Id.ToString() });
            }
            ViewBag.Parents = listItems;

            string showName(string txt, int level)
            {
                string str = "";
                if (level > 1)
                {
                    for (int i = 0; i < level; i++)
                    {
                        str += HttpUtility.HtmlDecode("&nbsp;&nbsp;");
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
        #endregion
    }
}