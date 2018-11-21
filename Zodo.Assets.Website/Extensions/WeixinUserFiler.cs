using HZC.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Threading.Tasks;

namespace Zodo.Assets.Website.Extensions
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class WeixinUserFiler : Attribute, IAsyncAuthorizationFilter
    {
        public Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            if (!context.HttpContext.User.HasClaim(c => c.Type == "WeixinUserId"))
            {
                if (context.HttpContext.Request.IsAjax())
                {
                    JsonResult result = new JsonResult(ResultUtil.Do(ResultCodes.未登录, "您的用户信息已过期，请重新打开窗口"));
                    context.Result = result;
                }
                else
                {
                    var url = "http://" + context.HttpContext.Request.Host.Host + context.HttpContext.Request.Path;
                    RedirectResult result = new RedirectResult("/OAuth2/Index?returnUrl=" + url);
                    context.Result = result;
                }
            }

            return Task.CompletedTask;
        }
    }
}
