using HZC.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Zodo.Assets.Application;
using Zodo.Assets.Core;

namespace Zodo.Assets.Website.Controllers
{
    [Authorize]
    public class MaintainController : MvcController
    {
        private MaintainService service = new MaintainService();

        #region 列表页
        public ActionResult Index()
        {
            InitUI();

            return View();
        }

        public JsonResult Get(MaintainSearchParam param, int pageIndex = 1, int pageSize = 20)
        {
            var list = service.PageListDto(param, pageIndex, pageSize);
            return Json(ResultUtil.PageList<MaintainDto>(list));

            // var list = service.ListDto(param);
            // return MyJson(ResultUtil.Success<List<MaintainDto>>(list), JsonRequestBehavior.AllowGet);
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
            Maintain entity = new Maintain();
            TryUpdateModelAsync(entity);

            var result =  service.Create2(entity, AppUser);
            // 如果有缓存，注意在这里要清空缓存

            return Json(result);
        }
        #endregion

        #region 修改
        public ActionResult Edit(int id)
        {
            var entity = service.Load(id);
            if (entity == null)
            {
                return new EmptyResult();
            }
            else
            {
                InitUI();
                return View(entity);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Edit(IFormCollection collection)
        {
            Maintain entity = new Maintain();
            TryUpdateModelAsync(entity);
            var result = service.Update(entity, AppUser);
            // 如果有缓存，注意在这里要清空缓存

            return Json(result);
        }
        #endregion

        #region 删除
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Delete(int id)
        {
            var entity = service.Load(id);
            var result =  service.Delete(entity, AppUser);
            // 如果有缓存，注意在这里要清空缓存

            return Json(result);
        }
        #endregion

        #region 辅助方法
        private void InitUI()
        {
        }
        #endregion
    }
}
