using HZC.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Senparc.Weixin.Work.Containers;
using Senparc.Weixin.Work.Helpers;
using Zodo.Assets.Application;
using Zodo.Assets.Website.Extensions;
using Zodo.Assets.Website.Options;

namespace Zodo.Assets.Website.Controllers
{
    [WeixinUserFiler]
    public class WeixinAssetController : WeixinWorkController
    {
        private AssetService service = new AssetService();

        public IActionResult Index()
        {
            var nonce = JSSDKHelper.GetNoncestr();
            var timestamp = JSSDKHelper.GetTimestamp();
            var ticket = JsApiTicketContainer.TryGetTicket(WeixinWorkOptions.CorpId, WeixinWorkOptions.Secret);
            var signature = JSSDKHelper.GetSignature(ticket, nonce, timestamp, Request.AbsoluteUri());

            ViewBag.CorpId = WeixinWorkOptions.CorpId;
            ViewBag.Nonce = nonce;
            ViewBag.TimeStamp = timestamp;
            ViewBag.Signature = signature;

            return View();
        }

        public JsonResult Load(int id)
        {
            var entity = service.LoadDto(id);
            if (entity == null)
            {
                return Json(ResultUtil.Do(ResultCodes.数据不存在, "请求的数据不存在"));
            }
            return Json(ResultUtil.Success<AssetDto>(entity));
        }
    }
}