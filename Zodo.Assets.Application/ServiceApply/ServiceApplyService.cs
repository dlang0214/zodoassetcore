using HZC.Database;
using HZC.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using Zodo.Assets.Core;

namespace Zodo.Assets.Application
{
    public class ServiceApplyService
    {
        private readonly MyDbUtil _db = new MyDbUtil();
        
        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public Result<int> Create(ServiceApply entity)
        {
            entity.ReceiveAt = null;
            entity.CompleteAt = null;
            entity.State = "待处理";
            entity.CreateAt = DateTime.Now;
            entity.ServiceManId = null;
            entity.ServiceManName = null;
            entity.Reply = "";
            entity.Score = "--";

            var id = _db.Create(entity);
            return id > 0 ? ResultUtil.Success(id) : ResultUtil.Do(ResultCodes.数据库操作失败, 0);
        }

        /// <summary>
        /// 设置申请为接受状态
        /// </summary>
        /// <param name="id">申请id</param>
        /// <param name="userId">实施人ID</param>
        /// <param name="userName">实施人姓名</param>
        /// <returns></returns>
        public Result Receive(int id, string userId, string userName)
        {
            const string sql =
                "UPDATE [Asset_ServiceApply] SET [State]='已接受',ReceiveAt=GETDATE(),ServiceManId=@UserId,ServiceManName=@UserName WHERE Id=@Id";
            var row = _db.Execute(sql, new { Id = id, UserId = userId, UserName = userName });
            return row > 0 ? ResultUtil.Success() : ResultUtil.Do(ResultCodes.数据库操作失败, "数据库操作失败");
        }

        /// <summary>
        /// 设置申请为完成状态
        /// </summary>
        /// <param name="id">申请id</param>
        /// <param name="userId">完成人id</param>
        /// <param name="userName">完成人姓名</param>
        /// <returns></returns>
        public Result Complete(int id, string userId, string userName)
        {
            const string sql =
                "UPDATE [Asset_ServiceApply] SET [State]='待评价',CompleteAt=GETDATE(),ServiceManId=@UserId,ServiceManName=@UserName WHERE Id=@Id";
            var row = _db.Execute(sql, new { Id = id, UserId = userId, UserName = userName });
            return row > 0 ? ResultUtil.Success() : ResultUtil.Do(ResultCodes.数据库操作失败, "数据库操作失败");
        }

        /// <summary>
        /// 服务评价
        /// </summary>
        /// <param name="id">申请id</param>
        /// <param name="score">评价内容</param>
        /// <param name="userid">评价人微信的userid</param>
        /// <param name="reply">评价说明</param>
        /// <returns></returns>
        public Result Score(int id, string score, string userid, string reply)
        {
            var entity = Load(id);
            if (entity == null)
            {
                return ResultUtil.Do(ResultCodes.数据不存在, "请求的数据不存在");
            }

            if (entity.State == "待处理" || entity.State == "已接受")
            {
                return ResultUtil.Do(ResultCodes.验证失败, "该服务尚未完成，禁止评分");
            }

            if (userid != entity.UserId)
            {
                return ResultUtil.Do(ResultCodes.验证失败, "仅申请人可执行此操作");
            }

            const string sql = "UPDATE [Asset_ServiceApply] SET [State]='已评价',Score=@Score,Reply=@Reply WHERE Id=@Id";
            var row = _db.Execute(sql, new { Id = id, Score = score, Reply = reply });
            return row > 0 ? ResultUtil.Success() : ResultUtil.Do(ResultCodes.数据库操作失败, "数据库操作失败");
        }

        /// <summary>
        /// 添加备注
        /// </summary>
        /// <param name="id"></param>
        /// <param name="reason"></param>
        /// <param name="analysis"></param>
        /// <param name="solution"></param>
        /// <returns></returns>
        public Result Remark(int id, string reason, string analysis, string solution)
        {
            const string sql = "UPDATE [Asset_ServiceApply] SET Reason=@Reason,Analysis=@Analysis,Solution=@Solution WHERE Id=@Id";
            var row = _db.Execute(sql, new { Id = id, Reason = reason, Analysis = analysis, Solution = solution });
            return row > 0 ? ResultUtil.Success() : ResultUtil.Do(ResultCodes.数据库操作失败, "数据库操作失败");
        }

        /// <summary>
        /// 填写故障原因
        /// </summary>
        /// <param name="id">申请id</param>
        /// <param name="reason">故障原因</param>
        /// <returns></returns>
        public Result SetReason(int id, string reason)
        {
            const string sql = "UPDATE [Asset_ServiceApply] SET Reason=@Reason WHERE Id=@Id";
            var row = _db.Execute(sql, new { Id = id, Reason = reason });
            return row > 0 ? ResultUtil.Success() : ResultUtil.Do(ResultCodes.数据库操作失败, "数据库操作失败");
        }

        /// <summary>
        /// 填写解决方案
        /// </summary>
        /// <param name="id">申请id</param>
        /// <param name="solution">解决方案</param>
        /// <returns></returns>
        public Result SetSolution(int id, string solution)
        {
            const string sql = "UPDATE [Asset_ServiceApply] SET Solution=@Solution WHERE Id=@Id";
            var row = _db.Execute(sql, new { Id = id, Solution = solution });
            return row > 0 ? ResultUtil.Success() : ResultUtil.Do(ResultCodes.数据库操作失败, "数据库操作失败");
        }

        /// <summary>
        /// 加载一个服务申请
        /// </summary>
        /// <param name="id">申请id</param>
        /// <returns></returns>
        public ServiceApply Load(int id)
        {
            return _db.Load<ServiceApply>(id);
        }

        /// <summary>
        /// 服务申请的分页列表
        /// </summary>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="pageSize">每页条数</param>
        /// <param name="param">查询参数</param>
        /// <returns></returns>
        public PageList<ServiceApply> PageList(int pageIndex, int pageSize, ServiceApplySearchParam param)
        {
            var util = param.ToSearchUtil();
            return _db.Query<ServiceApply>(util, pageIndex, pageSize);
        }

        public List<ServiceApply> Fetch(ServiceApplySearchParam param)
        {
            var util = param.ToSearchUtil();
            return _db.Fetch<ServiceApply>(util).ToList();
        }
    }
}
