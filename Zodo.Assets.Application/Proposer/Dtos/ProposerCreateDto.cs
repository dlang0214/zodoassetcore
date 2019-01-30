using System;

namespace Zodo.Assets.Application
{
    public class ProposerCreateDto
    {
        public string ProposerId { get; set; }

        public string ProposerName { get; set; }

        public string CustomerUnit { get; set; }

        public string CustomerName { get; set; }

        public string CustomerJob { get; set; }

        public string CustomerMobile { get; set; }

        public string ProjectName { get; set; }

        public DateTime PlanDealAt { get; set; }

        public decimal PlanAmount { get; set; }

        public decimal PlanProfit { get; set; }

        public decimal ServiceChargeAmount { get; set; }

        public string Reason { get; set; }

        public string Approvers { get; set; }
    }
}
