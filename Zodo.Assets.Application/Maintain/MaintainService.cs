using HZC.Infrastructure;
using System.Collections.Generic;
using Zodo.Assets.Core;

namespace Zodo.Assets.Application
{
    public class MaintainService : BaseService<Maintain>
    {
        public Result<int> Create2(Maintain entity, IAppUser user)
        {
            var result = Create(entity, user);
            if (result.Code != 200) return result;
            if (entity.ServiceResult != "维修中" || !entity.AssetId.HasValue) return result;
            const string sql = "UPDATE Asset_Asset SET [State]='维修',UpdateAt=GETDATE(),UpdateBy=@UserId,Updator=@UserName WHERE Id=@Id";
            db.Execute(sql, new { Id = entity.AssetId, UserName = user.Name, UserId = user.Id });
            return result;
        }

        public Result<int> Update2(Maintain entity, IAppUser user)
        {
            var result = Update(entity, user);
            if (result.Code == 200)
            {
                if (entity.ServiceResult != "维修中" || !entity.AssetId.HasValue) return result;
                const string sql = "UPDATE Asset_Asset SET State='维修',UpdateAt=GETDATE(),UpdateBy=@UserId,Updator=@UserName WHERE Id=@Id";
                db.Execute(sql, new { Id = entity.AssetId, UserName = user.Name, UserId = user.Id });
            }
            else
            {
                if (entity.ServiceResult == "维修中" || !entity.AssetId.HasValue) return result;
                const string sql = "UPDATE Asset_Asset SET State=@State,UpdateAt=GETDATE(),UpdateBy=@UserId,Updator=@UserName WHERE Id=@Id";
                db.Execute(sql, new { Id = entity.AssetId, UserName = user.Name, UserId = user.Id, State = entity.OrigState });
            }

            return result;
        }

        public PageList<MaintainDto> PageListDto(MaintainSearchParam param = null, int pageIndex = 1, int pageSize = 20, string cols = "*")
        {
            if (param == null)
            {
                param = new MaintainSearchParam();
            }
            var util = param.ToSearchUtil();
            return db.Query<MaintainDto>(util, pageIndex, pageSize, "Asset_Maintain", cols);
        }

        public IEnumerable<MaintainDto> ListDto(MaintainSearchParam param = null, string cols = "*")
        {
            if (param == null)
            {
                param = new MaintainSearchParam();
            }
            var util = param.ToSearchUtil();
            return db.Fetch<MaintainDto>(util, "Asset_Maintain", cols);
        }

        #region 实体验证
        public override string ValidCreate(Maintain entity, IAppUser user)
        {
            //if (string.IsNullOrWhiteSpace(entity.AssetName))
            //{
            //    return "维修的资产不能为空";
            //}

            //if (string.IsNullOrWhiteSpace(entity.DeptName))
            //{
            //    return "申请部门不能为空";
            //}

            //if (string.IsNullOrWhiteSpace(entity.AccountName))
            //{
            //    return "申请人不能为空";
            //}

            return string.Empty;
        }

        public override string ValidUpdate(Maintain entity, IAppUser user)
        {
            return ValidCreate(entity, user);
        }

        public override string ValidDelete(Maintain entity, IAppUser user)
        {
            return string.Empty;
        }
        #endregion

        #region 申请
        //public Result Apply(int assetId, string assetCode, string assetName, string band, string model, string imei, string origState,
        //    int deptId, string deptName, int accountId, string accountName, string position, string repairman, string repairmanName,
        //    DateTime expectedServiceTime, string serviceType, string describe)
        //{

        //}
        #endregion
    }
}

