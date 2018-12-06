using HZC.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using Zodo.Assets.Application;
using Zodo.Assets.Core;

namespace Zodo.Assets.Website.Controllers
{
    public class LoanController : MvcController
    {
        private readonly LoanService _service = new LoanService();

        #region 列表页
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult Get(LoanSearchParam param, int pageIndex = 1, int pageSize = 20)
        {
            var list = _service.PageListDto(param, pageIndex, pageSize);
            return Json(ResultUtil.PageList(list));

            // var list = service.ListDto(param);
            // return MyJson(ResultUtil.Success<List<LoanDto>>(list), JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 创建
        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Create(IFormCollection collection)
        {
            var entity = new Loan();
            TryUpdateModelAsync(entity);

            var result =  _service.Create(entity, AppUser);
            return Json(result);
        }
        #endregion

        #region 归还
        public ActionResult Return(int id)
        {
            var entity = _service.Load(id);
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

            if (!entity.ReturnAt.HasValue)
            {
                return Json(ResultUtil.Do(ResultCodes.验证失败, "归还日期不能为空"));
            }

            var aService = new AssetService();
            var result = aService.Return(entity.Id, (DateTime)entity.ReturnAt, AppUser);
            return Json(result);
        }
        #endregion

        #region 详情
        public IActionResult Details(int id)
        {
            var entity = _service.Load(id);
            return entity == null ? new EmptyResult() : (IActionResult)View(entity);
        }
        #endregion

        #region 逾期未归还
        public IActionResult Overdue()
        {
            return View();
        }

        public JsonResult GetOvervue(int pageIndex = 1, int pageSize = 20)
        {
            var param = new LoanSearchParam {State = 3};

            var list = _service.PageListDto(param);
            return Json(ResultUtil.PageList(list));
        }
        #endregion
    }
}
