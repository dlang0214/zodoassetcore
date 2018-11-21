using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HZC.Database;
using HZC.Infrastructure;
using HZC.SearchUtil;
using Zodo.Assets.Core;
using Zodo.Assets.Services;

namespace Zodo.Assets.Application
{
    public class StockService : BaseService<Stock>
    {
        #region 创建
        /// <summary>
        /// 创建，已废弃
        /// </summary>
        /// <param name="t"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        [Obsolete("原始方法，已弃用")]
        public override Result<int> Create(Stock t, IAppUser user)
        {
            t.IsFinish = false;
            var error = ValidCreate(t, user);
            if (!string.IsNullOrWhiteSpace(error))
            {
                return ResultUtil.Do<int>(ResultCodes.验证失败, 0, error);
            }
            t.BeforeCreate(user);
            //KeyValuePairList sqls = new KeyValuePairList();
            //sqls.Add(db.GetCommonInsertSql<Stock>(), t);
            //sqls.Add("UPDATE Asset_Stock SET IsFinish=1,UpdateAt=GETDATE(),UpdateBy=@UserId,Updator=@UserName WHERE IsFinish=0", new
            //{
            //    UserId = user.Id,
            //    UserName = user.Name
            //});
            //sqls.Add("UPDATE Asset_StockItem SET IsFinish=1,UpdateAt=GETDATE(),UpdateBy=@UserId,Updator=@UserName WHERE IsFinish=0", new
            //{
            //    UserId = user.Id,
            //    UserName = user.Name
            //});
            //var result = db.ExecuteTran(sqls);
            var id = db.Create<Stock>(t);

            string sql = @"
                INSERT INTO [Asset_StockItem] (
                    StockId,IsFinish,AssetId,AssetCode,AssetName,DeptId,DeptName,AccountId,AccountName,Position,
                    CheckAt,CheckBy,Checkor,CheckResult,CheckMethod,Remark,IsDel,CreateAt,CreateBy,Creator,UpdateAt,UpdateBy,Updator) 
                SELECT @StockId,0,Id,Code,Name,DeptId,DeptName,AccountId,AccountName,Position,null,null,null,0,null,'',0,
                    GETDATE(),@UserId,@UserName,GETDATE(),@UserId,@UserName FROM [AssetView] 
                WHERE Code<>'' AND Code IS NOT NULL AND IsDel=0 AND [State]<>'报废'";
            db.Execute(sql, new { StockId = id, UserId = user.Id, UserName = user.Name });

            return id > 0 ? ResultUtil.Success<int>(0) : ResultUtil.Do<int>(ResultCodes.数据库操作失败, 0);
        }

        /// <summary>
        /// 创建一个盘点
        /// </summary>
        /// <param name="t"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public Result<int> Create2(Stock t, IAppUser user)
        {
            t.IsFinish = false;
            var error = ValidCreate(t, user);
            if (!string.IsNullOrWhiteSpace(error))
            {
                return ResultUtil.Do<int>(ResultCodes.验证失败, 0, error);
            }
            t.BeforeCreate(user);
            var id = db.Create<Stock>(t);
            return id > 0 ? ResultUtil.Success<int>(0) : ResultUtil.Do<int>(ResultCodes.数据库操作失败, 0);
        }

        /// <summary>
        /// 配置盘点明细
        /// </summary>
        /// <param name="stockId">盘点ID</param>
        /// <param name="assetIds">要盘点的资产ID数组</param>
        /// <param name="user">记录创建人</param>
        /// <returns></returns>
        public Result SetItems(int stockId, int[] assetIds, IAppUser user)
        {
            string sql = "SELECT AssetId FROM Asset_StockItem WHERE StockId=@Id AND IsDel=0";
            var ids = db.FetchBySql<int>(sql, new { Id = stockId });
            List<int> realIds = new List<int>();
            if (ids.Count() > 0)
            {
                realIds = assetIds.Where(id => !ids.Contains(id)).ToList();
            }
            else
            {
                realIds = assetIds.ToList();
            }
            if (realIds.Count == 0)
            {
                return ResultUtil.Do(ResultCodes.数据不存在, "所选资产已存在");
            }
            else
            {
                sql = @"
                    INSERT INTO [Asset_StockItem] (
                        StockId,IsFinish,AssetId,AssetCode,AssetName,DeptId,DeptName,AccountId,AccountName,Position,
                        CheckAt,CheckBy,Checkor,CheckResult,CheckMethod,Remark,IsDel,CreateAt,CreateBy,Creator,UpdateAt,UpdateBy,Updator,
                        FinancialCode,Healthy,[State]) 
                    SELECT @StockId,0,Id,Code,Name,DeptId,DeptName,AccountId,AccountName,Position,null,null,null,0,null,'',0,
                        GETDATE(),@UserId,@UserName,GETDATE(),@UserId,@UserName,FinancialCode,Healthy,State FROM [AssetView] 
                    WHERE IsDel=0 AND Id IN @Ids";
                var rows = db.Execute(sql, new { StockId = stockId, Ids = realIds, UserId = user.Id, UserName = user.Name });
                return rows > 0 ? ResultUtil.Success<int>(rows) : ResultUtil.Do(ResultCodes.数据库操作失败, "数据库操作失败");
            }
        }
        #endregion

