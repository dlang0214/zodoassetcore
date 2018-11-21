using System;
using System.Collections.Generic;
using System.Text;
using HZC.Infrastructure;
using Zodo.Assets.Core;

namespace Zodo.Assets.Application
{
    public class ServiceApplicationService : BaseService<ServiceApplication>
    {
        //public Result<int> Create(ServiceApplication entity, IAppUser user)
        //{
        //    var error = ValidCreate(entity, user);
        //    if (!string.IsNullOrWhiteSpace(error))
        //    {
        //        return ResultUtil.Do<int>(ResultCodes.验证失败, 0, error);
        //    }

        //    entity.BeforeCreate(user);
        //    var id = db.Create<ServiceApplication>(entity);
        //    return id > 0 ? ResultUtil.Success<int>(id) : ResultUtil.Do<int>(ResultCodes.数据库操作失败, 0);
        //}

        public PageList<ServiceApplication> List(ServiceApplicationSearchParam param, int pageIndex, int pageSize)
        {
            var pageList = db.Query<ServiceApplication>(param.ToSearchUtil(), pageIndex, pageSize);
            return pageList;
        }

        public override string ValidCreate(ServiceApplication entity, IAppUser user)
        {
            if (entity.DeptId <= 0)
            {
                return "申请部门不能为空";
            }

            if (string.IsNullOrWhiteSpace(entity.AccountName))
            {
                return "申请人不能为空";
            }

            if (entity.RequireCompleteAt == null || !ValidDate(entity.RequireCompleteAt))
            {
                return "要求办结时间不合法";
            }

            if (entity.ApplyAt == null || !ValidDate(entity.ApplyAt))
            {
                return "申请日期不合法";
            }

            return string.Empty;
        }

        public override string ValidDelete(ServiceApplication entity, IAppUser user)
        {
            return string.Empty;
        }

        public override string ValidUpdate(ServiceApplication entity, IAppUser user)
        {
            return string.Empty;
        }
    }
}
