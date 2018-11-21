﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Zodo.Assets.Website.Extensions
{
    public class WeixinWorkController : Controller
    {
        public WeixinUser WxUser
        {
            get
            {
                var user = HttpContext.Session.Get<WeixinUser>("WeixinUser");
                if (user == null)
                {
                    var u = new WeixinUser();
                    u.UserId = User.GetWeixinUserId();
                    u.UserName = User.GetWeixinUserName();
                    u.DeptId = User.GetWeixinDeptId();
                    u.DeptName = User.GetWeixinDeptName();

                    HttpContext.Session.Set<WeixinUser>("WeixinUser", u);
                    return u;
                }
                else
                {
                    return user;
                }
            }
        }
    }

    public class WeixinUser
    {
        public string UserId { get; set; }

        public string UserName { get; set; }

        public int DeptId { get; set; }

        public string DeptName { get; set; }
    }
}