        #region 关闭盘点
        /// <summary>
        /// 关闭盘点
        /// </summary>
        /// <param name="id">盘点ID</param>
        /// <param name="user">操作人</param>
        /// <returns></returns>
        public Result Finish(int id, IAppUser user)
        {
            var entity = Load(id);
            if (entity == null)
            {
                return ResultUtil.Do(ResultCodes.数据不存在, "请求的数据不存在");
            }
            if (entity.IsFinish)
            {
                return ResultUtil.Do(ResultCodes.验证失败, "该盘点已结束，请勿重复操作");
            }

            KeyValuePairList sqls = new KeyValuePairList();
            sqls.Add("UPDATE Asset_Stock SET IsFinish=1,FinishAt=GETDATE(),UpdateAt=GETDATE(),UpdateBy=@UserId,Updator=@UserName WHERE Id=@Id", new
            {
                Id = id,
                UserId = user.Id,
                UserName = user.Name
            });
            sqls.Add("UPDATE Asset_StockItem SET IsFinish=1,UpdateAt=GETDATE(),UpdateBy=@UserId,Updator=@UserName WHERE StockId=@Id", new
            {
                Id = id,
                UserId = user.Id,
                UserName = user.Name
            });
            var result = db.ExecuteTran(sqls);

            return result ? ResultUtil.Success<int>(0) : ResultUtil.Do<int>(ResultCodes.数据库操作失败, 0);
        }
        #endregion

        #region 删除
        public Result Delete(int id, IAppUser user)
        {
            var entity = Load(id);
            var error = ValidDelete(entity, user);
            if (!string.IsNullOrWhiteSpace(error))
            {
                return ResultUtil.Do(ResultCodes.验证失败, error);
            }

            KeyValuePairList sqls = new KeyValuePairList();
            sqls.Add("UPDATE Asset_Stock SET IsDel=1,UpdateAt=GETDATE(),UpdateBy=@UserId,Updator=@UserName WHERE Id=@Id", new
            {
                Id = id,
                UserId = user.Id,
                UserName = user.Name
            });
            sqls.Add("UPDATE Asset_StockItem SET IsDel=1,UpdateAt=GETDATE(),UpdateBy=@UserId,Updator=@UserName WHERE StockId=@Id", new
            {
                Id = id,
                UserId = user.Id,
                UserName = user.Name
            });
            var result = db.ExecuteTran(sqls);

            return result ? ResultUtil.Success() : ResultUtil.Do(ResultCodes.数据库操作失败);
        }
        #endregion

        #region 分页列表
        public PageList<Stock> Query(int pageIndex, int pageSize, bool? isFinish = null, string key = "")
        {
            MySearchUtil util = MySearchUtil.New().OrderByDesc("UpdateAt").AndEqual("IsDel", false);
            if (isFinish.HasValue)
            {
                util.AndEqual("IsFinish", isFinish.Value);
            }
            if (!string.IsNullOrWhiteSpace(key))
            {
                util.AndContains(new string[] { "Title", "Remark" }, key.Trim());
            }
            return db.Query<Stock>(util, pageIndex, pageSize);
        }
        #endregion
        
        #region 加载
        public Stock LoadWithItems(int id)
        {
            var result = db.LoadWith<Stock, StockItem>(@"
                SELECT * FROM Asset_Stock WHERE Id=@Id;SELECT * FROM Asset_StockItem WHERE StockId=@Id;",
                (stock, items) =>
                {
                    if (stock != null)
                    {
                        stock.Items = items;
                    }
                    return stock;
                }, new { Id = id });

            return result;
        }

        public IEnumerable<StockItem> Items(StockItemSearchParam param)
        {
            var util = param.ToSearchUtil();
            return db.Fetch<StockItem>(util);
        }

        public PageList<StockItem> PageListItems(StockItemSearchParam param, int pageIndex = 1, int pageSize = 20)
        {
            var util = param.ToSearchUtil();
            return db.Query<StockItem>(util, pageIndex, pageSize);
        }
        #endregion

        #region 验证相关

        public int RunningStockCount()
        {
            MySearchUtil util = new MySearchUtil();
            util.AndEqual("IsDel", false).AndEqual("IsFinish", false);

            return db.GetCount<Stock>(util);
        }

        public override string ValidCreate(Stock entity, IAppUser user)
        {
            if (string.IsNullOrWhiteSpace(entity.Title))
            {
                return "盘点标题不能为空";
            }

            //if (RunningStockCount() > 0)
            //{
            //    return "上次盘点尚未完成";
            //}

            return string.Empty;
        }

        public override string ValidDelete(Stock entity, IAppUser user)
        {
            if (entity == null)
            {
                return "请求的数据不存在";
            }
            if (entity.IsFinish)
            {
                return "盘点已结束，禁止删除";
            }
            return string.Empty;
        }

        public override string ValidUpdate(Stock entity, IAppUser user)
        {
            return string.Empty;
        } 
        #endregion
    }
}
