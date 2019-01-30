using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HZC.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Zodo.Assets.Services;
using Zodo.Assets.Website.Extensions;

namespace Zodo.Assets.Website.Controllers
{
    [WeixinUserFiler]
    public class ProposerController : WeixinWorkController
    {
        private readonly ProposerService service = new ProposerService();

        public ProposerController()
        { }

        #region 列表页
        public ActionResult Index()
        {
            InitUI();
            return View();
        }

        public JsonResult Get(ProposerSearchParam param, int pageIndex = 1, int pageSize = 20)
        {
            var list = service.(param, pageIndex, pageSize);
            return Json(ResultUtil.PageList<ProposerEntity>(list));

            // var list = service.List(param);
            // return MyJson(ResultUtil.Success<List<ProposerEntity>>(list));
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
            ProposerEntity entity = new ProposerEntity();
            TryUpdateModelAsync(entity);

            var result = service.Create(entity, AppUser);
            // 如果有缓存，注意在这里要清空缓存

            return MyJson(result);
        }
        #endregion

        #region 修改
        public ActionResult Edit(int id)
        {
            var entity = service.Load(id);
            if (entity == null)
            {
                return NotFound;
            }
            else
            {
                InitUI();
                return View(entity);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Edit(int id, IFormCollection collection)
        {
            ProposerEntity entity = new ProposerEntity();
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
            if (entity == null)
            {
                return Json(ResultUtil.AuthFail("请求的数据不存在"));
            }
            var result = service.Remove(entity, AppUser);
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