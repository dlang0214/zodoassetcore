using HZC.Database;
using HZC.Infrastructure;
using HZC.SearchUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using Zodo.Assets.Core;

namespace Zodo.Assets.Application
{
    public class AssetService : BaseService<Asset>
    {
        #region 加载一个Dto
        /// <summary>
        /// 加载一个Dto
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public AssetDto LoadDto(int id)
        {
            var entity = db.LoadBySql<AssetDto>("SELECT * FROM AssetView WHERE Id=@Id AND IsDel=0", new { Id = id });
            return entity;
        }

        public AssetDto LoadDto(string code)
        {
            var entity = db.LoadBySql<AssetDto>("SELECT * FROM AssetView WHERE Code=@Code AND IsDel=0", new { Code = code });
            return entity;
        }

        public Asset Load(string code)
        {
            return db.Load<Asset>(MySearchUtil.New().AndEqual("Code", code).AndEqual("IsDel", false));
        }
        #endregion

        #region 返回列表
        public PageList<AssetDto> PageListDto(AssetSearchParam param = null, int pageIndex = 1, int pageSize = 20, string cols = "*")
        {
            if (param == null)
            {
                param = new AssetSearchParam();
            }
            var util = param.ToSearchUtil();
            return db.Query<AssetDto>(util, pageIndex, pageSize, "AssetView", cols);
        }

        public IEnumerable<AssetDto> ListDto(AssetSearchParam param = null, string cols = "*")
        {
            if (param == null)
            {
                param = new AssetSearchParam();
            }
            var util = param.ToSearchUtil();
            return db.Fetch<AssetDto>(util, table: "AssetView", cols: cols);
        }
        #endregion

        #region 返回报废资产列表
        public PageList<AssetDto> ScrapAssets(AssetSearchParam param, int pageIndex, int pageSize)
        {
            param.State = "报废";
            param.IncludeScrap = true;
            return db.Query<AssetDto>(param.ToSearchUtil(), pageIndex, pageSize, "AssetView");
        }
        #endregion

        #region 返回选择Dto列表
        public List<AssetSelectDto> FetchSelectDto(int cateId = 0, string key = "", int? deptId = null)
        {
            var param = new AssetSearchParam();
            if (cateId > 0)
            {
                param.CateId = cateId;
            }
            if (deptId.HasValue)
            {
                if (deptId > 0)
                {
                    param.IsContainSubDept = true;
                    param.DeptId = deptId;
                }
                else
                {
                    param.IsContainSubDept = false;
                    param.DeptId = 0;
                }
            }
            if (!string.IsNullOrWhiteSpace(key))
            {
                param.Key = key;
            }

            return db.Fetch<AssetSelectDto>(param.ToSearchUtil(), "Asset_Asset", "Id, Name, Code, State").ToList();
        }
        #endregion

        #region 资产调配

