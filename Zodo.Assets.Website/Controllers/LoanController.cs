using System;
using System.Collections.Generic;
using System.Linq;
using HZC.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Zodo.Assets.Application;
using Zodo.Assets.Core;

namespace Zodo.Assets.Website.Controllers
{
    public class LoanController : MvcController
    {
        private LoanService service = new LoanService();

        #region 列表页
        public ActionResult Index()
        {
            InitUI();
            return View();
        }

        public JsonResult Get(LoanSearchParam param, int pageIndex = 1, int pageSize = 20)
        {
            var list = service.PageListDto(param, pageIndex, pageSize);
            return Json(ResultUtil.PageList<LoanDto>(list));

            // var list = service.ListDto(param);
            // return MyJson(ResultUtil.Success<List<LoanDto>>(list), JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 逾期未归还
        public ActionResult Late()
        {
            InitUI();
            return View();
        }

        public JsonResult GetLate(LoanSearchParam param, int pageIndex = 1, int pageSize = 20)
        {
            var list = service.PageListDto(param, pageIndex, pageSize);
            return Json(ResultUtil.PageList<LoanDto>(list));
        }
        #endregion

        #region 创建
        public ActionResult Create()
        {
            InitUI();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Create(IFormCollection collection)
        {
            Loan entity = new Loan();
            TryUpdateModelAsync(entity);

            var result =  service.Create(entity, AppUser);
            return Json(result);
        }
        #endregion

        #region 归还
        public ActionResult Return(int id)
        {
            var entity = service.Load(id);
            if (entity == null)
            {
                return new EmptyResult();
            }
            entity.ReturnAt = DateTime.Today;
            return View(entity);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Return(IFormCollection collection)
        {
            var entity = new Loan();
            TryUpdateModelAsync(entity);

            AssetService aService = new AssetService();
            var result = aService.Return(entity.Id, (DateTime)entity.ReturnAt, AppUser);
            return Json(result);
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
            return View(entity);
        }
        #endregion

        #region 逾期未归还
        public IActionResult Overdue()
        {
            return View();
        }

        public JsonResult GetOvervue(int pageIndex = 1, int pageSize = 20)
        {
            var param = new LoanSearchParam();
            param.State = 3;

            var list = service.PageListDto(param);
            return Json(ResultUtil.PageList<LoanDto>(list));
        }
        #endregion

        #region 辅助方法
        private void InitUI()
        {
        }
        #endregion
    }
}
