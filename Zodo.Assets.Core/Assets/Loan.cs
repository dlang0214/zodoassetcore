using HZC.Database;
using System;

namespace Zodo.Assets.Core
{
    [MyDataTable("Asset_Loan")]
    public class Loan : Entity
    {
        /// <summary>
        /// 资产ID
        /// </summary>
        public int AssetId { get; set; }

        /// <summary>
        /// 资产编号
        /// </summary>
        public string AssetCode { get; set; }

        /// <summary>
        /// 资产名称
        /// </summary>
        public string AssetName { get; set; }

        /// <summary>
        /// 来源部门
        /// </summary>
        public int FromDeptId { get; set; }

        /// <summary>
        /// 来源部门名称
        /// </summary>
        public string FromDeptName { get; set; }

        /// <summary>
        /// 来源位置
        /// </summary>
        public string FromPosition { get; set; }

        /// <summary>
        /// 原使用人
        /// </summary>
        public int FromAccountId { get; set; }

        /// <summary>
        /// 原使用人姓名
        /// </summary>
        public string FromAccountName { get; set; }

        /// <summary>
        /// 目标部门
        /// </summary>
        public int TargetDeptId { get; set; }

        /// <summary>
        /// 目标部门名称
        /// </summary>
        public string TargetDeptName { get; set; }

        /// <summary>
        /// 目标位置
        /// </summary>
        public string TargetPosition { get; set; }

        /// <summary>
        /// 目标员工
        /// </summary>
        public int TargetAccountId { get; set; }

        /// <summary>
        /// 目标员工姓名
        /// </summary>
        public string TargetAccountName { get; set; }

        /// <summary>
        /// 图片
        /// </summary>
        public string Pics { get; set; }

        /// <summary>
        /// 借出日期
        /// </summary>
        public DateTime LoanAt { get; set; }

        /// <summary>
        /// 预计归还日期
        /// </summary>
        public DateTime ExpectedReturnAt { get; set; }

        /// <summary>
        /// 是否归还
        /// </summary>
        public bool IsReturn { get; set; }

        /// <summary>
        /// 归还日期
        /// </summary>
        public DateTime? ReturnAt { get; set; }

        /// <summary>
        /// 借出原因
        /// </summary>
        public string Remark { get; set; }
    }
}
