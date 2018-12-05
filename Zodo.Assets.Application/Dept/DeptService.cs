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
        private readonly MyDbUtil _db = new MyDbUtil();

        public Result<int> Create(Dept dept, IAppUser user)
        {
            try
            {
                var error = Validate(dept);
                if (!string.IsNullOrWhiteSpace(error))
                {
                    return ResultUtil.Do(ResultCodes.验证失败, 0, error);
                }

                dept.BeforeCreate(user);
                var id = _db.Create(dept);
                if (id <= 0)
                {
                    return ResultUtil.Do(ResultCodes.数据库操作失败, 0, "数据写入失败");
                }

                DeptUtil.Clear();
                return ResultUtil.Success(id);
            }
            catch (Exception ex)
            {
                return ResultUtil.Exception(ex, 0);
            }
        }
         
        public Result<int> Update(Dept dept, IAppUser user)
        {
            try
            {
                var error = Validate(dept);
                if (!string.IsNullOrWhiteSpace(error))
                {
                    return ResultUtil.Do(ResultCodes.验证失败, 0, error);
                }

                if (dept.ParentId == dept.Id)
                {
                    return ResultUtil.Do(ResultCodes.验证失败, 0, "不能将自身设置为上级");
                }

                dept.BeforeUpdate(user);
                var row = _db.Update(dept);
                if (row <= 0)
                {
                    return ResultUtil.Do(ResultCodes.数据库操作失败, 0, "数据写入失败");
                }

                DeptUtil.Clear();
                return ResultUtil.Success(dept.Id);
            }
            catch (Exception ex)
            {
                return ResultUtil.Exception(ex, 0);
            }
        }

        public Result<int> Save(Dept dept, IAppUser user)
        {
            return dept.Id <= 0 ? Create(dept, user) : Update(dept, user);
        }

        public Result Delete(int id, IAppUser user)
        {
            try
            {
                var entity = _db.Load<Dept>(id);
                if (entity == null)
                {
                    return ResultUtil.Do(ResultCodes.数据不存在, "请求的数据不存在");
                }

                var childrenCount = _db.GetCount<Dept>(MySearchUtil.New()
                    .AndEqual("ParentId", id)
                    .AndEqual("IsDel", false));
                if (childrenCount > 0)
                {
                    return ResultUtil.Do(ResultCodes.验证失败, "下属部门不为空，禁止删除");
                }

                var accountCount = _db.GetCount<Account>(MySearchUtil.New()
                    .AndEqual("DeptId", id)
                    .AndEqual("IsDel", false));
                if (accountCount > 0)
                {
                    return ResultUtil.Do(ResultCodes.验证失败, "部门内员工不为空，禁止删除");
                }

                var row = _db.Remove<Dept>(id);
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
            var util = input.ToSearchUtil();
            var all = _db.Fetch<Dept>(util);
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
            return _db.Load<Dept>(id);
        }

        private string Validate(Dept dept)
        {
            return string.IsNullOrWhiteSpace(dept.Name) ? "部门名称不能为空" : string.Empty;
        }
    }
}
