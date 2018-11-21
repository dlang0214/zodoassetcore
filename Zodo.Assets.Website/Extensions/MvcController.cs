using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using Zodo.Assets.Application;

namespace Zodo.Assets.Website
{
    public class MvcController : Controller
    {
        protected AppUserDto AppUser
        {
            get
            {
                var user = SessionExtensions.Get<AppUserDto>(HttpContext.Session, "User");
                if (user == null)
                {
                    user = new AppUserDto();
                    user.Id = User.FindId();
                    user.Name = HttpContext.User.Identity.Name;

                    SessionExtensions.Set<AppUserDto>(HttpContext.Session, "User", user);
                }
                return user;
            }
        }
    }
}
