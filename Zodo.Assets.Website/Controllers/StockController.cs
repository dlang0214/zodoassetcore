using HZC.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Zodo.Assets.Application;
using Zodo.Assets.Core;

namespace Zodo.Assets.Website.Controllers
{
    public class StockController : MvcController
    {
        private readonly StockService _service = new StockService();

        #region 首页
        public IActionResult Index()
        {
            return View();
        }

        public JsonResult Get(int pageIndex, int pageSize, bool? isFinish = null, string key = "")
        {
            var list = _service.Query(pageIndex, pageSize, isFinish, key);
            return Json(ResultUtil.PageList(list));
        }
        #endregion

        #region 创建一次盘点
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Create(Stock entity)
        {
            var result = _service.Create2(entity, AppUser);
            return Json(result);
        }
        #endregion

        #region 编辑盘点信息
        public IActionResult Edit(int id)
        {
            var entity = _service.Load(id);
            if (entity == null)
            {
                return new EmptyResult();
            }

            return View(entity);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Edit(int id, Stock entity)
        {
            if (entity.Id != id)
            {
                return Json(ResultUtil.Do(ResultCodes.非法请求, "非法请求"));
            }

            var result = _service.Update(entity, AppUser);
            return Json(result);
        }
        #endregion

        #region 删除
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Delete(int id)
        {
            var result = _service.Delete(id, AppUser);
            return Json(result);
        }
        #endregion
        
        #region 添加盘点明细
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult AddItems(int id, int[] assetIds)
        {
            var result = _service.SetItems(id, assetIds, AppUser);
            return Json(result);
        }
        #endregion
        
        #region 结束盘点
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Finish(int id)
        {
            var result = _service.Finish(id, AppUser);
            return Json(result);
        } 
        #endregion
    }
}