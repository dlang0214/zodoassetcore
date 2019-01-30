using HZC.Database;
using System;

namespace Zodo.Assets.Services
{
    [MyDataTable("SC_Proposer")]
    public partial class ProposerEntity : Entity
    {
        /// <summary>
        /// 申请人ID
        /// </summary>
        public string ProposerId { get; set; }

        /// <summary>
        /// 申请人
        /// </summary>
        public string ProposerName { get; set; }

        /// <summary>
        /// 客户单位
        /// </summary>
        public string CustomerUnit { get; set; }

        /// <summary>
        /// 客户姓名
        /// </summary>
        public string CustomerName { get; set; }

        /// <summary>
        /// 客户职位
        /// </summary>
        public string CustomerJob { get; set; }

        /// <summary>
        /// 客户联系方式
        /// </summary>
        public string CustomerMobile { get; set; }

        /// <summary>
        /// 项目名称
        /// </summary>
        public string ProjectName { get; set; }

        /// <summary>
        /// 预计成交你时间
        /// </summary>
        public DateTime PlanDealAt { get; set; }

        /// <summary>
        /// 预计金额
        /// </summary>
        public decimal PlanAmount { get; set; }

        /// <summary>
        /// 预计利润
        /// </summary>
        public decimal PlanProfit { get; set; }

        /// <summary>
        /// 服务费金额
        /// </summary>
        public decimal ServiceChargeAmount { get; set; }

        /// <summary>
        /// 申请原因
        /// </summary>
        public string Reason { get; set; }

        /// <summary>
        /// 审批人
        /// </summary>
        public string Approvers { get; set; }

        /// <summary>
        /// 审批人ID
        /// </summary>
        public string ApproverId { get; set; }

        /// <summary>
        /// 审批人
        /// </summary>
        public string ApproverName { get; set; }

        /// <summary>
        /// 审批时间
        /// </summary>
        public DateTime ApproveAt { get; set; }

        /// <summary>
        /// ApproveResult
        /// </summary>
        public int ApproveResult { get; set; }

        /// <summary>
        /// ApproveRemark
        /// </summary>
        public string ApproveRemark { get; set; }

        /// <summary>
        /// 订单编号
        /// </summary>
        public string OrderNumbers { get; set; }

        /// <summary>
        /// 是否有服务费发票
        /// </summary>
        public bool HasServiceChargeInvoice { get; set; }

        /// <summary>
        /// 服务费发票
        /// </summary>
        public string ServiceChargeInvoice { get; set; }

        /// <summary>
        /// 服务费发票税点
        /// </summary>
        public decimal ServiceChargeTaxPoint { get; set; }

        /// <summary>
        /// 服务费支付记录
        /// </summary>
        public string ServiceChargePayLog { get; set; }

        /// <summary>
        /// 实际成交价
        /// </summary>
        public decimal ActualAmount { get; set; }

        /// <summary>
        /// 实际利润
        /// </summary>
        public decimal ActualProfit { get; set; }

        /// <summary>
        /// 回款记录
        /// </summary>
        public string PaymentLog { get; set; }

        /// <summary>
        /// 核算利润
        /// </summary>
        public decimal BusinessProfit { get; set; }

        /// <summary>
        /// 阶段
        /// </summary>
        public int Step { get; set; }

        public int Status { get; set; }
    }
}
