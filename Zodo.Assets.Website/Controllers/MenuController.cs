using HZC.Infrastructure;
using IdentityModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Web;
using Zodo.Assets.Application;
using Zodo.Assets.Core;

namespace Zodo.Assets.Website.Controllers
{
    public class MenuController : MvcController
    {
        private readonly MenuService _service = new MenuService();
        private readonly JwtSettings _setting;

        public MenuController(IOptions<JwtSettings> option)
        {
            _setting = option.Value;
        }

        #region 首页
        public IActionResult Index()
        {
            return View();
        }

        public JsonResult Get()
        {
            var menus = MenuUtil.All();
            return Json(ResultUtil.Success(menus));
        }
        #endregion

        #region 保存
        public IActionResult Edit(int? id)
        {
            Menu entity;
            if (!id.HasValue)
            {
                entity = new Menu();
            }
            else
            {
                entity = _service.Load((int)id);
                if (entity == null)
                {
                    return new EmptyResult();
                }
            }
            InitUi();
            return View(entity);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Edit(IFormCollection collection)
        {
            try
            {
                var entity = new Menu();
                TryUpdateModelAsync(entity);
                var result = _service.Save(entity, AppUser);
                if (result.Code == 200)
                {
                    MenuUtil.Clear();
                }
                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(ResultUtil.Exception(ex));
            }
        }
        #endregion

        #region 删除
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Delete(int id, IFormCollection collection)
        {
            try
            {
                var entity = _service.Load(id);
                var result = _service.Delete(entity, AppUser);
                if(result.Code == 200)
                {
                    MenuUtil.Clear();
                }
                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(ResultUtil.Exception(ex));
            }
        }
        #endregion

        #region 树形列表
        [AllowAnonymous]
        public JsonResult Tree()
        {
            var tree = MenuUtil.Tree();
            return Json(tree);
        }
        #endregion

        #region 私有方法
        private void InitUi()
        {
            var menus = MenuUtil.All();
            var listItems = new List<SelectListItem>();
            foreach (var d in menus)
            {
                listItems.Add(new SelectListItem { Text = ShowName(d.Name, d.Level), Value = d.Id.ToString() });
            }
            ViewBag.Parents = listItems;

            string ShowName(string txt, int level)
            {
                var str = "";
                if (level > 1)
                {
                    for (var i = 0; i < level; i++)
                    {
                        str += HttpUtility.HtmlDecode("&nbsp;&nbsp;");
                    }
                    str += "|- " + txt;
                }
                else
                {
                    str = txt;
                }

                return str;
            }
        }
        #endregion

        [AllowAnonymous]
        public ActionResult Login()
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_setting.SecretKey);
            var authTime = DateTime.UtcNow;
            var expiresAt = authTime.AddMinutes(5);

            var claims = new[]
            {
                new Claim(JwtClaimTypes.Role, "admin"),
                new Claim(JwtClaimTypes.Name, "admin"),
                new Claim(JwtClaimTypes.Id, "4546545646"),
                new Claim(JwtClaimTypes.Audience, _setting.Audience),
                new Claim(JwtClaimTypes.Issuer, _setting.Issuer)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expiresAt,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            HttpContext.Request.Headers.Add("Authentication", tokenString);
            //return RedirectToAction("Index");
            //return Content("到这儿了");
            return Ok(new
            {
                token = tokenString
            });
        }
    }
}