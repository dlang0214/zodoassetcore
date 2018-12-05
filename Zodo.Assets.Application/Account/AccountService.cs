using HZC.Database;
using HZC.Infrastructure;
using HZC.SearchUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using Zodo.Assets.Core;

namespace Zodo.Assets.Application
{
    public class AccountService
    {
        private readonly MyDbUtil _db = new MyDbUtil();

        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public Result<int> Create(Account entity, IAppUser user)
        {
            try
            {
                var error = Validate(entity);
                if (!string.IsNullOrWhiteSpace(error))
                {
                    return ResultUtil.Do<int>(ResultCodes.验证失败, 0, error);
                }

                entity.BeforeCreate(user);
                var id = _db.Create<Account>(entity);
                return id > 0 ? ResultUtil.Success<int>(id) : ResultUtil.Do<int>(ResultCodes.数据库操作失败, 0, "数据写入失败");
            }
            catch (Exception ex)
            {
                return ResultUtil.Exception<int>(ex, 0);
            }
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public Result<int> Update(Account entity, IAppUser user)
        {
            try
            {
                var error = Validate(entity);
                if (!string.IsNullOrWhiteSpace(error))
                {
                    return ResultUtil.Do(ResultCodes.验证失败, 0, error);
                }

                entity.BeforeUpdate(user);
                var row = _db.Update(entity);
                if (row <= 0) return ResultUtil.Do(ResultCodes.数据库操作失败, 0, "数据写入失败");
                _db.Execute(@"UPDATE [Asset_Asset] SET DeptId=@DeptId WHERE AccountId=@AccountId",
                    new {AccountId = entity.Id, DeptId = entity.DeptId});
                return ResultUtil.Success(entity.Id);
            }
            catch (Exception ex)
            {
                return ResultUtil.Exception(ex, 0);
            }
        }

        /// <summary>
        /// 保存；id > 0 更新数据，id == 0 添加数据；
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public Result<int> Save(Account entity, IAppUser user)
        {
            return entity.Id <= 0 ? Create(entity, user) : Update(entity, user);
        }

        /// <summary>
        /// 移除数据
        /// </summary>
        /// <param name="id"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public Result Delete(int id, IAppUser user)
        {
            try
            {
                var entity = _db.Load<Account>(id);
                if (entity == null)
                {
                    return ResultUtil.Do(ResultCodes.数据不存在, "请求的数据不存在");
                }

                var assetCount = _db.GetCount<Asset>(MySearchUtil.New()
                    .AndEqual("AccountId", id)
                    .AndEqual("IsDel", false));
                if (assetCount > 0)
                {
                    return ResultUtil.Do(ResultCodes.验证失败, "此用户下资产不为空，禁止删除");
                }

                var row = _db.Remove<Account>(id);
                return row > 0 ? ResultUtil.Success() : ResultUtil.Do(ResultCodes.数据库操作失败, "数据库写入失败");
            }
            catch (Exception ex)
            {
                return ResultUtil.Exception(ex);
            }
        }

        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <param name="input"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public PageList<AccountListDto> PageList(AccountSearchParam input, int pageIndex, int pageSize)
        {
            var util = input.ToSearchUtil();
            return _db.Query<AccountListDto>(util, pageIndex, pageSize, "AccountView");
        }

        /// <summary>
        /// 加载实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Account Load(int id)
        {
            var entity = _db.Load<Account>(id);
            if (entity == null || entity.IsDel)
            {
                return null;
            }

            return entity;
        }

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Result Delete(int id)
        {
            var account = Load(id);
            if (account == null)
            {
                return ResultUtil.Do(ResultCodes.验证失败, "请求的数据不存在");
            }

            var assetCount = _db.GetCount<Asset>(MySearchUtil.New()
                .AndEqual("AccountId", id)
                .AndEqual("IsDel", false)
                .AndNotEqual("State", "报废"));

            if (assetCount > 0)
            {
                return ResultUtil.Do(ResultCodes.验证失败, "此用户下资产不为空");
            }

            var row = _db.Remove<Account>(id);
            return row > 0 ? ResultUtil.Success() : ResultUtil.Do(ResultCodes.数据库操作失败);
        }

        /// <summary>
        /// 获取用户基本信息
        /// </summary>
        /// <param name="deptId">部门id</param>
        /// <returns></returns>
        public List<AccountBaseDto> GetAccountsBaseInfo(int? deptId, string key = "")
        {
            var param = new AccountSearchParam
            {
                IsStrict = true
            };
            if (deptId.HasValue)
            {
                param.Dept = (int)deptId;
            }
            if (!string.IsNullOrWhiteSpace(key))
            {
                param.Key = key;
            }
            return _db.Fetch<AccountBaseDto>(param.ToSearchUtil(), "Base_Account", "Id,Name,DeptId").ToList();
        }

        #region 私有方法
        private string Validate(Account entity)
        {
            return string.IsNullOrWhiteSpace(entity.Name) ? "员工姓名不能为空" : string.Empty;
        }
        #endregion
    }
}
