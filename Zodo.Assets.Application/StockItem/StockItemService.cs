using HZC.Database;
using HZC.Infrastructure;
using HZC.SearchUtil;
using System.Collections.Generic;
using Zodo.Assets.Core;
using Zodo.Assets.Services;

namespace Zodo.Assets.Application
{
    /// <summary>
    /// 盘点状态说明：0未盘点，1已盘点，2盘点异常
    /// 盘点方式说明：1扫码，2手动
    /// </summary>
    public class StockItemService
    {
        private MyDbUtil db = new MyDbUtil();

        /// <summary>
        /// 手动盘点
        /// </summary>
        /// <param name="assetId">资产id</param>
        /// <param name="stockId">盘点id</param>
        /// <param name="result">盘点结果</param>
        /// <param name="method">盘点方式，1扫码盘点|2手动盘点</param>
        /// <param name="checkor">盘点人</param>
        /// <returns></returns>
        public Result DirectCheck(int assetId, int stockId, int result, int method, string checkor)
        {
            MySearchUtil util = MySearchUtil.New()
                .AndEqual("IsDel", false)
                .AndEqual("IsFinish", false)
                .AndEqual("StockId", stockId)
                .AndEqual("AssetId", assetId);

            var item = db.Load<StockItem>(util);
            if (item == null)
            {
                return ResultUtil.Do(ResultCodes.验证失败, "请求的资产未记录在本次盘点内");
            }
            if (item.CheckResult > 0)
            {
                return ResultUtil.Do(ResultCodes.验证失败, "该资产已经盘点");
            }

            string sql = @"
                UPDATE [Asset_StockItem] SET 
                    CheckAt=GETDATE(),Checkor=@Checkor,CheckResult=@Result,
                    CheckMethod=@Method,UpdateAt=GETDATE(),Updator=@Checkor 
                WHERE Id=@Id;
                UPDATE [Asset_Asset] SET LastCheckTime=GETDATE() WHERE Id=@AssetId;";
            var row = db.Execute(sql, new { Checkor = checkor, Result = result, Method = method, Id = item.Id, AssetId = item.AssetId });

            return row > 0 ? ResultUtil.Success() : ResultUtil.Do(ResultCodes.数据库操作失败, "操作失败");
        }

        /// <summary>
        /// 获取盘点项
        /// </summary>
        /// <param name="assetId">资产id</param>
        /// <param name="stockId">盘点id</param>
        /// <returns></returns>
        public Result<StockItem> Load(int assetId, int stockId)
        {
            MySearchUtil util = MySearchUtil.New()
                .AndEqual("IsDel", false)
                .AndEqual("IsFinish", false)
                .AndEqual("StockId", stockId)
                .AndEqual("AssetId", assetId);

            var item = db.Load<StockItem>(util);
            return item == null ? ResultUtil.Do<StockItem>(ResultCodes.数据不存在, null) : ResultUtil.Success<StockItem>(item);
        }

        /// <summary>
        /// 获取盘点项详情，包含资产的部分参数
        /// </summary>
        /// <param name="assetId">资产id</param>
        /// <param name="stockId">盘点id</param>
        /// <returns></returns>
        public Result LoadDto(int assetId, int stockId)
        {
            var entity = db.LoadBySql<StockItemDto>(@"
                SELECT * FROM StockItemView WHERE AssetId=@AssetId AND StockId=@StockId AND IsDel=0",
                new { AssetId = assetId, StockId = stockId });
            return entity == null ? ResultUtil.Do<StockItemDto>(ResultCodes.数据不存在, null) : ResultUtil.Success<StockItemDto>(entity);
        }

        /// <summary>
        /// 盘点
        /// </summary>
        /// <param name="stockItemId">盘点项id</param>
        /// <param name="result">盘点结果，0未盘点|1盘点成功|2盘点异常</param>
        /// <param name="method">盘点方式</param>
        /// <param name="checkor">盘点人</param>
        /// <param name="remark">备注</param>
        /// <returns></returns>
        public Result Check(int stockItemId, int result, int method, string checkor, string remark)
        {
            var entity = Load(stockItemId);
            if (entity == null)
            {
                return ResultUtil.Do(ResultCodes.数据不存在, "请求的记录不存在");
            }

            string sql = @"
                UPDATE [Asset_StockItem] SET 
                    CheckAt=GETDATE(),Checkor=@Checkor,CheckResult=@Result,
                    CheckMethod=@Method,UpdateAt=GETDATE(),Updator=@Checkor,Remark=@Remark
                WHERE Id=@Id;
                UPDATE [Asset_Asset] SET LastCheckTime=GETDATE() WHERE Id=@AssetId;";
            var row = db.Execute(sql, new { Checkor = checkor, Result = result, Method = method, Id = stockItemId, AssetId = entity.AssetId, Remark = remark });

            return row > 0 ? ResultUtil.Success() : ResultUtil.Do(ResultCodes.数据库操作失败, "操作失败");
        }

        /// <summary>
        /// 加载盘点项
        /// </summary>
        /// <param name="id">盘点项id</param>
        /// <returns></returns>
        public StockItem Load(int id)
        {
            return db.Load<StockItem>(id);
        }

        /// <summary>
        /// 获取符合条件的所有盘点项
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public IEnumerable<StockItemDto> FetchDto(StockItemSearchParam param)
        {
            MySearchUtil util = param.ToSearchUtil();
            return db.Fetch<StockItemDto>(util, "StockItemView");
        }
    }
}
