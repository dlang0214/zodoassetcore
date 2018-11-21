using HZC.Database;
using HZC.Infrastructure;
using HZC.SearchUtil;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Senparc.Weixin.Work.Containers;
using Senparc.Weixin.Work.Helpers;
using Zodo.Assets.Application;
using Zodo.Assets.Core;
using Zodo.Assets.Website.Extensions;
using Zodo.Assets.Website.Options;

namespace Zodo.Assets.Website.Controllers
{
    [WeixinUserFiler]
    public class WeixinStockController : WeixinWorkController
    {
        private StockItemService service = new StockItemService();
        private MyDbUtil db = new MyDbUtil();

        #region 首页
        /// <summary>
        /// 未完成盘点列表
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            string sql = "SELECT * FROM Asset_Stock WHERE IsDel=0 AND IsFinish=0";
            var stock = db.FetchBySql<Stock>(sql);
            return View(stock);
        }
        #endregion
        
        #region 盘点详情
        /// <summary>
        /// 盘点详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult Details(int id)
        {
            var entity = db.Load<Stock>(id);
            if (entity == null)
            {
                return RedirectToAction("Error", new { title = "403", message = "您无权查看此数据" });
            }
            return View(entity);
        }
        #endregion
        
        #region 扫描二维码和设置盘点结果
        /// <summary>
        /// 二维码扫描和设置盘点结果
        /// </summary>
        /// <param name="id">盘点id</param>
        /// <returns></returns>
        public IActionResult Check(int id)
        {
            var stockService = new StockService();
            var stock = stockService.Load(id);
            if (stock == null || stock.IsFinish || stock.IsDel)
            {
                return RedirectToAction("Error", new { title = "403", message = "请求的盘点不存在或已完成" });
            }

            var nonce = JSSDKHelper.GetNoncestr();
            var timestamp = JSSDKHelper.GetTimestamp();
            var ticket = JsApiTicketContainer.TryGetTicket(WeixinWorkOptions.CorpId, WeixinWorkOptions.Secret);
            var signature = JSSDKHelper.GetSignature(ticket, nonce, timestamp, Request.AbsoluteUri());

            ViewBag.CorpId = WeixinWorkOptions.CorpId;
            ViewBag.Nonce = nonce;
            ViewBag.TimeStamp = timestamp;
            ViewBag.Signature = signature;

            return View(stock);
        } 

        /// <summary>
        /// 获取盘点明细
        /// </summary>
        /// <param name="id">资产id</param>
        /// <param name="stockId">盘点id</param>
        /// <returns></returns>
        public JsonResult Load(int id, int stockId)
        {
            var result = service.LoadDto(id, stockId);
            return Json(result);
        }

        /// <summary>
        /// 设置盘点结果
        /// </summary>
        /// <param name="id"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Check(int id, int result, string remark)
        {
            if (result > 3 || result < 0)
            {
                return Json(ResultUtil.Do(ResultCodes.验证失败, "无效的盘点结果"));
            }
            var r = service.Check(id, result, 1, WxUser.UserName, remark);
            return Json(r);
        }

        /// <summary>
        /// 获取最近盘点的资产
        /// </summary>
        /// <param name="id"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public JsonResult GetLastCheckItems(int id, int count = 20)
        {
            MySearchUtil util = MySearchUtil.New()
                .AndNotNullOrEmpty("CheckAt")
                .OrderByDesc("CheckAt");

            var list = db.Fetch(util, "Asset_StockItem", "DeptName,AccountName,AssetName,AssetCode,Checkor,CheckAt", count);
            return Json(list);
        }
        #endregion

        #region 错误页面
        /// <summary>
        /// 显示错误
        /// </summary>
        /// <param name="title">错误标题</param>
        /// <param name="message">错误说明</param>
        /// <returns></returns>
        public IActionResult Error(string title, string message)
        {
            ViewData["Title"] = string.IsNullOrWhiteSpace(title) ? "操作失败" : title;
            ViewData["Message"] = message;
            return View();
        }
        #endregion
    }
}