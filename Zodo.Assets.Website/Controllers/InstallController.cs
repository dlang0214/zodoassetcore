using Microsoft.AspNetCore.Mvc;
using Zodo.Assets.Application;

namespace Zodo.Assets.Website.Controllers
{
    public class InstallController : Controller
    {
        public IActionResult Index()
        {
            var service = new UserService();
            var admin = service.GetAdmin();

            if (admin != null) return Redirect("/Login");

            admin = new Core.User
            {
                Name = "admin",
                IsDel = false,
                Pw = "123456"
            };

            service.Create(admin, new AppUserDto { Id = 1, Name = "admin" });
            return Redirect("/Login");
        }
    }
}