        /// <summary>
        /// 转移资产
        /// </summary>
        public Result Move(AssetLog log, string newPosition, IAppUser user)
        {
            var asset = LoadDto(log.AssetId);
            if (asset == null)
            {
                return ResultUtil.Do(ResultCodes.数据不存在, "指定的资产不存在或已删除");
            }
            switch (asset.State)
            {
                case "报废":
                    return ResultUtil.Do(ResultCodes.数据不存在, "指定的资产已报废，禁止操作");
                case "借出":
                    return ResultUtil.Do(ResultCodes.数据不存在, "指定的资产已借出，禁止操作");
            }

            log.AssetCode = asset.Code;
            log.AssetName = asset.Name;

            if (log.OperateAt < DateTime.Parse("1900-1-1"))
            {
                return ResultUtil.Do(ResultCodes.验证失败, "调配日期无效");
            }

            DeptDto targetDept;

            if (log.TargetAccountId > 0)
            {
                var accountService = new AccountService();
                var targetAccount = accountService.Load(log.TargetAccountId);

                if (targetAccount == null)
                {
                    return ResultUtil.Do(ResultCodes.数据不存在, "目标用户不存在");
                }

                targetDept = DeptUtil.Get(targetAccount.DeptId);
                if (targetDept == null)
                {
                    throw new Exception("用户所在的部门信息不存在，请联系管理员");
                }

                log.TargetAccountId = targetAccount.Id;
                log.TargetAccountName = targetAccount.Name;
                log.TargetDeptId = targetDept.Id;
                log.TargetDeptName = targetDept.Name;
            }
            else
            {
                targetDept = DeptUtil.Get(log.TargetDeptId);
                if (targetDept == null)
                {
                    return ResultUtil.Do(ResultCodes.数据不存在, "目标部门不存在");
                }
                else
                {
                    log.TargetDeptId = targetDept.Id;
                    log.TargetDeptName = targetDept.Name;
                    log.TargetAccountId = 0;
                    log.TargetAccountName = "";
                }
            }

            log.Type = "调配";

            var sql = new KeyValuePairList
            {
                {
                    "UPDATE Asset_Asset SET DeptId=@DeptId,@Position=@Position,AccountId=@AccountId,UpdateAt=GETDATE(),UpdateBy=@UserId,Updator=@UserName WHERE Id=@Id",
                    new { Id = log.AssetId, Position = newPosition, DeptId = log.TargetDeptId, AccountId = log.TargetAccountId, UserId = user.Id, UserName = user.Name }
                }
            };
            log.BeforeCreate(user);
            sql.Add(db.GetCommonInsertSql<AssetLog>(), log);

            var row = db.ExecuteTran(sql);
            return row ? ResultUtil.Success() : ResultUtil.Do(ResultCodes.数据库操作失败, "数据库写入失败");
        }
        #endregion

        #region 资产报废
        public Result Scrap(int assetId, DateTime operateAt, string pics, string remark, IAppUser user)
        {
            var asset = LoadDto(assetId);
            if (asset == null)
            {
                return ResultUtil.Do(ResultCodes.数据不存在, "资产不存在或已删除");
            }
            if (asset.State == "借出")
            {
                return ResultUtil.Do(ResultCodes.验证失败, "该资产处于借出状态，禁止报废");
            }

            var log = new AssetLog
            {
                AssetId = asset.Id,
                AssetName = asset.Name,
                AssetCode = asset.Code,
                FromAccountId = asset.AccountId,
                FromAccountName = asset.AccountName,
                FromDeptId = asset.DeptId,
                FromDeptName = asset.DeptName,
                TargetAccountId = 0,
                TargetAccountName = "",
                TargetDeptId = 0,
                TargetDeptName = "",
                Type = "报废",
                OperateAt = operateAt,
                Remark = remark,
                Pics = pics
            };
            log.BeforeCreate(user);

            var sqls = new KeyValuePairList
            {
                {
                    "UPDATE Asset_Asset SET [State]='报废',UpdateAt=GETDATE(),UpdateBy=@UserId,Updator=@UserName WHERE Id=@Id",
                    new { Id = log.AssetId, UserId = user.Id, UserName = user.Name }
                }
            };
            log.BeforeCreate(user);
            sqls.Add(db.GetCommonInsertSql<AssetLog>(), log);

            var row = db.ExecuteTran(sqls);
            return row ? ResultUtil.Success() : ResultUtil.Do(ResultCodes.数据库操作失败, "数据库写入失败");
        }
        #endregion

