using HZC.Database;
using HZC.Infrastructure;
using HZC.SearchUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using Zodo.Assets.Core;

namespace Zodo.Assets.Application
{
    public class DeptService
    {
        private MyDbUtil db = new MyDbUtil();

        public Result<int> Create(Dept dept, IAppUser user)
        {
            try
            {
                string error = Validate(dept);
                if (!string.IsNullOrWhiteSpace(error))
                {
                    return ResultUtil.Do<int>(ResultCodes.验证失败, 0, error);
                }

                dept.BeforeCreate(user);
                var id = db.Create<Dept>(dept);
                if (id > 0)
                {
                    DeptUtil.Clear();
                    return ResultUtil.Success<int>(id);
                }
                else
                {
                    return ResultUtil.Do<int>(ResultCodes.数据库操作失败, 0, "数据写入失败");
                }
            }
            catch (Exception ex)
            {
                return ResultUtil.Exception<int>(ex, 0);
            }
        }
         
        public Result<int> Update(Dept dept, IAppUser user)
        {
            try
            {
                string error = Validate(dept);
                if (!string.IsNullOrWhiteSpace(error))
                {
                    return ResultUtil.Do<int>(ResultCodes.验证失败, 0, error);
                }

                if (dept.ParentId == dept.Id)
                {
                    return ResultUtil.Do<int>(ResultCodes.验证失败, 0, "不能将自身设置为上级");
                }

                dept.BeforeUpdate(user);
                var row = db.Update<Dept>(dept);
                if (row > 0)
                {
                    DeptUtil.Clear();
                    return ResultUtil.Success<int>(dept.Id);
                }
                else
                {
                    return ResultUtil.Do<int>(ResultCodes.数据库操作失败, 0, "数据写入失败");
                }
            }
            catch (Exception ex)
            {
                return ResultUtil.Exception<int>(ex, 0);
            }
        }

        public Result<int> Save(Dept dept, IAppUser user)
        {
            if (dept.Id <= 0)
            {
                return Create(dept, user);
            }
            else
            {
                return Update(dept, user);
            }
        }

        public Result Delete(int id, IAppUser user)
        {
            try
            {
                var entity = db.Load<Dept>(id);
                if (entity == null)
                {
                    return ResultUtil.Do(ResultCodes.数据不存在, "请求的数据不存在");
                }

                var childrenCount = db.GetCount<Dept>(MySearchUtil.New()
                    .AndEqual("ParentId", id)
                    .AndEqual("IsDel", false));
                if (childrenCount > 0)
                {
                    return ResultUtil.Do(ResultCodes.验证失败, "下属部门不为空，禁止删除");
                }

                var accountCount = db.GetCount<Account>(MySearchUtil.New()
                    .AndEqual("DeptId", id)
                    .AndEqual("IsDel", false));
                if (accountCount > 0)
                {
                    return ResultUtil.Do(ResultCodes.验证失败, "部门内员工不为空，禁止删除");
                }

                var row = db.Remove<Dept>(id);
                if (row > 0)
                {
                    DeptUtil.Clear();
                    return ResultUtil.Success();
                }
                else
                {
                    return ResultUtil.Do(ResultCodes.数据库操作失败, "数据库写入失败");
                }
            }
            catch(Exception ex)
            {
                return ResultUtil.Exception(ex);
            }
        }

        public List<DeptDto> Fetch(DeptSearchParam input)
        {
            MySearchUtil util = input.ToSearchUtil();
            var all = db.Fetch<Dept>(util);
            return all.Select(a => new DeptDto
            {
                Id = a.Id,
                Name = a.Name,
                Sort = a.Sort,
                ParentId = a.ParentId
            }).ToList();
        }

        public Dept Load(int id)
        {
            return db.Load<Dept>(id);
        }

        private string Validate(Dept dept)
        {
            if (string.IsNullOrWhiteSpace(dept.Name))
            {
                return "部门名称不能为空";
            }

            return string.Empty;
        }
    }
}
