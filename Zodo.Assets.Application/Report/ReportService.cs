using HZC.Database;
using System.Collections.Generic;
using System.Linq;

namespace Zodo.Assets.Application
{
    public class ReportService
    {
        private readonly MyDbUtil _db = new MyDbUtil();

        public List<IdSummaryDto> DeptSummary()
        {
            var sql = "SELECT DeptId AS Id,COUNT(0) AS Num FROM Asset_Asset WHERE IsDel=0 GROUP BY DeptId";
            return _db.FetchBySql<IdSummaryDto>(sql).ToList();
        }

        public List<IdSummaryDto> CateSummary()
        {
            var sql = "SELECT AssetCateId AS Id,COUNT(0) AS Num FROM Asset_Asset WHERE IsDel=0 GROUP BY AssetCateId";
            return _db.FetchBySql<IdSummaryDto>(sql).ToList();
        }

        public List<StringSummaryDto> StateSummary()
        {
            var sql = "SELECT [State] AS Property,COUNT(0) AS Num FROM Asset_Asset WHERE IsDel=0 GROUP BY [State]";
            return _db.FetchBySql<StringSummaryDto>(sql).ToList();
        }

        public List<StringSummaryDto> HealthySummary()
        {
            var sql = "SELECT [Healthy] AS Property,COUNT(0) AS Num FROM Asset_Asset WHERE IsDel=0 GROUP BY [Healthy]";
            return _db.FetchBySql<StringSummaryDto>(sql).ToList();
        }
    }
}
