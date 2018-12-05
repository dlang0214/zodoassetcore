using System;

namespace Zodo.Assets.Services
{
    public class StockItemDto
    {
        public int Id { get; set; }

        public int StockId { get; set; }

        public bool IsFinish { get; set; }

        public int AssetId { get; set; }

        public string AssetCode { get; set; }

        public string AssetName { get; set; }

        public int DeptId { get; set; }

        public string DeptName { get; set; }

        public string Position { get; set; }

        public int AccountId { get; set; }

        public string AccountName { get; set; }

        public DateTime? CheckAt { get; set; }

        public string Checkor { get; set; }

        public int CheckMethod { get; set; }

        public int CheckResult { get; set; }

        public string Remark { get; set; }

        public string FinancialCode { get; set; }

        public string Band { get; set; }

        public string Model { get; set; }

        public string Imei { get; set; }

        public string Specification { get; set; }

        public string Healthy { get; set; }

        public string State { get; set; }
    }
}
