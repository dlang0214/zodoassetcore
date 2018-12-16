using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Web;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using Zodo.Assets.Application;
using Zodo.Assets.Core;
using Zodo.Assets.Website.Models;

namespace Zodo.Assets.Website.Controllers
{
    public class ReportController : MvcController
    {
        private readonly AssetService _service = new AssetService();
        private readonly ReportService _reportService = new ReportService();

        #region 状态报表
        public IActionResult States(AssetSearchParam param)
        {
            InitUi(param);
            //param.IsContainSubDept = true;
            //var list = _service.GetStateGroup(param);
            //return View("States", list);

            var sData = _reportService.StateSummary();
            return View("State2", sData);
        }

        public ActionResult ExportStates(AssetSearchParam param)
        {
            param.IsContainSubDept = true;
            var list = _service.GetStateGroup(param);
            var path = GetDetails(list, "资产状态清单");
            return Redirect(path);
        }
        #endregion

        #region 健康度报表
        public IActionResult Healthy(AssetSearchParam param)
        {
            InitUi(param);
            //param.IsContainSubDept = true;
            //var list = _service.GetHealthyGroup(param);
            //return View("Healthy", list);

            var sData = _reportService.HealthySummary();
            return View("Healthy2", sData);
        }

        public ActionResult ExportHealthy(AssetSearchParam param)
        {
            param.IsContainSubDept = true;
            var list = _service.GetHealthyGroup(param);
            var path = GetDetails(list, "资产健康度清单");
            return Redirect(path);
        }
        #endregion

        #region 部门报表
        public IActionResult Depts(AssetSearchParam param)
        {
            InitUi(param);
            //param.IsContainSubDept = true;
            //var list = _service.GetDeptGroup(param);
            //return View("Depts", list);

            var sData = _reportService.DeptSummary();
            var depts = DeptUtil.All();

            var data =
                from cate in depts
                join dto in sData on cate.Id equals dto.Id into dds

                from dd in dds.DefaultIfEmpty()
                select new StringSummaryDto()
                {
                    Property = ShowName2(cate.Name, cate.Level),
                    Num = dd?.Num ?? 0
                };
            return View("Dept2", data.ToList());
        }

        public ActionResult ExportDepts(AssetSearchParam param)
        {
            param.IsContainSubDept = true;
            var list = _service.GetDeptGroup(param);
            var path = GetDetails(list, "部门资产清单");
            return Redirect(path);
        }
        #endregion

        #region 员工报表
        public IActionResult Accounts(AssetSearchParam param)
        {
            InitUi(param);
            param.IsContainSubDept = true;
            var list = _service.GetAccountGroup(param);
            return View("Accounts", list);
        }

        public ActionResult ExportAccounts(AssetSearchParam param)
        {
            param.IsContainSubDept = true;
            var list = _service.GetAccountGroup(param);
            var path = GetDetails(list, "员工资产清单");
            return Redirect(path);
        }
        #endregion

        #region 资产类别清单
        public IActionResult Cates(AssetSearchParam param)
        {
            InitUi(param);
            //param.IsContainSubDept = true;
            //var list = _service.GetCateGroup(param);
            //return View(list);

            var sData = _reportService.CateSummary();
            var cates = AssetCateUtil.All();

            //var data = from d in sData
            //    join cate in cates on d.Id equals cate.Id into dds

            //    from dd in dds.DefaultIfEmpty()
            //    select new StringSummaryDto()
            //    {
            //        Property = ShowName(dd.Name, dd.Level),
            //        Num = d.Num
            //    };
            var data =
                from cate in cates
                join dto in sData on cate.Id equals dto.Id into dds

                from dd in dds.DefaultIfEmpty()
                select new StringSummaryDto()
                {
                    Property = ShowName2(cate.Name, cate.Level),
                    Num = dd?.Num ?? 0
                };

            return View("Cate2", data.ToList());
        }

        public ActionResult ExportCates(AssetSearchParam param)
        {
            param.IsContainSubDept = true;
            var list = _service.GetCateGroup(param);
            var path = GetDetails(list, "资产分类清单");
            return Redirect(path);
        }
        #endregion

