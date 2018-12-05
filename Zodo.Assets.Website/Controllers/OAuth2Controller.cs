using log4net;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Senparc.CO2NET.Extensions;
using Senparc.Weixin.Work.AdvancedAPIs;
using Senparc.Weixin.Work.Containers;
using System;
using System.Linq;
using System.Security.Claims;
using Zodo.Assets.Website.Extensions;
using Zodo.Assets.Website.Options;

namespace Zodo.Assets.Website.Controllers
{
    public class OAuth2Controller : Controller
    {
        private readonly string _corpId = WeixinWorkOptions.CorpId;
        private readonly string _secret = WeixinWorkOptions.Secret;
        private readonly string _agentId = WeixinWorkOptions.AgentId;

        private readonly WeixinDeptUtil _deptUtil;
        private readonly WeixinUserUtil2 _userUtil;

        private readonly ILog _log = LogManager.GetLogger(Startup.LogResposition.Name, "OAuth2Controller");

        public OAuth2Controller(IDistributedCache cache)
        {
            _deptUtil = new WeixinDeptUtil(cache);
            _userUtil = new WeixinUserUtil2(cache);
        }

        public IActionResult Index(string returnUrl)
        {
            //string state = "Zodolabs-Asset-" + DateTime.Now.Millisecond;
            var redirectUrl = "http://" + HttpContext.Request.Host.Host + "/OAuth2/UserInfoCallback?returnUrl=" + returnUrl.UrlEncode();
            var url = OAuth2Api.GetCode(_corpId, redirectUrl, "", _agentId);
            return Redirect(url);
        }

        public IActionResult UserInfoCallback(string code, string returnUrl)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                return RedirectToAction("Error", new { title = "无效访问", message = "您的请求参数不合法，请从正规渠道访问此功能。" });
            }

            try
            {
                var token = AccessTokenContainer.TryGetToken(_corpId, _secret);
                if (string.IsNullOrWhiteSpace(token))
                {
                    _log.Error("获取ACCESSTOKEN详情失败：AccessTokenContainer.TryGetToken()获取失败");
                    return RedirectToAction("Error", new { title = "访问失败", message = "从微信服务端请求数据失败，请稍候再试。" });
                }
                
                var user = OAuth2Api.GetUserId(token, code);
                if (user.errcode != Senparc.Weixin.ReturnCode_Work.请求成功)
                {
                    _log.Error("获取用户ID失败：" + user.errmsg);
                    return RedirectToAction("Error", new { title = "加载失败", message = "从微信服务端获取用户信息失败，请联系管理员或稍候再试" });
                }

                if (string.IsNullOrWhiteSpace(user.UserId))
                {
                    _log.Error("获取用户ID失败，接口调用成功，但USERID为空：" + JsonConvert.SerializeObject(user));
                    return RedirectToAction("Error", new { title = "拒绝访问", message = "仅限企业微信内部员工使用，未能获取到您的数据，请联系管理员" });
                }
                else
                {
                    var userInfo = _userUtil.Get(user.UserId);
                    if (userInfo != null)
                    {
                        var userName = userInfo.name;

                        var deptId = 0;
                        if (userInfo.department.Length > 0)
                        {
                            deptId = (int)userInfo.department.Max();
                        }

                        var dept = _deptUtil.Get(deptId);
                        if (dept == null)
                        {
                            _log.Error("获取部门失败，用户消息：" + string.Join(',', userInfo.department));
                            return RedirectToAction("Error", new { title = "访问失败", message = "获取部门数据失败，请联系管理员" });
                        }

                        var userDto = new WeixinUser
                        {
                            UserId = userInfo.userid.ToLower(),
                            UserName = userInfo.name,
                            DeptId = deptId,
                            DeptName = dept.name
                        };

                        var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme, "WeixinUserName", ClaimTypes.Role);

                        identity.AddClaim(new Claim("WeixinUserId", user.UserId));
                        identity.AddClaim(new Claim("WeixinUserName", userName));
                        identity.AddClaim(new Claim("WeixinDeptId", dept.id.ToString()));
                        identity.AddClaim(new Claim("WeixinDeptName", dept.name));
                        identity.AddClaim(new Claim(ClaimTypes.Role, "Weixin"));

                        var principal = new ClaimsPrincipal(identity);
                        HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                        HttpContext.SignInAsync(principal, new AuthenticationProperties
                        {
                            IsPersistent = true,
                            RedirectUri = "/OAuth2/Index"
                        });
                        
                        HttpContext.Session.Set("WeixinUser", userDto);
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        _log.Error("获取用户详情失败：缓存中不存在此");
                        return RedirectToAction("Error", new { title = "无效访问", message = "仅限企业微信内部员工使用，若您加入企业，请联系管理员" });
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex);
                return RedirectToAction("Error", new { title = "访问失败", message = "系统错误：" + ex.Message + "，请联系管理员" });
            }
        }

        #region 错误页面
        public IActionResult Error(string title, string message)
        {
            ViewData["Title"] = string.IsNullOrWhiteSpace(title) ? "操作失败" : title;
            ViewData["Message"] = message;
            return View();
        }
        #endregion
    }
}