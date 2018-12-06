using Microsoft.AspNetCore.Mvc;

namespace Zodo.Assets.Website.Extensions
{
    [WeixinUserFiler]
    public class WeixinWorkController : Controller
    {
        public WeixinUser WxUser
        {
            get
            {
                var user = HttpContext.Session.Get<WeixinUser>("WeixinUser");
                if (user == null)
                {
                    var u = new WeixinUser
                    {
                        UserId = User.GetWeixinUserId(),
                        UserName = User.GetWeixinUserName(),
                        DeptId = User.GetWeixinDeptId(),
                        DeptName = User.GetWeixinDeptName()
                    };

                    HttpContext.Session.Set("WeixinUser", u);
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
