using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using Zodo.Assets.Application;

namespace Zodo.Assets.Website.Controllers
{
    public class ReportController : MvcController
    {
        private AssetService service = new AssetService();

        #region 状态报表
        public IActionResult States(AssetSearchParam param)
        {
            var list = service.GetStateGroup(param);
            return View("States", list);
        }

        public ActionResult ExportStates(AssetSearchParam param)
        {
            var list = service.GetStateGroup(param);
            var path = GetDetails(list, "资产状态清单");
            return Redirect(path);
        }
        #endregion

        #region 健康度报表
        public IActionResult Healthy(AssetSearchParam param)
        {
            var list = service.GetHealthyGroup(param);
            return View("Healthy", list);
        }

        public ActionResult ExportHealthy(AssetSearchParam param)
        {
            var list = service.GetHealthyGroup(param);
            var path = GetDetails(list, "资产健康度清单");
            return Redirect(path);
        }
        #endregion

        #region 部门报表
        public IActionResult Depts(AssetSearchParam param)
        {
            var list = service.GetDeptGroup(param);
            return View("Depts", list);
        }

        public ActionResult ExportDepts(AssetSearchParam param)
        {
            var list = service.GetDeptGroup(param);
            var path = GetDetails(list, "部门资产清单");
            return Redirect(path);
        }
        #endregion

        #region 员工报表
        public IActionResult Accounts(AssetSearchParam param)
        {
            var list = service.GetAccountGroup(param);
            return View("Accounts", list);
        }

        public ActionResult ExportAccounts(AssetSearchParam param)
        {
            var list = service.GetAccountGroup(param);
            var path = GetDetails(list, "员工资产清单");
            return Redirect(path);
        }
        #endregion

        #region 资产类别清单
        public IActionResult Cates(AssetSearchParam param)
        {
            var list = service.GetCateGroup(param);
            return View(list);
        }

        public ActionResult ExportCates(AssetSearchParam param)
        {
            var list = service.GetCateGroup(param);
            var path = GetDetails(list, "资产分类清单");
            return Redirect(path);
        }
        #endregion

        #region 生成清单excel
        private string GetDetails(List<AssetGroupDto> groups, string name = "")
        {
            string folderName = DateTime.Today.ToString("yyyyMM");
            string fileName = (string.IsNullOrWhiteSpace(name) ? Guid.NewGuid().ToString("N") : name) + ".xlsx";
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


                int rowIndex = 1;
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
                    workSheet.Cells[rowIndex, 1, rowIndex, 13].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin, Color.Black);

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
                            workSheet.Cells[rowIndex, 1, rowIndex, 13].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin, Color.Black);

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
    }
}