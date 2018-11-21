using HZC.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

            if (!context.HttpContext.User.Identity.IsAuthenticated)
            {
                if (context.HttpContext.Request.IsAjax())
                {
                    JsonResult result = new JsonResult(ResultUtil.Do(ResultCodes.未登录, "您的用户信息已过期，请重新打开窗口"));
                    context.Result = result;
                }
                else
                {
                    RedirectResult result = new RedirectResult("/OAuth2/Index");
                    context.Result = result;
                }
            }
        }
    }
}
