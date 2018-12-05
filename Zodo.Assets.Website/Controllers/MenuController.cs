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
        private MenuService service = new MenuService();
        private JwtSettings setting;

        public MenuController(IOptions<JwtSettings> option)
        {
            setting = option.Value;
        }

        #region 首页
        public IActionResult Index()
        {
            return View();
        }

        public JsonResult Get()
        {
            var menus = MenuUtil.All();
            return Json(ResultUtil.Success<List<MenuDto>>(menus));
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
                entity = service.Load((int)id);
                if (entity == null)
                {
                    return new EmptyResult();
                }
            }
            InitUI();
            return View(entity);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Edit(IFormCollection collection)
        {
            try
            {
                Menu entity = new Menu();
                TryUpdateModelAsync(entity);
                var result = service.Save(entity, AppUser);
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
                var entity = service.Load(id);
                var result = service.Delete(entity, AppUser);
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
        private void InitUI()
        {
            var menus = MenuUtil.All();
            List<SelectListItem> listItems = new List<SelectListItem>();
            foreach (var d in menus)
            {
                listItems.Add(new SelectListItem { Text = showName(d.Name, d.Level), Value = d.Id.ToString() });
            }
            ViewBag.Parents = listItems;

            string showName(string txt, int level)
            {
                string str = "";
                if (level > 1)
                {
                    for (int i = 0; i < level; i++)
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
            var key = Encoding.UTF8.GetBytes(setting.SecretKey);
            var authTime = DateTime.UtcNow;
            var expiresAt = authTime.AddMinutes(5);

            var claims = new Claim[]
            {
                new Claim(JwtClaimTypes.Role, "admin"),
                new Claim(JwtClaimTypes.Name, "admin"),
                new Claim(JwtClaimTypes.Id, "4546545646"),
                new Claim(JwtClaimTypes.Audience, setting.Audience),
                new Claim(JwtClaimTypes.Issuer, setting.Issuer)
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