        #region 资产借出
        public Result Loan(int assetId, int targetDeptId, int targetAccountId, DateTime loanAt, DateTime expectedReturnAt, string pics, string newPositon, string remark, IAppUser user)
        {
            // 验证参数
            if (assetId <= 0)
            {
                return ResultUtil.Do(ResultCodes.验证失败, "指定了无效的资产id");
            }
            if (targetDeptId <= 0)
            {
                return ResultUtil.Do(ResultCodes.验证失败, "指定了无效的部门id");
            }
            if (loanAt < DateTime.Parse("1900-1-1") || expectedReturnAt < DateTime.Parse("1900-1-1"))
            {
                return ResultUtil.Do(ResultCodes.验证失败, "借出和预计归还日期必须大于1900-1-1");
            }
            if (loanAt > expectedReturnAt)
            {
                return ResultUtil.Do(ResultCodes.验证失败, "预计归还日期必须大于等于借出日期");
            }

            var loan = new Loan();
            var asset = LoadDto(assetId);
            if (asset == null)
            {
                return ResultUtil.Do(ResultCodes.数据不存在, "资产不存在或已删除"); 
            }
            if (asset.State == "借出" || asset.State == "报废")
            {
                return ResultUtil.Do(ResultCodes.验证失败, "该资产已借出或已报废，禁止借出操作");
            }

            if (targetAccountId > 0)
            {
                var accountService = new AccountService();
                var account = accountService.Load(targetAccountId);

                if (account == null)
                {
                    return ResultUtil.Do(ResultCodes.数据不存在, "目标使用人不存在或已删除");
                }

                var dept = DeptUtil.Get(account.DeptId);
                if (dept == null)
                {
                    throw new Exception("无法找到员工所属部门。员工ID:" + account.Id.ToString() + "；部门ID:" + account.DeptId.ToString());
                }

                loan.TargetAccountId = targetAccountId;
                loan.TargetAccountName = account.Name;
                loan.TargetDeptId = targetDeptId;
                loan.TargetDeptName = dept.Name;
            }
            else
            {
                var dept = DeptUtil.Get(targetDeptId);
                if (dept == null)
                {
                    return ResultUtil.Do(ResultCodes.数据不存在, "目标部门不存在或已删除");
                }
                loan.TargetAccountId = 0;
                loan.TargetAccountName = "";
                loan.TargetDeptId = dept.Id;
                loan.TargetDeptName = dept.Name;
            }

            loan.AssetId = asset.Id;
            loan.AssetCode = asset.Code;
            loan.AssetName = asset.Name;
            loan.FromAccountId = asset.AccountId;
            loan.FromAccountName = asset.AccountName;
            loan.FromDeptId = asset.DeptId;
            loan.FromDeptName = asset.DeptName;
            loan.LoanAt = loanAt;
            loan.ExpectedReturnAt = expectedReturnAt;
            loan.IsReturn = false;
            loan.ReturnAt = null;
            loan.Pics = pics;

            var log = new AssetLog
            {
                AssetId = loan.AssetId,
                AssetCode = loan.AssetCode,
                AssetName = loan.AssetName,
                FromAccountId = loan.FromAccountId,
                FromAccountName = loan.FromAccountName,
                FromDeptId = loan.FromDeptId,
                FromDeptName = loan.FromDeptName,
                TargetAccountId = loan.TargetAccountId,
                TargetAccountName = loan.TargetAccountName,
                TargetDeptId = loan.TargetDeptId,
                TargetDeptName = loan.TargetDeptName,
                Type = "借出",
                OperateAt = loan.LoanAt,
                Pics = loan.Pics
            };

            loan.BeforeCreate(user);
            log.BeforeCreate(user);

            var sqls = new KeyValuePairList
            {
                {
                    "UPDATE Asset_Asset SET [State]='借出',Position=@Position,DeptId=@DeptId,AccountId=@AccountId,UpdateAt=GETDATE(),UpdateBy=@UserId,Updator=@UserName WHERE Id=@Id",
                    new
                    {
                        Id = log.AssetId,
                        Position = newPositon,
                        UserId = user.Id,
                        UserName = user.Name,
                        AccountId = log.TargetAccountId,
                        DeptId = log.TargetDeptId
                    }
                },
                {db.GetCommonInsertSql<Loan>(), loan},
                {db.GetCommonInsertSql<AssetLog>(), log}
            };

            var row = db.ExecuteTran(sqls);
            return row ? ResultUtil.Success() : ResultUtil.Do(ResultCodes.数据库操作失败, "数据库写入失败");
        }
        #endregion

