using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HZC.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Zodo.Assets.Application;
using Zodo.Assets.Website.Extensions;

namespace Zodo.Assets.Website.Controllers
{
    [Authorize]
    public class ConfigController : MvcController
    {
        private DataItemService service = new DataItemService();
        private readonly WeixinDeptUtil2 weixinDeptUtil;
        private readonly WeixinUserUtil2 weixinUserUtil;

        public ConfigController(IDistributedCache cache)
        {
            weixinDeptUtil = new WeixinDeptUtil2(cache);
            weixinUserUtil = new WeixinUserUtil2(cache);
        }

        #region 资产管理员帐号配置
        public IActionResult AssetManager()
        {
            var entity = service.Load("AssetManager");
            if (entity == null)
            {
                entity = new Core.DataItem();
                entity.K = "AssetManager";
                entity.V = "";

                service.Create(entity, AppUser);
            }
            return View(entity);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult AssetManager(string v)
        {
            var result = service.Update("AssetManager", v, AppUser);
            return Json(result);
        }
        #endregion

        #region 服务类型
        public ActionResult ServiceType()
        {
            var entity = service.Load("ServiceTypes");
            if (entity == null)
            {
                entity = new Core.DataItem();
                entity.K = "ServiceTypes";
                entity.V = "";

                service.Create(entity, AppUser);
            }
            return View(entity);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult ServiceType(string v)
        {
            var result = service.Update("ServiceTypes", v, AppUser);
            return Json(result);
        }
        #endregion

        #region 服务申请状态
        public ActionResult ServiceState()
        {
            var entity = service.Load("ServiceStates");
            if (entity == null)
            {
                entity = new Core.DataItem();
                entity.K = "ServiceStates";
                entity.V = "";

                service.Create(entity, AppUser);
            }
            return View(entity);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult ServiceState(string v)
        {
            var result = service.Update("ServiceStates", v, AppUser);
            return Json(result);
        }
        #endregion

        #region 服务满意度
        public ActionResult ServiceScore()
        {
            var entity = service.Load("ServiceScores");
            if (entity == null)
            {
                entity = new Core.DataItem();
                entity.K = "ServiceScores";
                entity.V = "";

                service.Create(entity, AppUser);
            }
            return View(entity);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult ServiceScore(string v)
        {
            var result = service.Update("ServiceScores", v, AppUser);
            return Json(result);
        } 
        #endregion

        #region 微信通讯录缓存
        public ActionResult WexinCache()
        {
            return View();
        }

        public ActionResult WeixinUser()
        {
            return View();
        }

        public JsonResult GetWeixinUsers()
        {
            var list = weixinUserUtil.All();
            return Json(ResultUtil.Success(list));
        }

        public ActionResult WeixinDept()
        {
            return View();
        }

        public JsonResult GetWeixinDepts()
        {
            var list = weixinDeptUtil.All();
            return Json(ResultUtil.Success(list));
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public JsonResult ResetWeixinDeptCache()
        {
            try
            {
                weixinDeptUtil.Reset();
                return Json(ResultUtil.Success());
            }
            catch (Exception ex)
            {
                return Json(ResultUtil.Exception(ex));
            }
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public JsonResult ResetWeixinUserCache()
        {
            try
            {
                //WeixinUserUtil.Reset();
                weixinUserUtil.Reset();
                return Json(ResultUtil.Success());
            }
            catch (Exception ex)
            {
                return Json(ResultUtil.Exception(ex));
            }
        } 
        #endregion
    }
} 