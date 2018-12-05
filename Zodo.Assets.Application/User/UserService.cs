using HZC.Database;
using HZC.Infrastructure;
using HZC.SearchUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using Zodo.Assets.Core;

namespace Zodo.Assets.Application
{
    public class UserService
    {
        private readonly MyDbUtil _db = new MyDbUtil();

        #region 创建
        public Result<int> Create(User u, IAppUser user)
        {
            var error = Validate(u);
            if (!string.IsNullOrWhiteSpace(error))
            {
                return ResultUtil.Do(ResultCodes.验证失败, 0, error);
            }
            u.Pw = AESEncriptUtil.Encrypt("123456");
            u.Version = Guid.NewGuid().ToString("N");
            u.BeforeCreate(user);
            var id = _db.Create(u);
            return id > 0 ?  ResultUtil.Success(id) : ResultUtil.Do(ResultCodes.数据库操作失败, 0);
        }
        #endregion

        #region 修改
        public Result<int> Update(User u, IAppUser user)
        {
            var error = Validate(u);
            if (!string.IsNullOrWhiteSpace(error))
            {
                return ResultUtil.Do(ResultCodes.验证失败, 0, error);
            }
            
            u.Version = Guid.NewGuid().ToString("N");
            u.BeforeUpdate(user);
            var id = _db.Update(u);
            return id > 0 ? ResultUtil.Success(u.Id) : ResultUtil.Do(ResultCodes.数据库操作失败, 0);
        }
        #endregion

        #region 保存
        public Result<int> Save(User u, IAppUser user)
        {
            return u.Id <= 0 ? Create(u, user) : Update(u, user);
        }
        #endregion

        #region 删除
        public Result Delete(int id)
        {
            var user = Load(id);
            if (user == null)
            {
                return ResultUtil.Do(ResultCodes.验证失败, "请求的数据不存在");
            }
            if (user.Name == "admin")
            {
                return ResultUtil.Do(ResultCodes.验证失败, "admin用户禁止删除");
            }

            var row = _db.Remove<User>(id);
            return row > 0 ? ResultUtil.Success() : ResultUtil.Do(ResultCodes.数据库操作失败);
        }
        #endregion

        #region 重置密码
        public Result ResetPw(int id, IAppUser u)
        {
            var user = Load(id);
            if (user == null)
            {
                return ResultUtil.Do(ResultCodes.验证失败, "请求的数据不存在");
            }
            user.Version = Guid.NewGuid().ToString("N");
            user.Pw = AESEncriptUtil.Encrypt("123456");
            user.BeforeUpdate(u);

            return Update(user, u);
        }
        #endregion

        #region 加载实体
        public User Load(int id)
        {
            return _db.Load<User>(id);
        }
        #endregion

        #region 列表
        public List<AppUserDto> Fetch(UserSearchParam param)
        {
            return _db.Fetch<User>(param.ToSearchUtil()).Select(u => new AppUserDto
            {
                Id = u.Id,
                Name = u.Name
            }).ToList();
        }
        #endregion

        #region 修改密码
        public Result ChangePw(int id, string oldPw, string newPw)
        {
            var entity = Load(id);
            if (entity == null || entity.IsDel)
            {
                return ResultUtil.Do(ResultCodes.验证失败, "请求的用户不存在或已删除");
            }

            if (AESEncriptUtil.Decrypt(entity.Pw) != oldPw)
            {
                return ResultUtil.Do(ResultCodes.验证失败, "原始密码不正确");
            }

            entity.Pw = AESEncriptUtil.Encrypt(newPw);
            const string sql = "UPDATE Base_User SET Pw=@Pw WHERE Id=@Id";
            var row = _db.Execute(sql, new { Id = id, entity.Pw });
            return row > 0 ? ResultUtil.Success() : ResultUtil.Do(ResultCodes.数据库操作失败);
        }
        #endregion

        #region 登录
        public User Login(string name, string pw)
        {
            return _db.Load<User>(MySearchUtil.New()
                .AndEqual("Name", name)
                .AndEqual("IsDel", false));
        }
        #endregion

        #region 初始化admin
        public User GetAdmin()
        {
            var u = _db.Load<User>(MySearchUtil.New()
                .AndEqual("Name", "admin"));
            return u;
        }
        #endregion

        #region 私有方法
        private string Validate(User entity)
        {
            if (string.IsNullOrWhiteSpace(entity.Name))
            {
                return "用户名称不能为空";
            }

            var count = _db.GetCount<User>(MySearchUtil.New()
                .AndEqual("Name", entity.Name.Trim())
                .AndNotEqual("Id", entity.Id)
                .AndEqual("IsDel", false));
            return count > 0 ? "用户已存在" : string.Empty;
        }
        #endregion
    }
}