        #region 资产归还
        public Result Return(int loanId, DateTime returnAt, IAppUser user)
        {
            var loan = db.Load<Loan>(loanId);
            if (loan == null)
            {
                return ResultUtil.Do(ResultCodes.数据不存在, "借出记录不存在");
            }
            if (loan.IsReturn)
            {
                return ResultUtil.Do(ResultCodes.验证失败, "此借出记录已归还，请勿重复操作");
            }

            var state = loan.FromDeptId == 0 ? "闲置" : "使用中";

            var log = new AssetLog
            {
                AssetId = loan.AssetId,
                AssetCode = loan.AssetCode,
                AssetName = loan.AssetName,
                TargetAccountId = loan.FromAccountId,
                TargetAccountName = loan.FromAccountName,
                TargetDeptId = loan.FromDeptId,
                TargetDeptName = loan.FromDeptName,
                FromAccountId = loan.TargetAccountId,
                FromAccountName = loan.TargetAccountName,
                FromDeptId = loan.TargetDeptId,
                FromDeptName = loan.TargetDeptName,
                Type = "归还",
                OperateAt = loan.LoanAt,
                Pics = loan.Pics
            };
            log.BeforeCreate(user);

            var sqls = new KeyValuePairList
            {
                {
                    "UPDATE Asset_Asset SET [State]=@State,DeptId=@DeptId,Position=@Position,AccountId=@AccountId,UpdateAt=GETDATE(),UpdateBy=@UserId,Updator=@UserName WHERE Id=@Id",
                    new
                    {
                        Id = loan.AssetId,
                        State = state,
                        DeptId = loan.FromDeptId,
                        AccountId = loan.FromAccountId,
                        UserID = user.Id,
                        UserName = user.Name,
                        Position = loan.FromPosition
                    }
                },
                {
                    "UPDATE Asset_Loan SET IsReturn=1,ReturnAt=@ReturnAt,UpdateAt=GETDATE(),UpdateBy=@UserId,Updator=@UserName WHERE Id=@Id",
                    new {Id = loanId, UserId = user.Id, UserName = user.Name, ReturnAt = returnAt}
                },
                {db.GetCommonInsertSql<AssetLog>(), log}
            };

            var row = db.ExecuteTran(sqls);
            return row ? ResultUtil.Success() : ResultUtil.Do(ResultCodes.数据库操作失败, "数据库写入失败");
        }
        #endregion

        #region 回收|闲置
        public Result Recovery(int assetId, DateTime recoveryAt, string pics, string newPosition, string remark, IAppUser user)
        {
            var asset = LoadDto(assetId);
            if (asset == null)
            {
                return ResultUtil.Do(ResultCodes.数据不存在, "请求的资产不存在");
            }

            if (recoveryAt < DateTime.Parse("1900-1-1"))
            {
                return ResultUtil.Do(ResultCodes.验证失败, "回收日期不得小于1900-1-1");
            }

            var log = new AssetLog
            {
                AssetId = asset.Id,
                AssetName = asset.Name,
                AssetCode = asset.Code,
                FromAccountId = asset.AccountId,
                FromAccountName = asset.AccountName,
                FromDeptId = asset.DeptId,
                FromDeptName = asset.DeptName,
                TargetAccountId = 0,
                TargetAccountName = "",
                TargetDeptId = 0,
                TargetDeptName = "",
                Type = "回收",
                OperateAt = recoveryAt,
                Remark = remark + "\n回收后位置：" + newPosition,
                Pics = pics
            };
            log.BeforeCreate(user);

            var sqls = new KeyValuePairList
            {
                {
                    "UPDATE Asset_Asset SET [State]='闲置',Position=@Position,DeptId=0,AccountId=0,UpdateAt=GETDATE(),UpdateBy=@UserId,Updator=@UserName WHERE Id=@Id",
                    new {Id = log.AssetId, UserID = user.Id, UserName = user.Name, Position = newPosition}
                },
                {db.GetCommonInsertSql<AssetLog>(), log}
            };

            var row = db.ExecuteTran(sqls);
            return row ? ResultUtil.Success() : ResultUtil.Do(ResultCodes.数据库操作失败, "数据库写入失败");
        }
        #endregion

