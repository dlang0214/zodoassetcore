using HZC.Infrastructure;
using HZC.SearchUtil;
using System.Collections.Generic;
using Zodo.Assets.Core;

namespace Zodo.Assets.Application
{
    public class DataItemService : BaseService<DataItem>
    {
        public override Result<int> Create(DataItem entity, IAppUser user)
        {
            var error = ValidCreate(entity, user);
            if (!string.IsNullOrWhiteSpace(error))
            {
                return ResultUtil.Do(ResultCodes.验证失败, 0, "键已存在");
            }

            entity.BeforeCreate(user);
            var row = db.Create(entity);
            if (row > 0)
            {
                DataItemUtil.Clear();
            }
            return row > 0 ? ResultUtil.Success(row) : ResultUtil.Do(ResultCodes.数据库操作失败, 0);
        }

        public Result Create(string key, string value, IAppUser user)
        {
            var entity = new DataItem
            {
                K = key,
                V = value
            };
            return Create(entity, user);
        }

        public Result Update(string key, string value, IAppUser user)
        {
            const string sql = "UPDATE [Base_DataItem] SET V=@V,UpdateAt=GETDATE(),UpdateBy=@UserId,Updator=@UserName WHERE K=@K";
            var row = db.Execute(sql, new { K = key, V = value, UserId = user.Id, UserName = user.Name });
            if (row > 0)
            {
                DataItemUtil.Clear();
            }
            return row > 0 ? ResultUtil.Success(row) : ResultUtil.Do(ResultCodes.数据库操作失败);
        }

        public Result Remove(string key)
        {
            const string sql = "UPDATE [Base_DataItem] SET IsDel=1 WHERE K=@K";
            var row = db.Execute(sql, new { K = key });
            if (row > 0)
            {
                DataItemUtil.Clear();
            }
            return row > 0 ? ResultUtil.Success(row) : ResultUtil.Do(ResultCodes.数据库操作失败);
        }

        public DataItem Load(string key)
        {
            return db.Load<DataItem>(MySearchUtil.New().AndEqual("K", key).AndEqual("IsDel", false));
        }

        public IEnumerable<DataItem> Fetch()
        {
            return db.Fetch<DataItem>(MySearchUtil.New().AndEqual("IsDel", false));
        }

        public override string ValidCreate(DataItem entity, IAppUser user)
        {
            if (string.IsNullOrWhiteSpace(entity.K))
            {
                return "键不能为空";
            }

            var count = db.GetCount<DataItem>(MySearchUtil.New().AndEqual("K", entity.K).AndEqual("IsDel", false));
            return count > 0 ? "键已存在" : string.Empty;
        }

        public override string ValidUpdate(DataItem entity, IAppUser user) => string.Empty;

        public override string ValidDelete(DataItem entity, IAppUser user) => string.Empty;
    }
}
