using HZC.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Zodo.Assets.Application;
using Zodo.Assets.Core;

namespace Zodo.Assets.Website.Controllers
{
    public class StockController : MvcController
    {
        private StockService service = new StockService();

        #region 首页
        public IActionResult Index()
        {
            return View();
        }

        public JsonResult Get(int pageIndex, int pageSize, bool? isFinish = null, string key = "")
        {
            var list = service.Query(pageIndex, pageSize, isFinish, key);
            return Json(ResultUtil.PageList<Stock>(list));
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
            var result = service.Create2(entity, AppUser);
            return Json(result);
        }
        #endregion

        #region 编辑盘点信息
        public IActionResult Edit(int id)
        {
            var entity = service.Load(id);
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

            var result = service.Update(entity, AppUser);
            return Json(result);
        }
        #endregion

        #region 删除
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Delete(int id)
        {
            var result = service.Delete(id, AppUser);
            return Json(result);
        }
        #endregion
        
        #region 添加盘点明细
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult AddItems(int id, int[] assetIds)
        {
            var result = service.SetItems(id, assetIds, AppUser);
            return Json(result);
        }
        #endregion
        
        #region 结束盘点
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Finish(int id)
        {
            var result = service.Finish(id, AppUser);
            return Json(result);
        } 
        #endregion
    }
}