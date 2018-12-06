using HZC.Infrastructure;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Zodo.Assets.Application;
using Zodo.Assets.Website.Models;

namespace Zodo.Assets.Website.Controllers
{
    public class LoginController : Controller
    {
        public async Task<IActionResult> Index()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> Login(IFormCollection collection)
        {
            var service = new UserService();

            var model = new LoginViewModel();
            await TryUpdateModelAsync(model);

            if (!ModelState.IsValid)
            {
                List<string> errorMsg = new List<string>();
                foreach (var key in ModelState.Keys)
                {
                    var errors = ModelState[key].Errors.ToList();
                    foreach (var error in errors)
                    {
                        errorMsg.Add(error.ErrorMessage);
                    }
                }
                return Json(ResultUtil.Do(ResultCodes.验证失败, string.Join(";", errorMsg)));
            }

            var user = service.Login(model.Name, model.Pw);
            if (user == null)
            {
                return Json(ResultUtil.Do(ResultCodes.数据不存在, "用户不存在"));
            }
            else if (AESEncriptUtil.Decrypt(user.Pw) != model.Pw)
            {
                return Json(ResultUtil.Do(ResultCodes.验证失败, "帐号或密码错误"));
            }
            else
            {
                ClaimsIdentity identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
                identity.AddClaim(new Claim(ClaimTypes.Name, user.Name));
                identity.AddClaim(new Claim(ClaimTypes.Role, user.Name == "admin" ? "admin" : "user"));
                identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
                identity.AddClaim(new Claim(ClaimTypes.Version, user.Version));

                ClaimsPrincipal principal = new ClaimsPrincipal(identity);
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties
                {
                    IsPersistent = false,
                    //ExpiresUtc = DateTime.Today.AddDays(7),
                    //ExpiresUtc = DateTime.Now.AddMinutes(1),
                    RedirectUri = "/Login"
                });
                
                //HttpContext.Session.Set<AppUserDto>("User", new AppUserDto { Id = user.Id, Name = user.Name });
                return Json(new Result { Code = 200, Message = "" });
            }
        }
    }
}