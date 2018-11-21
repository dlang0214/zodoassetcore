using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Zodo.Assets.Website.Extensions
{
    public class MyPermissionRequirement
    {
        public string DenyUrl { get; set; }
    }

    public class WeixinPermissionHandler : AuthorizationHandler<OperationAuthorizationRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement)
        {
            var isAuthenticated = context.User.HasClaim(c => c.Type == "WeixinUserId");
            if (isAuthenticated)
            {
                context.Succeed(requirement);
            }
            else
            {
                var httpContext = (context.Resource as Microsoft.AspNetCore.Mvc.Filters.AuthorizationFilterContext).HttpContext;
                httpContext.Response.Redirect("/OAuth2/Index");
            }

            return Task.CompletedTask;
        }
    }
}
