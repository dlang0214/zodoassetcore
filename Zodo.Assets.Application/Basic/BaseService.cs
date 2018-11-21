using HZC.Database;
using HZC.Infrastructure;
using System;

namespace Zodo.Assets.Application
{
    public abstract class BaseService<T> where T : Entity
    {
        protected MyDbUtil db = new MyDbUtil();

        public virtual Result<int> Create(T t, IAppUser user)
        {
            try
            {
                var error = ValidCreate(t, user);
                if (!string.IsNullOrWhiteSpace(error))
                {
                    return ResultUtil.Do<int>(ResultCodes.验证失败, 0, error);
                }

                t.BeforeCreate(user);
                var id = db.Create<T>(t);
                if (id > 0)
                {
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

        public Result<int> Update(T t, IAppUser user)
        {
            try
            {
                var error = ValidUpdate(t, user);
                if (!string.IsNullOrWhiteSpace(error))
                {
                    return ResultUtil.Do<int>(ResultCodes.验证失败, 0, error);
                }

                t.BeforeUpdate(user);
                var row = db.Update<T>(t);
                if (row > 0)
                {
                    return ResultUtil.Success<int>(t.Id);
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

        public Result<int> Save(T t, IAppUser user)
        {
            if (t.Id <= 0)
            {
                return Create(t, user);
            }
            else
            {
                return Update(t, user);
            }
        }

        public Result Delete(T t, IAppUser user)
        {
            try
            {
                if (t == null)
                {
                    return ResultUtil.Do<int>(ResultCodes.数据不存在, 0, "请求的数据不存在");
                }

                var error = ValidDelete(t, user);
                if (!string.IsNullOrWhiteSpace(error))
                {
                    return ResultUtil.Do<int>(ResultCodes.验证失败, 0, error);
                }

                var row = db.Remove<T>(t.Id);
                if (row > 0)
                {
                    return ResultUtil.Success();
                }
                else
                {
                    return ResultUtil.Do(ResultCodes.数据库操作失败, "数据库写入失败");
                }
            }
            catch (Exception ex)
            {
                return ResultUtil.Exception(ex);
            }
        }

        public T Load(int id)
        {
            var entity = db.Load<T>(id);
            if (entity == null || entity.IsDel)
            {
                return null;
            }
            return entity;
        }

        protected bool ValidDate(DateTime dt)
        {
            var temp = DateTime.Parse("1900-1-1");
            return dt >= temp;
        }

        public abstract string ValidCreate(T entity, IAppUser user);

        public abstract string ValidUpdate(T entity, IAppUser user);

        public abstract string ValidDelete(T entity, IAppUser user);
    }
}
