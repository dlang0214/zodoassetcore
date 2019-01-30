using HZC.Database;
using HZC.Infrastructure;
using HZC.SearchUtil;
using System;
using Zodo.Assets.Application;

namespace Zodo.Assets.Services
{
    public partial class ProposerService : BaseService<ProposerEntity>
    {
        #region 创建申请
        public Result Create(ProposerCreateDto dto)
        {
            var entity = AutoMapper.Mapper.Map<ProposerEntity>(dto);
            var error = ValidCreate(entity, null);
            if (!string.IsNullOrWhiteSpace(error))
            {
                return ResultUtil.Do(ResultCodes.验证失败, error);
            }
            entity.CreateAt = DateTime.Now;
            entity.Creator = dto.ProposerName;
            entity.UpdateAt = DateTime.Now;
            entity.Updator = dto.ProposerName;
            entity.Step = 1;
            entity.Status = (int)ProposerStatus.待审批;

            var id = db.Create(entity);
            return id > 0 ? ResultUtil.Success() : ResultUtil.Do(ResultCodes.数据库操作失败, "创建失败");
        }
        #endregion

        #region 审批
        public Result Approve(int id, int result, string remark, string userId, string userName)
        {
            var entity = Load(id);
            if(entity == null)
            {
                return ResultUtil.Do(ResultCodes.验证失败, "请求的申请不存在");
            }
            if(entity.Step != 1)
            {
                return ResultUtil.Do(ResultCodes.验证失败, "仅待审批的申请可以进行此操作");
            }
            if(!entity.Approvers.Contains(userId))
            {
                return ResultUtil.Do(ResultCodes.验证失败, "您无权审批此申请");
            }
            if (result == -1 && string.IsNullOrWhiteSpace(remark))
            {
                return ResultUtil.Do(ResultCodes.验证失败, "未通过申请，请注明拒绝理由");
            }

            entity.ApproveResult = result;
            entity.ApproveRemark = remark;
            entity.UpdateAt = DateTime.Now;
            entity.Updator = userName;

            var row = db.Update(KeyValuePairList.New()
                .Add("ApproveResult", entity.ApproveResult)
                .Add("ApproveRemark", entity.ApproveRemark)
                .Add("UpdateAt", DateTime.Now)
                .Add("Updator", userName), MySearchUtil.New().AndEqual("Id", id));

            var sql = "UPDATE [SC_Proposer] SET ApproveResult=@ApproveResult,ApproveRemark=@ApproveRemark,UpdateAt=getdate(),Updator=@UserName,"
        }
        #endregion

        #region 重写实体验证
        public override string ValidCreate(ProposerEntity entity, IAppUser user)
        {
            if (string.IsNullOrWhiteSpace(entity.ProposerId))
            {
                return "申请人不能为空";
            }

            if (string.IsNullOrWhiteSpace(entity.CustomerUnit) ||
                string.IsNullOrWhiteSpace(entity.CustomerName) ||
                string.IsNullOrWhiteSpace(entity.CustomerJob) ||
                string.IsNullOrWhiteSpace(entity.CustomerMobile))
            {
                return "客户单位、姓名、职位、联系方式不能为空";
            }

            if (entity.PlanDealAt == null || entity.PlanDealAt < DateTime.Parse("2019-01-01"))
            {
                return "预计成交日期无效";
            }

            if (entity.PlanAmount <= 0 || entity.PlanProfit <= 0)
            {
                return "预计成交金额和利润必须大于0";
            }

            if(entity.ServiceChargeAmount <= 0)
            {
                return "服务费金额必须大于0";
            }

            if(string.IsNullOrWhiteSpace(entity.Reason))
            {
                return "申请理由不能为空";
            }

            if(string.IsNullOrWhiteSpace(entity.Approvers))
            {
                return "审批人不能为空";
            }

            return string.Empty;
        }

        public override string ValidUpdate(ProposerEntity entity, IAppUser user)
        {
            return string.Empty;
        }

        public override string ValidDelete(ProposerEntity entity, IAppUser user)
        {
            return string.Empty;
        }
        #endregion
    }
}

