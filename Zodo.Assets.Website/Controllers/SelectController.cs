using HZC.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Zodo.Assets.Application;

namespace Zodo.Assets.Website.Controllers
{
    public class SelectController : MvcController
    {
        public IActionResult DeptAndAccount()
        {
            var accountService = new AccountService();

            var depts = DeptUtil.Tree();
            var accounts = accountService.GetAccountsBaseInfo(null);

            ViewBag.Depts = depts;
            ViewBag.Accounts = accounts;

            return View();
        }

        public JsonResult DeptTree()
        {
            return Json(DeptUtil.Tree());
        }

        public JsonResult Accounts(AccountSearchParam param)
        {
            var service = new AccountService();
            var accounts = service.GetAccountsBaseInfo(param.Dept, param.Key);
            return Json(ResultUtil.Success(accounts));
        }

        public ActionResult Assets()
        {
            return View();
        }

        public JsonResult AssetCateTree()
        {
            return Json(AssetCateUtil.Tree());
        }

        public JsonResult GetAssets(int cateId = 0, string key = "", int? deptId = null)
        {
            var service = new AssetService();
            var param = new AssetSearchParam();
            if (cateId > 0)
            {
                param.CateId = cateId;
            }
            if (deptId.HasValue)
            {
                if (deptId > 0)
                {
                    param.IsContainSubDept = true;
                    param.DeptId = deptId;
                }
                else
                {
                    param.IsContainSubDept = false;
                    param.DeptId = 0;
                }
            }
            if (!string.IsNullOrWhiteSpace(key))
            {
                param.Key = key;
            }
            var assets = service.ListDto(param);
            return Json(ResultUtil.Success(assets));
        }
    }
}