        #region 分类报表
        public List<AssetGroupDto> GetCateGroup(AssetSearchParam param)
        {
            var all = ListDto(param).ToList();
            var groups = all.Select(a => a.AssetCateId).Distinct();

            var cates = AssetCateUtil.All();
            return groups.Select(g => new AssetGroupDto
            {
                GroupId = g,
                GroupName = cates.SingleOrDefault(c=>c.Id==g)?.Name,
                Assets = all.Where(a => a.AssetCateId == g).ToList()
            }).ToList();
        }
        #endregion

        #region 部门报表
        public List<AssetGroupDto> GetDeptGroup(AssetSearchParam param)
        {
            var all = ListDto(param).ToList();
            var groups = all.Select(a => a.DeptName).Distinct();

            return groups.Select(g => new AssetGroupDto
            {
                GroupName = string.IsNullOrWhiteSpace(g) ? "闲置物品" : g, Assets = all.Where(a => a.DeptName == g).ToList()
            }).ToList();
        }
        #endregion

        #region 用户报表
        public List<AssetGroupDto> GetAccountGroup(AssetSearchParam param)
        {
            var all = ListDto(param).ToList();
            var groups = all.Select(a => a.AccountName).Distinct();

            return groups.Where(g => !string.IsNullOrWhiteSpace(g))
                .Select(g => new AssetGroupDto {GroupName = g, Assets = all.Where(a => a.AccountName == g).ToList()})
                .ToList();
        }
        #endregion

        #region 状态报表
        public List<AssetGroupDto> GetStateGroup(AssetSearchParam param)
        {
            var all = ListDto(param).ToList();
            var groups = all.Select(a => a.State).Distinct();

            return groups.Select(g => new AssetGroupDto
                {
                    GroupName = string.IsNullOrWhiteSpace(g) ? "其他状态" : g,
                    Assets = all.Where(a => a.State == g).ToList()
                })
                .ToList();
        }
        #endregion

        #region 健康度报表
        public List<AssetGroupDto> GetHealthyGroup(AssetSearchParam param)
        {
            var all = ListDto(param).ToList();
            var groups = all.Select(a => a.Healthy).Distinct();

            return groups.Select(g =>
            {
                var dto = new AssetGroupDto
                {
                    Assets = all.Where(a => a.Healthy == g).ToList(),
                    GroupName = string.IsNullOrWhiteSpace(g) ? "其他" : g
                };
                return dto;
            }).ToList();
        }
        #endregion

        #region 实体验证
        public override string ValidCreate(Asset entity, IAppUser user) => ValidUpdate(entity, user);

        public override string ValidUpdate(Asset entity, IAppUser user)
        {
            if (string.IsNullOrWhiteSpace(entity.Name))
            {
                return "资产名称不能为空";
            }
            //if (string.IsNullOrWhiteSpace(entity.Source))
            //{
            //    return "采购来源不能为空";
            //}
            //if (string.IsNullOrWhiteSpace(entity.Position))
            //{
            //    return "资产位置不能为空";
            //}
            //if (entity.AssignDate == null)
            //{
            //    return "登记时间不能为空";
            //}
            if (entity.AssetCateId <= 0)
            {
                return "资产类型不能为空";
            }
            if (!string.IsNullOrWhiteSpace(entity.Code))
            {
                var count = db.GetCount<Asset>(MySearchUtil.New()
                    .AndEqual("Code", entity.Code)
                    .AndEqual("IsDel", false)
                    .AndNotEqual("Id", entity.Id));
                if (count > 0)
                {
                    return "资产编号已存在";
                }
            }
            return string.Empty;
        }

        public override string ValidDelete(Asset entity, IAppUser user)
        {
            return string.Empty;
        }
        #endregion

        #region 维修
        public Result<int> Maintain(Maintain entity, IAppUser user)
        {
            var service = new MaintainService();
            return service.Create2(entity, user);
        }
        #endregion
    }
}

