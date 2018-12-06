using HZC.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using Zodo.Assets.Application;
using Zodo.Assets.Core;

namespace Zodo.Assets.Website.Controllers
{
    public class AppUserController : MvcController
    {
        private readonly UserService _service = new UserService();

        #region 首页
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Get(UserSearchParam param)
        {
            var users = _service.Fetch(param);
            return Json(ResultUtil.Success(users));
        }
        #endregion

        #region 编辑
        public ActionResult Edit(int? id)
        {
            User entity;
            if (!id.HasValue)
            {
                entity = new User();
            }
            else
            {
                entity = _service.Load((int)id);
                if (entity == null)
                {
                    return new EmptyResult();
                }
            }
            return View(entity);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                var entity = new User();
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
                var result = _service.Delete(id);
                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(ResultUtil.Exception(ex));
            }
        }
        #endregion

        #region 重置密码
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult ResetPw(int id)
        {
            try
            {
                var result = _service.ResetPw(id, AppUser);
                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(ResultUtil.Exception(ex));
            }
        }
        #endregion

        #region 私有方法
        #endregion
    }
}