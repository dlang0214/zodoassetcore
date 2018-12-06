using HZC.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Zodo.Assets.Application;

namespace Zodo.Assets.Website.Controllers
{
    public class AssetLogController : MvcController
    {
        private readonly AssetLogService _service = new AssetLogService();

        #region 列表页
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult Get(AssetLogSearchParam param, int pageIndex = 1, int pageSize = 20)
        {
            var list = _service.PageListDto(param, pageIndex, pageSize);
            return Json(ResultUtil.PageList(list));

            // var list = service.ListDto(param);
            // return MyJson(ResultUtil.Success<List<AssetLogDto>>(list), JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 详情
        public IActionResult Details(int id)
        {
            var entity = _service.Load(id);
            if (entity == null)
            {
                return new EmptyResult();
            }

            return entity.Type == "回收" ? View("Details_Recovery", entity) : View(entity);
        }
        #endregion

        #region 创建
        public IActionResult Create()
        {
            return View();
        }
        #endregion
    }
}
