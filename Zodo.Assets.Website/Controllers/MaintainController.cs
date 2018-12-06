using HZC.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Zodo.Assets.Application;
using Zodo.Assets.Core;

namespace Zodo.Assets.Website.Controllers
{
    public class MaintainController : MvcController
    {
        private readonly MaintainService _service = new MaintainService();

        #region 列表页
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult Get(MaintainSearchParam param, int pageIndex = 1, int pageSize = 20)
        {
            var list = _service.PageListDto(param, pageIndex, pageSize);
            return Json(ResultUtil.PageList(list));

            // var list = service.ListDto(param);
            // return MyJson(ResultUtil.Success<List<MaintainDto>>(list), JsonRequestBehavior.AllowGet);
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
            var entity = new Maintain();
            TryUpdateModelAsync(entity);

            var result =  _service.Create2(entity, AppUser);
            // 如果有缓存，注意在这里要清空缓存

            return Json(result);
        }
        #endregion

        #region 修改
        public ActionResult Edit(int id)
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
        public JsonResult Edit(IFormCollection collection)
        {
            var entity = new Maintain();
            TryUpdateModelAsync(entity);
            var result = _service.Update(entity, AppUser);
            // 如果有缓存，注意在这里要清空缓存

            return Json(result);
        }
        #endregion

        #region 删除
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Delete(int id)
        {
            var entity = _service.Load(id);
            var result =  _service.Delete(entity, AppUser);
            // 如果有缓存，注意在这里要清空缓存

            return Json(result);
        }
        #endregion
    }
}
