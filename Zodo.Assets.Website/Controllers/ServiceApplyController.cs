using HZC.Infrastructure;
using log4net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Senparc.Weixin.Work.CommonAPIs;
using Senparc.Weixin.Work.Containers;
using System;
using System.Collections.Generic;
using System.Linq;
using Zodo.Assets.Application;
using Zodo.Assets.Core;
using Zodo.Assets.Website.Extensions;
using Zodo.Assets.Website.Options;

namespace Zodo.Assets.Website.Controllers
{
    [WeixinUserFiler]
    public class ServiceApplyController : WeixinWorkController
    {
        private ServiceApplyService service = new ServiceApplyService();
        ILog log = LogManager.GetLogger(typeof(ServiceApplyController));

        #region 手机端首页
        public IActionResult Index()
        {
            return View("Index");
        }

        public JsonResult Get(int? id, ServiceApplySearchParam param)
        {
            if (!id.HasValue || id <= 0)
            {
                id = 1;
            }

            if (!DataItemUtil.GetValues("AssetManager").Contains(WxUser.UserId))
            {
                param.UserId = WxUser.UserId;
            }
            var list = service.PageList((int)id, 5, param);

            return Json(ResultUtil.PageList<ServiceApply>(list));
        }
        #endregion

        #region 手机端发起申请
        public IActionResult Create()
        {
            var entity = new ServiceApply();
            entity.RequireCompleteAt = DateTime.Today.AddDays(1);
            ViewBag.Types = DataItemUtil.GetValues("ServiceTypes").ToSelectList();
            return View("Create", entity);
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            var entity = new ServiceApply();
            TryUpdateModelAsync(entity);
            entity.UserId = WxUser.UserId;
            entity.UserName = WxUser.UserName;
            entity.DeptId = WxUser.DeptId;
            entity.DeptName = WxUser.DeptName;

            try
            {
                var result = service.Create(entity);
                if (result.Code == 200)
                {
                    try
                    {
                        SendNewsMessage("来自" + entity.UserName + "的服务申请",
                            entity.Type + ":" + (string.IsNullOrWhiteSpace(entity.Describe) ? "暂无描述" : entity.Describe),
                            HttpContext.Request.Host.Host + "/ServiceApply/Details/" + result.Body.ToString(),
                            DataItemUtil.GetValue("AssetManager"));
                    }
                    catch (Exception ex)
                    {
                        log.Error(ex);
                    }
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError(String.Empty, result.Message);
                }
            }
            catch(Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            ViewBag.Types = DataItemUtil.GetValues("ServiceTypes").ToSelectList();
            return View(entity);
        }
        #endregion

        #region 手机端详情
        /// <summary>
        /// 申请详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Details(int id)
        {            
            var entity = service.Load(id);

            if (entity == null)
            {
                return RedirectToAction("Error", new { title = "404", message = "您要查看的申请信息不存在" });
            }

            string[] managers = DataItemUtil.GetValues("AssetManager");
            string viewName = "Details";
            if (managers.Contains(WxUser.UserId))
            {
                // 资产管理员
                if (entity.State == "待处理")
                {
                    viewName = "Details2";
                }
                else
                {
                    viewName = "Details3";
                }
            }
            else
            {
                // 普通用户
                if (entity.UserId != WxUser.UserId)
                {
                    return RedirectToAction("Error", new { title = "403", message = "您无权查看此数据" });
                }
            }
            
            if (!string.IsNullOrWhiteSpace(entity.AssetCode))
            {
                var assetService = new AssetService();
                var asset = assetService.Load(entity.AssetCode);
                entity.Asset = asset;
            }
            else
            {
                entity.Asset = null;
            }
            ViewBag.Scores = DataItemUtil.GetValues("ServiceScores").ToSelectList();
            return View(viewName, entity);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Recieve(int id)
        {
            string[] managers = DataItemUtil.GetValues("AssetManager");
            if (!managers.Contains(WxUser.UserId))
            {
                return Json(ResultUtil.Do(ResultCodes.无权限, "无权使用此功能"));
            }
            var result = service.Receive(id, User.GetWeixinUserId(), User.GetWeixinUserName());
            return Json(result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Remark(int id, string reason, string analysis, string solution)
        {
            string[] managers = DataItemUtil.GetValues("AssetManager");
            if (!managers.Contains(WxUser.UserId))
            {
                return Json(ResultUtil.Do(ResultCodes.无权限, "无权使用此功能"));
            }
            var result = service.Remark(id, reason, analysis, solution);
            return Json(result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Reason(int id, string reason)
        {
            string[] managers = DataItemUtil.GetValues("AssetManager");
            if (!managers.Contains(WxUser.UserId))
            {
                return Json(ResultUtil.Do(ResultCodes.无权限, "无权使用此功能"));
            }
            var result = service.SetReason(id, reason);
            return Json(result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Solution(int id, string solution)
        {
            string[] managers = DataItemUtil.GetValues("AssetManager");
            if (!managers.Contains(WxUser.UserId))
            {
                return Json(ResultUtil.Do(ResultCodes.无权限, "无权使用此功能"));
            }
            var result = service.SetSolution(id, solution);
            return Json(result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Complete(int id)
        {
            Result result;

            string[] managers = DataItemUtil.GetValues("AssetManager");

            if (!managers.Contains(WxUser.UserId))
            {
                result = ResultUtil.Do(ResultCodes.验证失败, "您无权使用此功能");
                return Json(result);
            }

            var entity = service.Load(id);
            if (entity == null)
            {
                result = ResultUtil.Do(ResultCodes.数据不存在, "请求的数据不存在");
                return Json(result);
            }
            
            if(entity.State == "待评价" || entity.State == "已评价")
            {
                result = ResultUtil.Do(ResultCodes.验证失败, "此申请已处理完成");
                return Json(result);
            }

            result = service.Complete(id, WxUser.UserId, WxUser.UserName);
            return Json(result);
        }

        public JsonResult Score(int id, string score, string reply)
        {
            Result result;

            result = service.Score(id, score, WxUser.UserId, reply);
            return Json(result);
        }
        #endregion

        #region 错误页面
        public IActionResult Error(string title, string message)
        {
            ViewData["Title"] = string.IsNullOrWhiteSpace(title) ? "操作失败" : title;
            ViewData["Message"] = message;
            return View();
        }
        #endregion

        #region 发送通知消息
        public Result SendNewsMessage(string title, string content, string url, string users)
        {
            //var token = CommonApi.GetToken(WeixinWorkOptions.CorpId, WeixinWorkOptions.Secret);
            var token = AccessTokenContainer.GetToken(WeixinWorkOptions.CorpId, WeixinWorkOptions.Secret);
            if (string.IsNullOrWhiteSpace(token))
            {
                return ResultUtil.Do<string>(ResultCodes.系统异常, token);
            }
            var result = Senparc.Weixin.Work.AdvancedAPIs.MassApi.SendNews(token, WeixinWorkOptions.AgentId, new List<Senparc.NeuChar.Entities.Article>
            {
                new Senparc.NeuChar.Entities.Article
                {
                     Title = title,
                     Description = content,
                     Url = url
                }
            }, toUser: users);
            if (result.errcode != Senparc.Weixin.ReturnCode_Work.请求成功)
            {
                return ResultUtil.Do<string>(ResultCodes.系统异常, result.errmsg);
            }
            else
            {
                return ResultUtil.Success();
            }
        }
        #endregion
    }
}