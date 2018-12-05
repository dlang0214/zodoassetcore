using HZC.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace Zodo.Assets.Website.Extensions
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class WeixinWorkAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (context == null)
            {
                throw new ArgumentException("filterContext");
            }

            if (context.HttpContext.User.Identity.IsAuthenticated) return;

            if (context.HttpContext.Request.IsAjax())
            {
                var result = new JsonResult(ResultUtil.Do(ResultCodes.未登录, "您的用户信息已过期，请重新打开窗口"));
                context.Result = result;
            }
            else
            {
                var result = new RedirectResult("/OAuth2/Index");
                context.Result = result;
            }
        }
    }
}
