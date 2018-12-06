using HZC.Infrastructure;
using log4net;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Threading.Tasks;
using Zodo.Assets.Application;
using Zodo.Assets.Website.Models;

namespace Zodo.Assets.Website.Controllers
{
    public class HomeController : MvcController
    {
        private readonly ILog _log = LogManager.GetLogger(Startup.LogResposition.Name, "");

        #region 首页
        public IActionResult Index()
        {
            ViewData["UserName"] = "admin";
            return View();
        }

        public async Task<ActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Redirect("/Login");
        }
        #endregion

        #region 用户中心
        public IActionResult UserHome()
        {
            return View();
        }
        #endregion

        #region 修改密码
        public IActionResult ChangePw()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult ChangePw(ChangePwViewModel model)
        {
            if (ModelState.IsValid)
            {
                var service = new UserService();
                var result = service.ChangePw(AppUser.Id, model.OldPw, model.NewPw);
                return Json(result);
            }
            else
            {
                var errorMessage = "";
                foreach (var v in ModelState.Values)
                {
                    foreach (var e in v.Errors)
                    {
                        errorMessage += e.ErrorMessage + ";";
                    }
                }
                return Json(ResultUtil.Do(ResultCodes.验证失败, errorMessage));
            }
        }
        #endregion

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            var feature = HttpContext.Features.Get<IExceptionHandlerFeature>();
            var error = feature?.Error;
            _log.Error(error);
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

//        public JsonResult Load(int id)
//        {
//            var service = new AssetService();
//            var entity = service.LoadDto(id);
//            if (entity == null)
//            {
//                return Json(ResultUtil.Do(ResultCodes.数据不存在, "请求的数据不存在"));
//            }
//            return Json(ResultUtil.Success(entity));
//        }
    }
}
