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
    public class DeptController : MvcController
    {
        private readonly DeptService _service = new DeptService();

        #region 首页
        // GET: Dept
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Get()
        {
            var depts = DeptUtil.All();
            return Json(ResultUtil.Success(depts));
        }
        #endregion

        #region 编辑
        public ActionResult Edit(int? id, int? p)
        {
            Dept entity;
            if (!id.HasValue)
            {
                entity = new Dept {Sort = 99};

                if (p.HasValue)
                {
                    entity.ParentId = (int)p;
                    var children = DeptUtil.All().Where(d => d.ParentId == p).ToList();
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
            try
            {
                Dept entity = new Dept();
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

        #region 资源列表
        public ActionResult Assets(int id)
        {
            var dept = DeptUtil.Get(id);
            if (dept == null)
            {
                return new EmptyResult();
            }

            ViewBag.DeptName = dept.Name;

            var param = new AssetSearchParam {DeptId = id, IsContainSubDept = true};

            var assets = new AssetService().ListDto(param);
            return View(assets);
        }
        #endregion

        #region 私有方法
        private void InitUi()
        {
            var dept = DeptUtil.All();
            var listItems = new List<SelectListItem>();
            foreach (var d in dept)
            {
                listItems.Add(new SelectListItem { Text = ShowName(d.Name, d.Level), Value = d.Id.ToString() });
            }
            ViewBag.Parents = listItems;

            string ShowName(string txt, int level)
            {
                var str = "";
                if (level > 1)
                {
                    for (var i = 0; i < level; i++)
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