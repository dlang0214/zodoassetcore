using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Zodo.Assets.Website.Extensions
{
    public class WeixinUser2Filter : TypeFilterAttribute
    {
        public WeixinUser2Filter() : base(typeof(WeixinUser2FilterImpl))
        {
        }

        private class WeixinUser2FilterImpl : IActionFilter
        {
            private IDistributedCache _cache;
            private ILogger _log;

            public WeixinUser2FilterImpl(IDistributedCache cache, ILogger log)
            {
                _cache = cache;
                _log = log;
            }

            public void OnActionExecuted(ActionExecutedContext context)
            {
                //throw new NotImplementedException();
                _log.LogDebug("执行结束WeixinUser2Filter");
            }

            public void OnActionExecuting(ActionExecutingContext context)
            {
                if (context.HttpContext.User.HasClaim(c => c.Type == "WeixinUserId"))
                {

                }
                else
                {

                }
                _log.LogDebug("开始执行WeixinUser2Filter");
            }
        }
    }
}
