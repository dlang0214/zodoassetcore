using HZC.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Web;
using Zodo.Assets.Application;
using Zodo.Assets.Core;

namespace Zodo.Assets.Website.Controllers
{
    [Authorize]
    public class ServiceApplicationController : Controller
    {
        private ServiceApplyService service = new ServiceApplyService();

        #region 首页
        public IActionResult Index()
        {
            ViewBag.States = DataItemUtil.GetValues("ServiceStates").ToSelectList();
            ViewBag.Types = DataItemUtil.GetValues("ServiceTypes").ToSelectList();
            return View();
        }

        public JsonResult Get(ServiceApplySearchParam param, int pageIndex = 1, int pageSize = 20)
        {
            var pageList = service.PageList(pageIndex, pageSize, param);
            return Json(ResultUtil.PageList<ServiceApply>(pageList));
        }
        #endregion

        public IActionResult Export(ServiceApplySearchParam param)
        {
            var list = service.Fetch(param);
            var path = SaveExcel(list, "服务申请记录");
            return Redirect(path);
        }

        #region 私有方法
        private void InitUI()
        {
            var depts = DeptUtil.GetSelectList();
            var list = depts.ToSelectList("Id", "Name");
            ViewBag.Parents = list;
        }

        private string SaveExcel(List<ServiceApply> groups, string name)
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
                workSheet.Column(1).Width = 16;
                workSheet.Column(2).Width = 18;
                workSheet.Column(3).Width = 24;
                workSheet.Column(5).Width = 40;
                workSheet.Column(6).Width = 20;
                workSheet.Column(7).Width = 20;
                workSheet.Column(8).Width = 20;
                workSheet.Column(9).Width = 20;
                workSheet.Column(10).Width = 20;
                workSheet.Column(11).Width = 16;
                workSheet.Column(12).Width = 16;
                workSheet.Column(13).Width = 16;

                int rowIndex = 1;

                workSheet.Cells[rowIndex, 1].Value = "服务申请记录";       // 标题文本
                workSheet.Cells[rowIndex, 1].Style.Font.Bold = true;        // 标题文字加粗
                workSheet.Cells[rowIndex, 1].Style.Font.Size = 12;          // 标题文字大小
                workSheet.Cells[rowIndex, 1, rowIndex, 13].Merge = true;     // 合并单元格
                workSheet.Cells[rowIndex, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;   // 居中

                workSheet.Row(rowIndex).Height = 30;

                rowIndex++;

                workSheet.Cells[rowIndex, 1].Value = "申请人";
                workSheet.Cells[rowIndex, 2].Value = "申请部门";
                workSheet.Cells[rowIndex, 3].Value = "申请类型";
                workSheet.Cells[rowIndex, 4].Value = "资产编号";
                workSheet.Cells[rowIndex, 5].Value = "问题描述";
                workSheet.Cells[rowIndex, 6].Value = "申请日期";
                workSheet.Cells[rowIndex, 7].Value = "期望完成日期";
                workSheet.Cells[rowIndex, 8].Value = "受理日期";
                workSheet.Cells[rowIndex, 9].Value = "完成日期";
                workSheet.Cells[rowIndex, 10].Value = "服务确认日期";
                workSheet.Cells[rowIndex, 11].Value = "受理人";
                workSheet.Cells[rowIndex, 12].Value = "状态";
                workSheet.Cells[rowIndex, 13].Value = "服务评价";

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

                if (groups.Count > 0)
                {
                    foreach (var item in groups)
                    {
                        workSheet.Cells[rowIndex, 1].Value = item.UserName;
                        workSheet.Cells[rowIndex, 2].Value = item.DeptName;
                        workSheet.Cells[rowIndex, 3].Value = item.Type;
                        workSheet.Cells[rowIndex, 4].Value = item.AssetCode;
                        workSheet.Cells[rowIndex, 5].Value = item.Describe;
                        workSheet.Cells[rowIndex, 6].Value = item.ApplyAt.ToString("yyyy-MM-dd HH:mm");
                        workSheet.Cells[rowIndex, 7].Value = item.RequireCompleteAt?.ToString("yyyy-MM-dd HH:mm");
                        workSheet.Cells[rowIndex, 8].Value = item.ReceiveAt?.ToString("yyyy-MM-dd HH:mm");
                        workSheet.Cells[rowIndex, 9].Value = item.CompleteAt?.ToString("yyyy-MM-dd HH:mm");
                        workSheet.Cells[rowIndex, 10].Value = item.ConfirmAt?.ToString("yyyy-MM-dd HH:mm");
                        workSheet.Cells[rowIndex, 11].Value = item.ServiceManName;
                        workSheet.Cells[rowIndex, 12].Value = item.State;
                        workSheet.Cells[rowIndex, 13].Value = item.Score;

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

                package.Save();

                return $"/report/{WebUtility.UrlEncode(fileName)}";
            }
            #endregion
        }
    }
}