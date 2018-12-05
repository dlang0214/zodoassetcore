using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Zodo.Assets.Application;

namespace Zodo.Assets.Website
{
    [Authorize]
    public class MvcController : Controller
    {
        protected AppUserDto AppUser
        {
            get
            {
                var user = HttpContext.Session.Get<AppUserDto>("User");

                if (user != null) return user;

                user = new AppUserDto
                {
                    Id = User.FindId(),
                    Name = HttpContext.User.Identity.Name
                };

                HttpContext.Session.Set("User", user);
                return user;
            }
        }
    }
}
