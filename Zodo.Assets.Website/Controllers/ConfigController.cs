using HZC.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System;
using Zodo.Assets.Application;
using Zodo.Assets.Website.Extensions;

namespace Zodo.Assets.Website.Controllers
{
    public class ConfigController : MvcController
    {
        private readonly DataItemService _service = new DataItemService();
        private readonly WeixinDeptUtil _weixinDeptUtil;

        public ConfigController(IDistributedCache cache)
        {
            _weixinDeptUtil = new WeixinDeptUtil(cache);
        }

        #region 资产管理员帐号配置
        public IActionResult AssetManager()
        {
            var entity = _service.Load("AssetManager");
            if (entity == null)
            {
                entity = new Core.DataItem
                {
                    K = "AssetManager",
                    V = ""
                };
                _service.Create(entity, AppUser);
            }
            return View(entity);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult AssetManager(string v)
        {
            var result = _service.Update("AssetManager", v, AppUser);
            return Json(result);
        }
        #endregion

        #region 服务类型
        public ActionResult ServiceType()
        {
            var entity = _service.Load("ServiceTypes");
            if (entity == null)
            {
                entity = new Core.DataItem
                {
                    K = "ServiceTypes",
                    V = ""
                };
                _service.Create(entity, AppUser);
            }
            return View(entity);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult ServiceType(string v)
        {
            var result = _service.Update("ServiceTypes", v, AppUser);
            return Json(result);
        }
        #endregion

        #region 服务申请状态
        public ActionResult ServiceState()
        {
            var entity = _service.Load("ServiceStates");
            if (entity == null)
            {
                entity = new Core.DataItem
                {
                    K = "ServiceStates",
                    V = ""
                };
                _service.Create(entity, AppUser);
            }
            return View(entity);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult ServiceState(string v)
        {
            var result = _service.Update("ServiceStates", v, AppUser);
            return Json(result);
        }
        #endregion

        #region 服务满意度
        public ActionResult ServiceScore()
        {
            var entity = _service.Load("ServiceScores");
            if (entity == null)
            {
                entity = new Core.DataItem
                {
                    K = "ServiceScores",
                    V = ""
                };
                _service.Create(entity, AppUser);
            }
            return View(entity);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult ServiceScore(string v)
        {
            var result = _service.Update("ServiceScores", v, AppUser);
            return Json(result);
        } 
        #endregion

        #region 微信通讯录缓存
        public ActionResult WeixinUser()
        {
            return View();
        }

        public JsonResult GetWeixinUsers()
        {
            var list = WeixinUserUtil.All();
            return Json(ResultUtil.Success(list));
        }

        public ActionResult WeixinDept()
        {
            return View();
        }

        public JsonResult GetWeixinDepts()
        {
            var list = _weixinDeptUtil.All();
            return Json(ResultUtil.Success(list));
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public JsonResult ResetWeixinDeptCache()
        {
            try
            {
                _weixinDeptUtil.Reset();
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
                WeixinUserUtil.Reset();
                return Json(ResultUtil.Success());
            }
            catch (Exception ex)
            {
                return Json(ResultUtil.Exception(ex));
            }
        }
        #endregion

        #region 审批人
        public IActionResult Approver()
        {
            var entity = _service.Load("Approver");
            if (entity == null)
            {
                entity = new Core.DataItem
                {
                    K = "Approver",
                    V = ""
                };
                _service.Create(entity, AppUser);
            }
            return View(entity);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Approver(string v)
        {
            var result = _service.Update("Approver", v, AppUser);
            return Json(result);
        }
        #endregion
        
        #region 申请人
        public IActionResult Proposer()
        {
            var entity = _service.Load("Proposer");
            if (entity == null)
            {
                entity = new Core.DataItem
                {
                    K = "Approver",
                    V = ""
                };
                _service.Create(entity, AppUser);
            }
            return View(entity);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Proposer(string v)
        {
            var result = _service.Update("Proposer", v, AppUser);
            return Json(result);
        } 
        #endregion
    }
} 