using HZC.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using Zodo.Assets.Application;
using Zodo.Assets.Core;

namespace Zodo.Assets.Website.Controllers
{
    public class AccountController : MvcController
    {
        private readonly AccountService _service = new AccountService();

        #region 首页
        public ActionResult Index()
        {
            InitUi();
            return View();
        }

        public ActionResult Get(AccountSearchParam param, int pageIndex, int pageSize)
        {
            var accounts = _service.PageList(param, pageIndex, pageSize);
            return Json(ResultUtil.PageList(accounts));
        }
        #endregion

        #region 编辑
        public ActionResult Edit(int? id)
        {
            Account entity;
            if (!id.HasValue)
            {
                entity = new Account();
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
                var entity = new Account();
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

        #region 私有方法
        private void InitUi()
        {
            var depts = DeptUtil.GetSelectList();
            var list = depts.ToSelectList("Id", "Name");
            ViewBag.Parents = list;
        }
        #endregion

        #region 资源列表
        public ActionResult Assets(int id)
        {
            var account = _service.Load(id);
            if (account == null)
            {
                return new EmptyResult();
            }

            ViewBag.AccountName = account.Name;

            var param = new AssetSearchParam {AccountId = id};

            var assets = new AssetService().ListDto(param);
            return View(assets);
        }
        #endregion
    }
}