using HZC.Infrastructure;
using System.Collections.Generic;
using System.Linq;
using Zodo.Assets.Core;

namespace Zodo.Assets.Application
{
    public class AssetLogService : BaseService<AssetLog>
    {

        public PageList<AssetLogDto> PageListDto(AssetLogSearchParam param = null, int pageIndex = 1, int pageSize = 20, string cols = "*")
        {
            if (param == null)
            {
                param = new AssetLogSearchParam();
            }
            var util = param.ToSearchUtil();
            return db.Query<AssetLogDto>(util, pageIndex, pageSize, "Asset_AssetLog", cols);
        }

        public IEnumerable<AssetLogDto> ListDto(AssetLogSearchParam param = null, string cols = "*")
        {
            if (param == null)
            {
                param = new AssetLogSearchParam();
            }
            var util = param.ToSearchUtil();
            return db.Fetch<AssetLogDto>(util, "Asset_AssetLog", cols);
        }

        public override Result<int> Create(AssetLog t, IAppUser user)
        {
            var error = ValidCreate(t, user);
            if (!string.IsNullOrWhiteSpace(error))
            {
                return ResultUtil.Do(ResultCodes.验证失败, 0, error);
            }

            var types = new[] { "调配", "借出", "归还", "报废", "回收" };
            return !types.Contains(t.Type) ? ResultUtil.Do(ResultCodes.验证失败, 0, "不受支持的操作类型") : null;
        }

        #region 实体验证
        public override string ValidCreate(AssetLog entity, IAppUser user)
        {
            if (entity.TargetDeptId <= 0)
            {
                return "目标部门不能为空";
            }
            if(!ValidDate(entity.OperateAt))
            {
                return "无效的操作日期";
            }
            if (string.IsNullOrWhiteSpace(entity.Type))
            {
                return "操作类型不能为空";
            }

            return string.Empty;
        }

        public override string ValidUpdate(AssetLog entity, IAppUser user) => string.Empty;

        public override string ValidDelete(AssetLog entity, IAppUser user) => string.Empty;

        #endregion
    }
}