        #region 生成清单excel
        private string GetDetails(List<AssetGroupDto> groups, string name = "")
        {
            var fileName = (string.IsNullOrWhiteSpace(name) ? Guid.NewGuid().ToString("N") : name) + ".xlsx";
            var baseFolderName = $"{Directory.GetCurrentDirectory()}//wwwroot//report";
            if (!Directory.Exists(baseFolderName))
            {
                Directory.CreateDirectory(baseFolderName);
            }
            var savePath = $"{baseFolderName}//{fileName}";

            if (System.IO.File.Exists(savePath))
            {
                System.IO.File.Delete(savePath);
            }

            using (var package = new ExcelPackage(new FileInfo(savePath)))
            {
                var workSheet = package.Workbook.Worksheets.Add("sheet1");
                workSheet.Cells.Style.Font.Name = "microsoft yahei";
                workSheet.Cells.Style.Font.Size = 9;
                workSheet.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                workSheet.Column(3).Width = 16;
                workSheet.Column(4).Width = 24;
                workSheet.Column(5).Width = 16;
                workSheet.Column(6).Width = 16;
                workSheet.Column(7).Width = 16;
                workSheet.Column(8).Width = 16;
                workSheet.Column(9).Width = 16;
                workSheet.Column(10).Width = 16;
                workSheet.Column(11).Width = 16;
                workSheet.Column(12).Width = 16;
                workSheet.Column(13).Width = 30;
                workSheet.Column(13).Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;


                var rowIndex = 1;
                foreach (var group in groups)
                {
                    workSheet.Cells[rowIndex, 1].Value = group.GroupName;       // 标题文本
                    workSheet.Cells[rowIndex, 1].Style.Font.Bold = true;        // 标题文字加粗
                    workSheet.Cells[rowIndex, 1].Style.Font.Size = 12;          // 标题文字大小
                    workSheet.Cells[rowIndex, 1, rowIndex, 13].Merge = true;     // 合并单元格

                    workSheet.Row(rowIndex).Height = 30;

                    rowIndex++;

                    workSheet.Cells[rowIndex, 1].Value = "资产编号";
                    workSheet.Cells[rowIndex, 2].Value = "财务编号";
                    workSheet.Cells[rowIndex, 3].Value = "资产状态";
                    workSheet.Cells[rowIndex, 4].Value = "资产分类";
                    workSheet.Cells[rowIndex, 5].Value = "资产名称";
                    workSheet.Cells[rowIndex, 6].Value = "品牌";
                    workSheet.Cells[rowIndex, 7].Value = "型号";
                    workSheet.Cells[rowIndex, 8].Value = "序列号";
                    workSheet.Cells[rowIndex, 9].Value = "规格";
                    workSheet.Cells[rowIndex, 10].Value = "健康度";
                    workSheet.Cells[rowIndex, 11].Value = "使用部门";
                    workSheet.Cells[rowIndex, 12].Value = "使用人";
                    workSheet.Cells[rowIndex, 13].Value = "资产位置";

                    workSheet.Cells[rowIndex, 1, rowIndex, 13].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    workSheet.Cells[rowIndex, 1, rowIndex, 13].Style.Fill.BackgroundColor.SetColor(Color.Yellow);

                    workSheet.Cells[rowIndex, 1, rowIndex, 13].Style.Font.Bold = true;
                    workSheet.Cells[rowIndex, 1, rowIndex, 13].Style.Font.Size = 9;
                    workSheet.Cells[rowIndex, 1, rowIndex, 13].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells[rowIndex, 1, rowIndex, 13].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells[rowIndex, 1, rowIndex, 13].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells[rowIndex, 1, rowIndex, 13].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells[rowIndex, 1, rowIndex, 13].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                    workSheet.Row(rowIndex).Height = 24;

                    rowIndex++;


                    if (group.Assets.Count > 0)
                    {
                        foreach (var asset in group.Assets)
                        {
                            workSheet.Cells[rowIndex, 1].Value = asset.Code;
                            workSheet.Cells[rowIndex, 2].Value = asset.FinancialCode;
                            workSheet.Cells[rowIndex, 3].Value = asset.State;
                            workSheet.Cells[rowIndex, 4].Value = asset.AssetCate;
                            workSheet.Cells[rowIndex, 5].Value = asset.Name;
                            workSheet.Cells[rowIndex, 6].Value = asset.Band;
                            workSheet.Cells[rowIndex, 7].Value = asset.Model;
                            workSheet.Cells[rowIndex, 8].Value = asset.Imei;
                            workSheet.Cells[rowIndex, 9].Value = asset.Specification;
                            workSheet.Cells[rowIndex, 10].Value = asset.Healthy;
                            workSheet.Cells[rowIndex, 11].Value = asset.DeptName;
                            workSheet.Cells[rowIndex, 12].Value = asset.AccountName;
                            workSheet.Cells[rowIndex, 13].Value = asset.Position;

                            workSheet.Cells[rowIndex, 1, rowIndex, 13].Style.Font.Size = 9;
                            workSheet.Cells[rowIndex, 1, rowIndex, 13].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            workSheet.Cells[rowIndex, 1, rowIndex, 13].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            workSheet.Cells[rowIndex, 1, rowIndex, 13].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            workSheet.Cells[rowIndex, 1, rowIndex, 13].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            workSheet.Cells[rowIndex, 1, rowIndex, 13].Style.Border.BorderAround(ExcelBorderStyle.Thin, Color.Black);

                            workSheet.Row(rowIndex).Height = 20;
                            rowIndex++;
                        }
                    }

                    rowIndex++;
                }

                package.Save();

                return $"/report/{WebUtility.UrlEncode(fileName)}";
            }
        }
        #endregion

        private void InitUi(AssetSearchParam param)
        {
            // 状态
            ViewBag.States = AssetParameters.States.ToSelectList(param.State);
            // 健康度
            ViewBag.Healthy = AssetParameters.Healthy.ToSelectList(param.Healthy);
            // 分类
            var cates = AssetCateUtil.All();
            var cateItems = new List<SelectListItem>();
            foreach (var c in cates)
            {
                cateItems.Add(new SelectListItem { Text = ShowName(c.Name, c.Level), Value = c.Id.ToString(), Selected = (param.CateId == c.Id)});
            }
            ViewBag.Cates = cateItems;
            ViewBag.Depts = DeptUtil.GetSelectList().ToSelectList("Id", "Name", param.DeptId.ToString());
        }

        private string ShowName(string txt, int level)
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

        private string ShowName2(string txt, int level)
        {
            var str = "";
            if (level > 1)
            {
                for (var i = 0; i < level; i++)
                {
                    str += HttpUtility.HtmlDecode("&nbsp;&nbsp;&nbsp;&nbsp;");
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
}