using HZC.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Zodo.Assets.Application;
using Zodo.Assets.Core;

namespace Zodo.Assets.Website.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WeixinApiController : ControllerBase
    {
        #region 服务申请
        public Result ServiceApply(Maintain maintain)
        {
            if (string.IsNullOrWhiteSpace(maintain.RepairMan))
            {
                return ResultUtil.Do(ResultCodes.验证失败, "未知来源，此功能仅限企业微信用户使用");
            }

            if (string.IsNullOrWhiteSpace(maintain.AssetName))
            {
                return ResultUtil.Do(ResultCodes.验证失败, "物品不能为空");
            }

            var service = new MaintainService();
            return service.Create2(maintain, new AppUserDto { Id = 0, Name = "WeixinApi" });
        }
        #endregion

        [HttpGet]
        public Result GetAssetByCode(string code)
        {
            AssetService service = new AssetService();
            var asset = service.LoadDto(code);
            return asset == null ? ResultUtil.Do(ResultCodes.数据不存在) : ResultUtil.Success<AssetDto>(asset);
        }
    }
}