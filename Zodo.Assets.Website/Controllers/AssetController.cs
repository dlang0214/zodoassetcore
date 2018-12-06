using HZC.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using Zodo.Assets.Application;
using Zodo.Assets.Core;
using Zodo.Assets.Website.Models;

namespace Zodo.Assets.Website.Controllers
{
    public class AssetController : MvcController
    {
        private readonly AssetService _service = new AssetService();

        #region 列表页
        public IActionResult Index()
        {
            InitDeptAndAccount();
            InitUi();
            return View();
        }

        public JsonResult Get(AssetSearchParam param, int pageIndex = 1, int pageSize = 20)
        {
            var list = _service.PageListDto(param, pageIndex, pageSize);
            return Json(ResultUtil.PageList(list));
        }
        #endregion

        #region 创建
        public ActionResult Create()
        {
            InitUi();
            return View("Edit2");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Create(IFormCollection collection)
        {
            var entity = new Asset();
            TryUpdateModelAsync(entity);

            var result =  _service.Create(entity, AppUser);
            return Json(result);
        }
        #endregion

        #region 修改
        public ActionResult Edit(int? id)
        {
            Asset entity;
            if (id.HasValue)
            {
                // 编辑
                entity = _service.Load((int)id);
                if (entity == null)
                {
                    return new EmptyResult();
                }
                var dept = DeptUtil.Get(entity.Id);
                ViewBag.DeptName = dept == null ? "" : dept.Name;

                if (entity.AccountId > 0)
                {
                    var accountService = new AccountService();
                    var account = accountService.Load(entity.AccountId);
                    ViewBag.AccountName = account == null ? "" : account.Name;
                }
                else
                {
                    ViewBag.AccountName = "";
                }
                InitDeptAndAccount();
            }
            else
            {
                // 创建
                entity = new Asset();
                entity.AssignDate = DateTime.Today;
                InitDeptAndAccount();
            }

            InitUi();
            return View("Edit2", entity);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Edit(IFormCollection collection)
        {
            try
            {
                var entity = new Asset();
                TryUpdateModelAsync(entity);
                var result = _service.Save(entity, AppUser);
                if (entity.Id == 0)
                {
                    entity.Id = result.Body;
                }
                SaveQrImage(entity);
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
        public JsonResult Delete(int id)
        {
            var entity = _service.Load(id);
            var result =  _service.Delete(entity, AppUser);
            return Json(result);
        }
        #endregion

        #region 详情
        public ActionResult Details(int id)
        {
            var entity = _service.LoadDto(id);
            if (entity == null)
            {
                return new EmptyResult();
            }
            
            return View(entity);
        }

        public JsonResult GetAssetLogs(int id)
        {
            var logService = new AssetLogService();
            var param = new AssetLogSearchParam();
            param.AssetId = id;
            var list = logService.ListDto(param);
            return Json(ResultUtil.Success(list.ToList()));
        }

        public JsonResult GetMaintainLogs(int id)
        {
            var maintainService = new MaintainService();
            var param = new MaintainSearchParam
            {
                AssetId = id
            };

            var list = maintainService.ListDto(param);
            return Json(ResultUtil.Success(list.ToList()));
        }

        public JsonResult GetServiceApplyLogs(string code)
        {
            var applyService = new ServiceApplyService();
            var list = applyService.Fetch(new ServiceApplySearchParam()
            {
                AssetCode = code
            });
            return Json(ResultUtil.Success(list));
        }
        #endregion

        #region 获取所有员工
        public JsonResult GetAllAccount()
        {
            return Json(new AccountBaseDto());
        }
        #endregion

        #region 资产调配
        public ActionResult Move(int id)
        {
            var entity = _service.LoadDto(id);
            if (entity == null)
            {
                return new EmptyResult();
            }
            var logDto = new AssetLog();
            logDto.AssetId = entity.Id;
            logDto.AssetName = entity.Name;
            logDto.AssetCode = entity.Code;
            logDto.FromAccountId = entity.AccountId;
            logDto.FromAccountName = entity.AccountName;
            logDto.FromDeptId = entity.DeptId;
            logDto.FromDeptName = entity.DeptName;
            logDto.Type = "调配";
            logDto.TargetDeptId = 0;
            logDto.TargetAccountId = 0;
            logDto.OperateAt = DateTime.Today;
            logDto.Remark = "";
            logDto.Pics = "";

            ViewBag.State = entity.State;
            ViewBag.Healthy = entity.Healthy;
            ViewBag.Position = entity.Position;

            if (entity.State == "作废" || entity.State == "借出")
            {
            }
            else
            {
                InitDeptAndAccount();
            }
            return View(logDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Move(AssetLog log, string position)
        {
            var result = _service.Move(log, position, AppUser);
            return Json(result);
        }
        #endregion

        #region 资产借出
        public ActionResult Loan(int id)
        {
            var asset = _service.LoadDto(id);
            if (asset == null)
            {
                return new EmptyResult();
            }

            ViewBag.State = asset.State;
            ViewBag.Healthy = asset.Healthy;

            var loan = new Loan();
            loan.AssetId = asset.Id;
            loan.AssetName = asset.Name;
            loan.AssetCode = asset.Code;
            loan.FromAccountId = asset.AccountId;
            loan.FromAccountName = asset.AccountName;
            loan.FromDeptId = asset.DeptId;
            loan.FromDeptName = asset.DeptName;
            loan.TargetDeptId = 0;
            loan.TargetAccountId = 0;
            loan.LoanAt = DateTime.Today;
            loan.ExpectedReturnAt = DateTime.Today.AddMonths(1);
            loan.IsReturn = false;
            loan.TargetPosition = asset.Position;
            loan.FromPosition = asset.Position;
            loan.Remark = "";
            loan.Pics = "";

            InitDeptAndAccount();
            return View(loan);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Loan(IFormCollection collection)
        {
            var entity = new Loan();
            TryUpdateModelAsync(entity);

            return Json(_service.Loan(entity.AssetId, entity.TargetDeptId, entity.TargetAccountId, entity.LoanAt, entity.ExpectedReturnAt, entity.Pics, entity.TargetPosition, entity.Remark, AppUser));
        }
        #endregion

        #region 资产报废
        public ActionResult Scrap(int id)
        {
            var entity = _service.LoadDto(id);
            if (entity == null)
            {
                return new EmptyResult();
            }
            var logDto = new AssetLog();
            logDto.AssetId = entity.Id;
            logDto.AssetName = entity.Name;
            logDto.AssetCode = entity.Code;
            logDto.FromAccountId = entity.AccountId;
            logDto.FromAccountName = entity.AccountName;
            logDto.FromDeptId = entity.DeptId;
            logDto.FromDeptName = entity.DeptName;
            logDto.Type = "报废";
            logDto.TargetDeptId = 0;
            logDto.TargetAccountId = 0;
            logDto.OperateAt = DateTime.Today;
            logDto.Remark = "";
            logDto.Pics = "";

            ViewBag.State = entity.State;
            ViewBag.Healthy = entity.Healthy;

            return View(logDto);
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public JsonResult Scrap(IFormCollection collection)
        {
            var log = new AssetLog();
            TryUpdateModelAsync(log);

            var result = _service.Scrap(log.AssetId, log.OperateAt, log.Pics, log.Remark, AppUser);
            return Json(result);
        }
        #endregion

        #region 资产回收
        public IActionResult Recovery(int id)
        {
            var entity = _service.LoadDto(id);
            if (entity == null)
            {
                return new EmptyResult();
            }
            var logDto = new AssetLog();
            logDto.AssetId = entity.Id;
            logDto.AssetName = entity.Name;
            logDto.AssetCode = entity.Code;
            logDto.FromAccountId = entity.AccountId;
            logDto.FromAccountName = entity.AccountName;
            logDto.FromDeptId = entity.DeptId;
            logDto.FromDeptName = entity.DeptName;
            logDto.Type = "调配";
            logDto.TargetDeptId = 0;
            logDto.TargetAccountId = 0;
            logDto.OperateAt = DateTime.Today;
            logDto.Remark = "";
            logDto.Pics = "";

            ViewBag.State = entity.State;
            ViewBag.Healthy = entity.Healthy;
            ViewBag.Position = entity.Position;

            if (entity.State == "作废" || entity.State == "借出")
            {
            }
            else
            {
                InitDeptAndAccount();
            }
            return View(logDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Recovery(IFormCollection collection, string position)
        {
            var entity = new AssetLog();
            TryUpdateModelAsync(entity);
            var result = _service.Recovery(entity.AssetId, entity.OperateAt, entity.Pics, position, entity.Remark, AppUser);
            return Json(result);
        }
        #endregion

        #region 报废资产列表
        public IActionResult Scraps()
        {
            return View();
        }

        public JsonResult GetScrapts(AssetSearchParam param, int pageIndex = 1, int pageSize = 20)
        {
            var pageList = _service.ScrapAssets(param, pageIndex, pageSize);
            return Json(ResultUtil.PageList(pageList));
        }
        #endregion

        #region 二维码
        public ActionResult QrCode(int id)
        {
            var entity = _service.LoadDto(id);
            if (entity == null)
            {
                return new EmptyResult();
            }
            var code = new AssetQrDto();
            code.Id = entity.Id;
            code.Code = entity.Code;

            return View(entity);
        }

        public ActionResult SmallQrCode(int id)
        {
            var entity = _service.LoadDto(id);
            if (entity == null)
            {
                return new EmptyResult();
            }
            var code = new AssetQrDto();
            code.Id = entity.Id;
            code.Code = entity.Code;

            return View(entity);
        }
        #endregion

        #region 生成二维码
        private void SaveQrImage(Asset asset)
        {
            var assetQrDto = new AssetQrDto();
            assetQrDto.Id = asset.Id;
            assetQrDto.Code = asset.Code;

            var content = JsonConvert.SerializeObject(assetQrDto);
            var generator = new QRCodeGenerator();
            var codeData = generator.CreateQrCode(content, QRCodeGenerator.ECCLevel.M, true);
            var qrCode = new QRCode(codeData);

            var qrImage = qrCode.GetGraphic(10, Color.Black, Color.White, false);

            var savePath = $"{Directory.GetCurrentDirectory()}//wwwroot//upload//QrCodes";
            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }
            savePath += $"//{asset.Id}.jpg";
            qrImage.Save(savePath, System.Drawing.Imaging.ImageFormat.Jpeg);
        }
        #endregion

        #region 辅助方法
        private void InitDeptAndAccount()
        {
            // 部门
            var depts = DeptUtil.GetSelectList();
            var list = depts.ToSelectList("Id", "Name");
            ViewBag.Depts = list;

            // 员工
            var accountService = new AccountService();
            var accounts = accountService.GetAccountsBaseInfo(null);
            ViewBag.Accounts = accounts;
        }

        private void InitUi()
        {
            // 状态
            ViewBag.States = AssetParameters.States.ToSelectList();
            // 健康度
            ViewBag.Healthy = AssetParameters.Healthy.ToSelectList();
            // 分类
            var cates = AssetCateUtil.All();
            var cateItems = new List<SelectListItem>();
            foreach (var c in cates)
            {
                cateItems.Add(new SelectListItem { Text = showName(c.Name, c.Level), Value = c.Id.ToString() });
            }
            ViewBag.Cates = cateItems;
        }

        private string showName(string txt, int level)
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
        #endregion

        #region 根据编号获取资产
        [HttpGet]
        public JsonResult LoadByCode(string code)
        {
            var entity = _service.LoadDto(code);
            return Json(ResultUtil.Success(entity));
        }
        #endregion

        #region 维修
        public ActionResult Maintain(int id)
        {
            var asset = _service.LoadDto(id);
            if (asset == null)
            {
                return new EmptyResult();
            }

            var mt = new Maintain
            {
                AssetCode = asset.Code,
                AssetName = asset.Name,
                Position = asset.Position,
                Imei = asset.Imei,
                Model = asset.Model,
                Band = asset.Band,
                OrigState = asset.State,
                DeptId = asset.DeptId,
                DeptName = asset.DeptName,
                AccountId = asset.AccountId,
                AccountName = asset.AccountName,
                RepairAt = DateTime.Today,
                ServiceFinishAt = DateTime.Today,
                ServiceStartAt = DateTime.Today
            };

            return View(mt);
        }

        [HttpPost]
        public JsonResult Maintain(IFormCollection collection)
        {
            var mt = new Maintain();
            TryUpdateModelAsync(mt);

            var result = _service.Maintain(mt, AppUser);
            
            return Json(result);
        }
        #endregion
    }
}
