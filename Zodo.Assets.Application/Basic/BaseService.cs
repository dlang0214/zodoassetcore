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
                    return ResultUtil.Do(ResultCodes.验证失败, 0, error);
                }

                t.BeforeCreate(user);
                var id = db.Create(t);
                return id > 0 ? ResultUtil.Success(id) : ResultUtil.Do(ResultCodes.数据库操作失败, 0, "数据写入失败");
            }
            catch (Exception ex)
            {
                return ResultUtil.Exception(ex, 0);
            }
        }

        public Result<int> Update(T t, IAppUser user)
        {
            try
            {
                var error = ValidUpdate(t, user);
                if (!string.IsNullOrWhiteSpace(error))
                {
                    return ResultUtil.Do(ResultCodes.验证失败, 0, error);
                }

                t.BeforeUpdate(user);
                var row = db.Update(t);
                return row > 0 ? ResultUtil.Success(t.Id) : ResultUtil.Do(ResultCodes.数据库操作失败, 0, "数据写入失败");
            }
            catch (Exception ex)
            {
                return ResultUtil.Exception(ex, 0);
            }
        }

        public Result<int> Save(T t, IAppUser user)
        {
            return t.Id <= 0 ? Create(t, user) : Update(t, user);
        }

        public Result Delete(T t, IAppUser user)
        {
            try
            {
                if (t == null)
                {
                    return ResultUtil.Do(ResultCodes.数据不存在, 0, "请求的数据不存在");
                }

                var error = ValidDelete(t, user);
                if (!string.IsNullOrWhiteSpace(error))
                {
                    return ResultUtil.Do(ResultCodes.验证失败, 0, error);
                }

                var row = db.Remove<T>(t.Id);
                return row > 0 ? ResultUtil.Success() : ResultUtil.Do(ResultCodes.数据库操作失败, "数据库写入失败");
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
