using HZC.Infrastructure;
using HZC.SearchUtil;
using System.Collections.Generic;
using Zodo.Assets.Core;

namespace Zodo.Assets.Application
{
    public class LoanService : BaseService<Loan>
    {
        public PageList<LoanDto> PageListDto(LoanSearchParam param = null, int pageIndex = 1, int pageSize = 20, string cols = "*")
        {
            if (param == null)
            {
                param = new LoanSearchParam();
            }
            var util = param.ToSearchUtil();
            return db.Query<LoanDto>(util, pageIndex, pageSize, "Asset_Loan", cols);
        }

        public IEnumerable<LoanDto> ListDto(LoanSearchParam param = null, string cols = "*")
        {
            if (param == null)
            {
                param = new LoanSearchParam();
            }
            MySearchUtil util = param.ToSearchUtil();
            return db.Fetch<LoanDto>(util, "Asset_Loan", cols);
        }

        #region 实体验证
        public override string ValidCreate(Loan entity, IAppUser user) => string.Empty;

        public override string ValidUpdate(Loan entity, IAppUser user) => string.Empty;

        public override string ValidDelete(Loan entity, IAppUser user) => string.Empty;

        #endregion
    }
}

