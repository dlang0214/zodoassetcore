using HZC.Database;
using HZC.Infrastructure;
using HZC.SearchUtil;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using Zodo.Assets.Application;
using Zodo.Assets.Core;
using Zodo.Assets.Services;
using Zodo.Assets.Website.Models;

namespace Zodo.Assets.Website.Controllers
{

    public class StockItemController : MvcController
    {
        private StockService stockService = new StockService();
        private StockItemService service = new StockItemService();

        #region 首页
        public IActionResult Index(int id)
        {
            var entity = stockService.Load(id);
            if (entity == null)
            {
                return new EmptyResult();
            }

            InitDepts();
            if (entity.IsFinish)
            {
                return View(entity);
            }
            return View(entity);
        }

        public ActionResult Get(StockItemSearchParam param, int pageIndex = 1, int pageSize = 20)
        {
            var items = stockService.PageListItems(param, pageIndex, pageSize);
            return Json(ResultUtil.PageList<StockItem>(items));
        }
        #endregion

        #region 详情
        public ActionResult Details(int id)
        {
            var entity = service.Load(id);
            if (entity == null)
            {
                return new EmptyResult();
            }
            if (string.IsNullOrWhiteSpace(entity.Checkor))
            {
                entity.Checkor = AppUser.Name;
                entity.CheckAt = DateTime.Now;
                entity.CheckMethod = 2;
                return View("Details", entity);
            }
            return View(entity);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Stock(int id, int checkResult, string remark)
        {
            var result = service.Check(id, checkResult, 2, AppUser.Name, remark);
            return Json(result);
        }
        #endregion

        #region 部门Select
        private void InitDepts()
        {
            var depts = DeptUtil.GetSelectList();
            var list = depts.ToSelectList("Id", "Name");
            ViewBag.Parents = list;
        }
        #endregion

        #region 选择资产
        /// <summary>
        /// 选择资产并添加到盘点明细
        /// </summary>
        /// <param name="id">盘点ID（StockId）</param>
        /// <returns></returns>
        public IActionResult Select(int id)
        {
            var stockService = new StockService();
            var stock = stockService.Load(id);

            if (stock == null || stock.IsFinish || stock.IsDel)
            {
                return new EmptyResult();
            }

            InitDepts();
            // 状态
            ViewBag.States = AssetParameters.States.ToSelectList();
            // 健康度
            ViewBag.Healthy = AssetParameters.Healthy.ToSelectList();
            // 分类
            var cates = AssetCateUtil.All();
            List<SelectListItem> cateItems = new List<SelectListItem>();
            foreach (var c in cates)
            {
                cateItems.Add(new SelectListItem { Text = showName(c.Name, c.Level), Value = c.Id.ToString() });
            }
            ViewBag.Cates = cateItems;

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

            return View(stock);
        }

        public JsonResult GetAssets(AssetSearchParam param, int stockId, int pageIndex = 1, int pageSize = 50)
        {
            MySearchUtil util = param.ToSearchUtil();
            util.And("Id NOT IN (SELECT AssetId FROM Asset_StockItem WHERE StockId=" + stockId.ToString() + ")");

            var db = new MyDbUtil();
            var list = db.Query<AssetDto>(util, pageIndex, pageSize, "AssetView");
            return Json(ResultUtil.PageList<AssetDto>(list));
        }
        #endregion

        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult Delete(int id)
        {
            var db = new MyDbUtil();
            var row = db.Delete<StockItem>(id);
            var result = row > 0 ? ResultUtil.Success() : ResultUtil.Do(ResultCodes.数据库操作失败, "操作失败");
            return Json(result);
        }

        public IActionResult Export(int id, string title)
        {
            if (id <= 0)
            {
                return Content("无效访问");
            }
            var param = new StockItemSearchParam();
            param.StockId = id;
            var list = service.FetchDto(param);
            return Redirect(SaveExcel(list, "盘点明细", title));
        }

        private string SaveExcel(IEnumerable<StockItemDto> groups, string fileName, string stockName)
        {
            string folderName = DateTime.Today.ToString("yyyyMM");
            fileName = (string.IsNullOrWhiteSpace(fileName) ? Guid.NewGuid().ToString("N") : fileName) + ".xlsx";
            fileName = stockName + "-" + fileName;
            string baseFolderName = $"{Directory.GetCurrentDirectory()}//wwwroot//report";
            if (!Directory.Exists(baseFolderName))
            {
                Directory.CreateDirectory(baseFolderName);
            }
            string savePath = $"{baseFolderName}//{fileName}";

            if (System.IO.File.Exists(savePath))
            {
                System.IO.File.Delete(savePath);
            }

            using (ExcelPackage package = new ExcelPackage(new System.IO.FileInfo(savePath)))
            {
                ExcelWorksheet workSheet = package.Workbook.Worksheets.Add("sheet1");
                workSheet.Cells.Style.Font.Name = "microsoft yahei";
                workSheet.Cells.Style.Font.Size = 9;
                workSheet.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                workSheet.Column(1).Width = 16;
                workSheet.Column(2).Width = 16;
                workSheet.Column(3).Width = 24;
                workSheet.Column(4).Width = 16;
                workSheet.Column(5).Width = 16;
                workSheet.Column(6).Width = 16;
                workSheet.Column(7).Width = 16;
                workSheet.Column(8).Width = 16;
                workSheet.Column(9).Width = 16;
                workSheet.Column(10).Width = 16;
                workSheet.Column(11).Width = 16;
                workSheet.Column(12).Width = 30;
                workSheet.Column(13).Width = 16;
                workSheet.Column(14).Width = 16;
                workSheet.Column(15).Width = 16;
                workSheet.Column(16).Width = 16;

                int rowIndex = 1;

                workSheet.Cells[rowIndex, 1].Value = "盘点明细";       // 标题文本
                workSheet.Cells[rowIndex, 1].Style.Font.Bold = true;        // 标题文字加粗
                workSheet.Cells[rowIndex, 1].Style.Font.Size = 12;          // 标题文字大小
                workSheet.Cells[rowIndex, 1, rowIndex, 13].Merge = true;     // 合并单元格
                workSheet.Cells[rowIndex, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;   // 居中

                workSheet.Row(rowIndex).Height = 30;

                rowIndex++;

                workSheet.Cells[rowIndex, 1].Value = "资产编码";
                workSheet.Cells[rowIndex, 2].Value = "财务编码";
                workSheet.Cells[rowIndex, 3].Value = "资产名称";
                workSheet.Cells[rowIndex, 4].Value = "品牌";
                workSheet.Cells[rowIndex, 5].Value = "型号";
                workSheet.Cells[rowIndex, 6].Value = "规格";
                workSheet.Cells[rowIndex, 7].Value = "序列号";
                workSheet.Cells[rowIndex, 8].Value = "健康度";
                workSheet.Cells[rowIndex, 9].Value = "状态";
                workSheet.Cells[rowIndex, 10].Value = "使用部门";
                workSheet.Cells[rowIndex, 11].Value = "使用人";
                workSheet.Cells[rowIndex, 12].Value = "所在位置";
                workSheet.Cells[rowIndex, 13].Value = "盘点方式";
                workSheet.Cells[rowIndex, 14].Value = "盘点时间";
                workSheet.Cells[rowIndex, 15].Value = "盘点人";
                workSheet.Cells[rowIndex, 16].Value = "盘点结果";

                workSheet.Cells[rowIndex, 1, rowIndex, 16].Style.Fill.PatternType = ExcelFillStyle.Solid;
                workSheet.Cells[rowIndex, 1, rowIndex, 16].Style.Fill.BackgroundColor.SetColor(Color.Yellow);

                workSheet.Cells[rowIndex, 1, rowIndex, 16].Style.Font.Bold = true;
                workSheet.Cells[rowIndex, 1, rowIndex, 16].Style.Font.Size = 9;
                workSheet.Cells[rowIndex, 1, rowIndex, 16].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                workSheet.Cells[rowIndex, 1, rowIndex, 16].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                workSheet.Cells[rowIndex, 1, rowIndex, 16].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                workSheet.Cells[rowIndex, 1, rowIndex, 16].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                workSheet.Cells[rowIndex, 1, rowIndex, 16].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin, Color.Black);

                workSheet.Row(rowIndex).Height = 24;

                rowIndex++;

                if (groups.Count() > 0)
                {
                    foreach (var item in groups)
                    {
                        var checkMethod = "";
                        if (item.CheckMethod == 1)
                        {
                            checkMethod = "扫码盘点";
                        }
                        else if(item.CheckMethod == 2)
                        {
                            checkMethod = "手动盘点";
                        }

                        var checkResult = "";
                        if (item.CheckResult == 1)
                        {
                            checkResult = "盘点成功";
                        }
                        else if (item.CheckResult == 2)
                        {
                            checkResult = "盘点异常";
                        }
                        else if(item.CheckResult == 0)
                        {
                            checkResult = "未盘点";
                        }

                        workSheet.Cells[rowIndex, 1].Value = item.AssetCode;
                        workSheet.Cells[rowIndex, 2].Value = item.FinancialCode;
                        workSheet.Cells[rowIndex, 3].Value = item.AssetName;
                        workSheet.Cells[rowIndex, 4].Value = item.Band;
                        workSheet.Cells[rowIndex, 5].Value = item.Model;
                        workSheet.Cells[rowIndex, 6].Value = item.Specification;
                        workSheet.Cells[rowIndex, 7].Value = item.Imei;
                        workSheet.Cells[rowIndex, 8].Value = item.Healthy;
                        workSheet.Cells[rowIndex, 9].Value = item.State;
                        workSheet.Cells[rowIndex, 10].Value = item.DeptName;
                        workSheet.Cells[rowIndex, 11].Value = item.AccountName;
                        workSheet.Cells[rowIndex, 12].Value = item.Position;
                        workSheet.Cells[rowIndex, 13].Value = checkMethod;
                        workSheet.Cells[rowIndex, 14].Value = item.CheckAt?.ToString("yyyy-MM-dd HH:mm");
                        workSheet.Cells[rowIndex, 15].Value = item.Checkor;
                        workSheet.Cells[rowIndex, 16].Value = checkResult;

                        workSheet.Cells[rowIndex, 1, rowIndex, 16].Style.Font.Size = 9;
                        workSheet.Cells[rowIndex, 1, rowIndex, 16].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        workSheet.Cells[rowIndex, 1, rowIndex, 16].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        workSheet.Cells[rowIndex, 1, rowIndex, 16].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        workSheet.Cells[rowIndex, 1, rowIndex, 16].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        workSheet.Cells[rowIndex, 1, rowIndex, 16].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin, Color.Black);

                        workSheet.Row(rowIndex).Height = 20;
                        rowIndex++;
                    }
                }

                package.Save();

                return $"/report/{WebUtility.UrlEncode(fileName)}";
            }
        }
    }
}