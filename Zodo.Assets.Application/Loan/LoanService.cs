using System;
using System.Data;
using System.Collections.Generic;
using HZC.Infrastructure;
using HZC.SearchUtil;
using Zodo.Assets.Core;

namespace Zodo.Assets.Application
{
    public partial class LoanService : BaseService<Loan>
    {

        public PageList<LoanDto> PageListDto(LoanSearchParam param = null, int pageIndex = 1, int pageSize = 20, string cols = "*")
        {
            if (param == null)
            {
                param = new LoanSearchParam();
            }
            MySearchUtil util = param.ToSearchUtil();
            return db.Query<LoanDto>(util, pageIndex, pageSize, table: "Asset_Loan", cols: cols);
        }

        public IEnumerable<LoanDto> ListDto(LoanSearchParam param = null, string cols = "*")
        {
            if (param == null)
            {
                param = new LoanSearchParam();
            }
            MySearchUtil util = param.ToSearchUtil();
            return db.Fetch<LoanDto>(util, table: "Asset_Loan", cols: cols);
        }

        #region 实体验证
        public override string ValidCreate(Loan entity, IAppUser user)
        {
            return string.Empty;
        }

        public override string ValidUpdate(Loan entity, IAppUser user)
        {
            return string.Empty;
        }

        public override string ValidDelete(Loan entity, IAppUser user)
        {
            return string.Empty;
        }
        #endregion
    }
}

